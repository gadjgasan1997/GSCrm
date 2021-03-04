using GSCrm.Mapping;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using GSCrm.Data;
using GSCrm.Transactions;
using GSCrm.Models.Enums;
using GSCrm.Notifications.Factories.UserNotFactories0;
using GSCrm.Notifications.Params;
using Microsoft.AspNetCore.Mvc;
using static GSCrm.CommonConsts;
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Repository
{
    public class EmployeeRepository : BaseRepository<Employee, EmployeeViewModel>
    {
        #region Construts
        public EmployeeRepository(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(serviceProvider, context)
        { }
        #endregion

        #region Override Methods
        public override bool HasPermissionsForSeeItem(Employee employee)
            => new OrganizationRepository(serviceProvider, context).HasPermissionsForSeeOrgItem();

        public override EmployeeViewModel LoadView(Employee employee)
        {
            EmployeeViewModel employeeViewModel = cachService.GetCachedCurrentEntity<EmployeeViewModel>(currentUser);

            // Прикрепление всех сущностей
            if (employee.EmployeeStatus == EmployeeStatus.Active)
            {
                AttachContacts(employeeViewModel);
                AttachPositions(employeeViewModel);
                AttachSubordinates(employeeViewModel);
            }

            // Кеширование результата
            cachService.SetCurrentView(currentUser.Id, EMPLOYEE);
            cachService.CacheEntity(currentUser, employeeViewModel);
            cachService.CacheCurrentEntity(currentUser, employeeViewModel);
            return employeeViewModel;
        }

        protected override bool RespsIsCorrectOnCreate(EmployeeViewModel employeeViewModel)
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("EmpCreate");

        protected override bool TryCreatePrepare(EmployeeViewModel employeeViewModel)
        {
            employeeViewModel.Normalize();
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CheckEmployeeAccount(employeeViewModel),
                () => {
                    new PersonValidator(resManager)
                        .CheckPersonName(employeeViewModel.FirstName, employeeViewModel.LastName, employeeViewModel.MiddleName, ref errors);
                },
                () => {
                    new PersonValidator(resManager).CheckPersonEmail(employeeViewModel.Email, errors);
                },
                () => CheckDivisionLength(employeeViewModel),
                () => {
                    Organization currentOrganization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
                    DivisionExists(currentOrganization.GetDivisions(context), employeeViewModel.DivisionName);
                },
                () => CheckPositionLength(employeeViewModel),
                () => CheckPositionExists(employeeViewModel),
                () => TryPrepareEmployeeAccount(employeeViewModel),
                () => {
                    // В случае, если такой аккаунт существует, а не был создан организацией, пользователю высылается инвайт
                    if (employeeViewModel.UserAccountExists)
                        SendOrgInvite();
                }
            });
            return !errors.Any();
        }

        protected override bool RespsIsCorrectOnUpdate(EmployeeViewModel employeeViewModel)
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("EmpUpdate");

        protected override bool TryUpdatePrepare(EmployeeViewModel employeeViewModel)
        {
            employeeViewModel.Normalize();
            new PersonValidator(resManager).CheckPersonName(employeeViewModel.FirstName,
                employeeViewModel.LastName, employeeViewModel.MiddleName, ref errors);
            return !errors.Any();
        }

        protected override bool RespsIsCorrectOnDelete(Employee employee)
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("EmpDelete");

        protected override bool TryDeletePrepare(Employee employee)
        {
            CheckEmployeePositions(employee);
            RemoveUserOrganization(employee);
            new AccountRepository(serviceProvider, context).CheckAccountsForLock(employee, transaction);
            return true;
        }
        #endregion

        #region Searching
        public void SearchContact(EmployeeViewModel employeeViewModel)
        {
            employeeViewModel.NormalizeSearch();
            EmployeeViewModel cachedViewModel = cachService.GetCachedCurrentEntity<EmployeeViewModel>(currentUser);
            cachedViewModel.SearchContactType = employeeViewModel.SearchContactType;
            cachedViewModel.SearchContactPhone = employeeViewModel.SearchContactPhone;
            cachedViewModel.SearchContactEmail = employeeViewModel.SearchContactEmail;
        }

        public void ClearContactSearch()
        {
            EmployeeViewModel cachedViewModel = cachService.GetCachedCurrentEntity<EmployeeViewModel>(currentUser);
            cachedViewModel.SearchContactType = default;
            cachedViewModel.SearchContactPhone = default;
            cachedViewModel.SearchContactEmail = default;
        }

        public void SearchSubordinate(EmployeeViewModel employeeViewModel)
        {
            employeeViewModel.NormalizeSearch();
            EmployeeViewModel cachedViewModel = cachService.GetCachedCurrentEntity<EmployeeViewModel>(currentUser);
            cachedViewModel.SearchSubordinateFullName = employeeViewModel.SearchSubordinateFullName;
        }

        public void ClearSubordinateSearch()
        {
            EmployeeViewModel cachedViewModel = cachService.GetCachedCurrentEntity<EmployeeViewModel>(currentUser);
            cachedViewModel.SearchSubordinateFullName = default;
        }
        #endregion

        #region Attaching Positions
        /// <summary>
        /// Добавляет должности к сотруднику, преобразую данные в отображение для выбранной страницы
        /// </summary>
        /// <returns></returns>
        public void AttachPositions(EmployeeViewModel employeeViewModel)
            => employeeViewModel.EmployeePositionViewModels = employeeViewModel.GetPositions(context)
                .MapToViewModels(employeeViewModel, new EmployeePositionMap(serviceProvider, context), (employeeViewModel, positions) =>
                    GetLimitedPositionsList(employeeViewModel, positions));

        private List<EmployeePosition> GetLimitedPositionsList(EmployeeViewModel employeeViewModel, List<EmployeePosition> positions)
        {
            List<EmployeePosition> limitedPositions = positions;
            LimitPosByName(employeeViewModel, ref limitedPositions);
            LimitPosByParent(employeeViewModel, ref limitedPositions);
            LimitViewItemsByPageNumber(employeeViewModel.Id, EMP_POSITIONS, ref limitedPositions);
            return limitedPositions;
        }

        /// <summary>
        /// Ограничение списка подразделений по названию
        /// </summary>
        /// <param name="employeeViewModelCash"></param>
        /// <param name="positionsToLimit"></param>
        private void LimitPosByName(EmployeeViewModel employeeViewModelCash, ref List<EmployeePosition> positionsToLimit)
        {
            if (!string.IsNullOrEmpty(employeeViewModelCash.SearchPosName))
            {
                TransformCollection(
                    collectionToLimit: ref positionsToLimit,
                    limitingCollection: employeeViewModelCash.GetDivision(context).GetPositions(context),
                    limitCondition: n => n.Name.ToLower().Contains(employeeViewModelCash.SearchPosName),
                    selectCondition: i => i.Id,
                    removeCondition: (positionIdList, employeePosition) => employeePosition.PositionId == null || !positionIdList.Contains((Guid)employeePosition.PositionId));
            }
        }

        /// <summary>
        /// Ограничение списка подразделений по названию родительского
        /// </summary>
        /// <param name="employeeViewModelCash"></param>
        /// <param name="positionsToLimit"></param>
        private void LimitPosByParent(EmployeeViewModel employeeViewModelCash, ref List<EmployeePosition> positionsToLimit)
        {
            if (!string.IsNullOrEmpty(employeeViewModelCash.SearchParentPosName))
                positionsToLimit = positionsToLimit.LimitByParent(context, employeeViewModelCash, employeeViewModelCash.SearchParentPosName);
        }
        #endregion

        #region Attaching Contacts
        /// <summary>
        /// Добавляет контакты к сотруднику, преобразую данные в отображение для выбранной страницы
        /// </summary>
        /// <returns></returns>
        public void AttachContacts(EmployeeViewModel employeeViewModel)
            => employeeViewModel.EmployeeContactViewModels = employeeViewModel.GetContacts(context)
                .MapToViewModels(employeeViewModel, new EmployeeContactMap(serviceProvider, context), (employeeViewModel, contacts) =>
                    GetLimitedContactsList(employeeViewModel, contacts));

        private List<EmployeeContact> GetLimitedContactsList(EmployeeViewModel employeeViewModel, List<EmployeeContact> contacts)
        {
            List<EmployeeContact> limitedContacts = contacts;
            LimitContactsByType(employeeViewModel, ref limitedContacts);
            LimitContactsByPhone(employeeViewModel, ref limitedContacts);
            LimitContactsByEmail(employeeViewModel, ref limitedContacts);
            LimitViewItemsByPageNumber(employeeViewModel.Id, EMP_CONTACTS, ref limitedContacts);
            return limitedContacts;
        }

        /// <summary>
        /// Ограничение списка контактов по типу
        /// </summary>
        /// <param name="employeeViewModelCash"></param>
        /// <param name="contactsToLimit"></param>
        private void LimitContactsByType(EmployeeViewModel employeeViewModelCash, ref List<EmployeeContact> contactsToLimit)
        {
            if (!string.IsNullOrEmpty(employeeViewModelCash.SearchContactType))
                contactsToLimit = contactsToLimit.Where(t => t.ContactType.ToString() == employeeViewModelCash.SearchContactType).ToList();
        }

        /// <summary>
        /// Ограничение списка контактов по телефону
        /// </summary>
        /// <param name="employeeViewModelCash"></param>
        /// <param name="contactsToLimit"></param>
        private void LimitContactsByPhone(EmployeeViewModel employeeViewModelCash, ref List<EmployeeContact> contactsToLimit)
        {
            if (!string.IsNullOrEmpty(employeeViewModelCash.SearchContactPhone))
                contactsToLimit = contactsToLimit.Where(p => p.PhoneNumber.ToLower().Contains(employeeViewModelCash.SearchContactPhone)).ToList();
        }

        /// <summary>
        /// Ограничение списка контактов по почте
        /// </summary>
        /// <param name="employeeViewModelCash"></param>
        /// <param name="contactsToLimit"></param>
        private void LimitContactsByEmail(EmployeeViewModel employeeViewModelCash, ref List<EmployeeContact> contactsToLimit)
        {
            if (!string.IsNullOrEmpty(employeeViewModelCash.SearchContactEmail))
                contactsToLimit = contactsToLimit.Where(p => p.Email.ToLower().Contains(employeeViewModelCash.SearchContactEmail)).ToList();
        }
        #endregion

        #region Attaching Subordinates
        /// <summary>
        /// Добавляет подчиненных к сотруднику, преобразую данные в отображение для выбранной страницы
        /// </summary>
        /// <returns></returns>
        public void AttachSubordinates(EmployeeViewModel employeeViewModel)
            => employeeViewModel.SubordinatesViewModels = employeeViewModel.GetSubordinates(context)
                .MapToViewModels(employeeViewModel, new EmployeeMap(serviceProvider, context), (employeeViewModel, subordinates) =>
                    GetLimitedSubordinatesList(employeeViewModel, subordinates));

        private List<Employee> GetLimitedSubordinatesList(EmployeeViewModel employeeViewModel, List<Employee> employees)
        {
            List<Employee> limitedSubordinates = employees;
            LimitSubordinatesByFullName(employeeViewModel, ref limitedSubordinates);
            LimitViewItemsByPageNumber(employeeViewModel.Id, EMP_SUBS, ref limitedSubordinates);
            return limitedSubordinates;
        }

        /// <summary>
        /// Метод ограничивает список подчиненных по полному имени
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <param name="employeesToLimit"></param>
        private void LimitSubordinatesByFullName(EmployeeViewModel employeeViewModelCash, ref List<Employee> employeesToLimit)
        {
            if (!string.IsNullOrEmpty(employeeViewModelCash.SearchSubordinateFullName))
                employeesToLimit = employeesToLimit.Where(emp => emp.GetFullName().ToLower().Contains(employeeViewModelCash.SearchSubordinateFullName)).ToList();
        }
        #endregion

        #region Validations
        /// <summary>
        /// Проверяет прикрепленный к работнику или вновь созданный аккаунт
        /// </summary>
        /// <param name="employeeViewModel"></param>
        private void CheckEmployeeAccount(EmployeeViewModel employeeViewModel)
        {
            if (employeeViewModel.UserAccountExists)
                CheckExistsEmployeeAccount(employeeViewModel);
            else CheckNewEmployeeAccount(employeeViewModel);
        }

        /// <summary>
        /// Проверяет на существование и занятость акккаунт
        /// </summary>
        /// <param name="employeeViewModel"></param>
        private void CheckExistsEmployeeAccount(EmployeeViewModel employeeViewModel)
        {
            // Проверка имени пользователя
            if (string.IsNullOrEmpty(employeeViewModel.UserName))
            {
                errors.Add("UserNameIsNull", resManager.GetString("UserNameIsNull"));
                return;
            }

            // Проверка, что такая учетная запись существует в этой организации
            User user = context.Users
                .Include(u => u.UserOrganizations)
                .AsNoTracking().FirstOrDefault(n => n.UserName == employeeViewModel.UserName);
            if (user == null)
            {
                errors.Add("UserNotExists", resManager.GetString("UserNotExists"));
                return;
            }
            else transaction.AddParameter("UserAccount", user);

            // Проверка, что пользователю уже не было выслано приглашение
            Organization currentOrganization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
            if (user.UserOrganizations.FirstOrDefault(org => org.OrganizationId == currentOrganization.Id && !org.Accepted) != null)
            {
                errors.Add("UserAlreadyHasInviteNotification", resManager.GetString("UserAlreadyHasInviteNotification"));
                return;
            }

            // Проверка, что учетная запись не занята другим сотрудником
            Employee employeeWithSameAccount = context.GetOrgEmployees(employeeViewModel.OrganizationId).FirstOrDefault(i => i.UserId == Guid.Parse(user.Id));
            if (employeeWithSameAccount != null)
            {
                errors.Add("UserAccountIsBusy", $"{resManager.GetString("UserAccountIsBusy")}: {employeeWithSameAccount.GetIntialsFullName()}");
                return;
            }
        }

        /// <summary>
        /// Проверяет новый созданный акккаунт
        /// </summary>
        /// <param name="employeeViewModel"></param>
        private void CheckNewEmployeeAccount(EmployeeViewModel employeeViewModel)
        {
            AuthRepository authRepository = new AuthRepository(serviceProvider, context);
            UserViewModel userModel = new UserViewModel()
            {
                UserName = employeeViewModel.UserName,
                Email = employeeViewModel.Email,
                Password = employeeViewModel.Password,
                ConfirmPassword = employeeViewModel.ConfirmPassword
            };
            authRepository.TrySignupValidate(userModel);
            authRepository.Errors.ToList().ForEach(error => errors.Add(error.Key, error.Value));
        }

        /// <summary>
        /// Проверка на заполненность подразделения
        /// </summary>
        /// <param name="employeeViewModel"></param>
        private void CheckDivisionLength(EmployeeViewModel employeeViewModel)
        {
            if (string.IsNullOrEmpty(employeeViewModel.DivisionName))
                errors.Add("DivisionNameLength", resManager.GetString("DivisionNameLength"));
        }

        /// <summary>
        /// Метод проверяет наличие подразделения с таким именем
        /// </summary>
        /// <param name="allDivisions"></param>
        /// <param name="divisionName"></param>
        /// <returns></returns>
        private bool DivisionExists(List<Division> allDivisions, string divisionName)
        {
            Division division = allDivisions.FirstOrDefault(n => n.Name == divisionName);
            if (division != null)
            {
                transaction.AddParameter("Division", division);
                return true;
            }
            errors.Add("DivisionNotExists", resManager.GetString("DivisionNotExists"));
            return false;
        }

        /// <summary>
        /// Проверка на заполненность основной должности
        /// </summary>
        /// <param name="employeeViewModel"></param>
        private void CheckPositionLength(EmployeeViewModel employeeViewModel)
        {
            if (string.IsNullOrEmpty(employeeViewModel.PrimaryPositionName))
                errors.Add("PositionNameLength", resManager.GetString("PositionNameLength"));
        }

        /// <summary>
        /// Метод проверяет наличие должности в поданом на вход подразделении
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <param name="division"></param>
        private void CheckPositionExists(EmployeeViewModel employeeViewModel)
        {
            Division division = (Division)transaction.GetParameterValue("Division");
            Position primaryPosition = division.GetPositions(context).FirstOrDefault(n => n.Name == employeeViewModel.PrimaryPositionName);
            if (primaryPosition == null)
            {
                errors.Add("PositionNotExists", resManager.GetString("PositionNotExists"));
                return;
            }
            transaction.AddParameter("PrimaryPosition", primaryPosition);
        }

        /// <summary>
        /// Метод проверяет модель при изменении подразделения на сотруднике
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        private bool TryChangeDivisionValidate(EmployeeViewModel employeeViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (!new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("EmpChangeDiv"))
                         AddHasNoPermissionsError(OperationType.ChangeEmployeeDivision);
                },
                () => CheckDivisionLength(employeeViewModel),
                () => CheckPositionLength(employeeViewModel),
                () => CheckDivisionForSelected(employeeViewModel),
                () => {
                    if (DivisionExists((List<Division>)transaction.GetParameterValue("AllDivisions"), employeeViewModel.DivisionName))
                        CheckPositionExists(employeeViewModel);
                }
            });
            return !errors.Any();
        }

        /// <summary>
        /// Метод проверяет, что пользователь выбрал другое подразделение для сотрудника при его изменении
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <param name="division"></param>
        private void CheckDivisionForSelected(EmployeeViewModel employeeViewModel)
        {
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);
            List<Division> allDivisions = (List<Division>)transaction.GetParameterValue("AllDivisions");
            Division currentDivision = allDivisions.FirstOrDefault(i => i.Id == employee.DivisionId);
            if (currentDivision.Name == employeeViewModel.DivisionName)
                errors.Add("ThisEmployeeDivisionIsAlreadySelect", resManager.GetString("ThisEmployeeDivisionIsAlreadySelect"));
        }

        /// <summary>
        /// Метод проверяет модель при разблокировке сотрудника в случае блокировки из-за отсутствия должности
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <returns></returns>
        private bool TryUnlockValidateOnPositionAbsent(EmployeeViewModel employeeViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (!new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("EmpUnlock"))
                         AddHasNoPermissionsError(OperationType.UnlockEmployee);
                },
                () => CheckDivisionLength(employeeViewModel),
                () => CheckPositionLength(employeeViewModel),
                () => {
                    if (DivisionExists((List<Division>)transaction.GetParameterValue("AllDivisions"), employeeViewModel.DivisionName))
                        CheckPositionExists(employeeViewModel);
                }
            });
            return !errors.Any();
        }

        /// <summary>
        /// Метод проверяет модель при разблокировке сотрудника в случае его выхода из организации
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <returns></returns>
        private bool TryUnlockValidateOnUserAccountAbsent(EmployeeViewModel employeeViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (!new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("EmpUnlock"))
                         AddHasNoPermissionsError(OperationType.UnlockEmployee);
                },
                () => CheckEmployeeAccount(employeeViewModel),
                () => {
                    new PersonValidator(resManager).CheckPersonEmail(employeeViewModel.Email, errors);
                }
            });
            return !errors.Any();
        }

        /// <summary>
        /// Метод проверяет модель при разблокировке в случае, если заблокированный сотрудник вышел из организации
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <returns></returns>
        private bool TryUnlockValidateOnLockedEmployeeLeftOrg(EmployeeViewModel employeeViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (!new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("EmpUnlock"))
                         AddHasNoPermissionsError(OperationType.UnlockEmployee);
                },
                () => CheckEmployeeAccount(employeeViewModel),
                () => {
                    new PersonValidator(resManager).CheckPersonEmail(employeeViewModel.Email, errors);
                },
                () => CheckDivisionLength(employeeViewModel),
                () => CheckPositionLength(employeeViewModel),
                () => {
                    if (DivisionExists((List<Division>)transaction.GetParameterValue("AllDivisions"), employeeViewModel.DivisionName))
                        CheckPositionExists(employeeViewModel);
                }
            });
            return !errors.Any();
        }
        #endregion

        #region Other Methods
        /// <summary>
        /// Удаление записи из таблицы "UserOrganizations"
        /// </summary>
        /// <param name="employeeToDelete"></param>
        private void RemoveUserOrganization(Employee employeeToDelete)
        {
            Organization organization = employeeToDelete.GetOrganization(context);
            Func<UserOrganization, bool> predicate = i => i.UserId == employeeToDelete.UserId.ToString() && i.OrganizationId == organization.Id;
            UserOrganization userOrganization = context.UserOrganizations.AsNoTracking().FirstOrDefault(predicate);
            transaction.AddChange(userOrganization, EntityState.Deleted);
        }

        /// <summary>
        /// Метод выполняет попытку смены подразделения
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <returns></returns>
        public bool TryChangeDivision(EmployeeViewModel employeeViewModel, out Dictionary<string, string> errors)
        {
            employeeViewModel.Normalize();
            errors = new Dictionary<string, string>();
            transaction = viewModelsTF.Create(currentUser.Id, OperationType.ChangeEmployeeDivision, employeeViewModel);
            if (TryChangeDivisionValidate(employeeViewModel))
            {
                new EmployeeMap(serviceProvider, context).ChangeDivision(employeeViewModel);
                if (viewModelsTF.TryCommit(transaction, this.errors))
                {
                    viewModelsTF.Close(transaction);
                    return true;
                }
            }
            viewModelsTF.Close(transaction, TransactionStatus.Error);
            errors = this.errors;
            return false;
        }

        /// <summary>
        /// Метод пытает произвести разблокировку сотрудника, возникшую в результате отсутствия у него должности и/или подразделения
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public bool TryUnlock(ref EmployeeViewModel employeeViewModel, out Dictionary<string, string> errors)
        {
            employeeViewModel.Normalize();

            // Получение сотрудника из бд
            transaction = viewModelsTF.Create(currentUser.Id, OperationType.UnlockEmployee, employeeViewModel);
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);

            // В зависимости от причины, по которой был заблокирован сотрудник
            switch (employee.EmployeeLockReason)
            {
                // Разблокировка в случае блокировки из-за выхода сотрудника из организации или отказа от приглашения
                case EmployeeLockReason.RejectInvite:
                case EmployeeLockReason.EmployeeLeftOrganization:
                    if (TryUnlockValidateOnUserAccountAbsent(employeeViewModel) && TryPrepareEmployeeAccount(employeeViewModel))
                    {
                        // Маппинг
                        new EmployeeMap(serviceProvider, context).UnlockOnUserAccountAbsent(employeeViewModel.UserAccountExists);

                        // Попытка сделать коммит
                        if (viewModelsTF.TryCommit(transaction, this.errors))
                        {
                            viewModelsTF.Close(transaction);
                            if (employeeViewModel.UserAccountExists)
                                SendOrgInvite();
                            OnUnlockSuccess(ref employeeViewModel, employee);
                            errors = this.errors;
                            return true;
                        }
                    }
                    break;

                // Разблокировка в случае блокировки из-за отсутствия подразделения или основной должности
                case EmployeeLockReason.DivisionAbsent:
                case EmployeeLockReason.PrimaryPositionAbsent:
                    if (TryUnlockValidateOnPositionAbsent(employeeViewModel))
                    {
                        // Маппинг
                        new EmployeeMap(serviceProvider, context).UnlockOnPositionAbsent();

                        // Попытка сделать коммит
                        if (viewModelsTF.TryCommit(transaction, this.errors))
                        {
                            viewModelsTF.Close(transaction);
                            OnUnlockSuccess(ref employeeViewModel, employee);
                            errors = this.errors;
                            return true;
                        }
                    }
                    break;

                // Разблокировка в случае, если уже заблокированный сотрудник покинул организацию
                case EmployeeLockReason.LockedEmployeeLeftOrg:
                    if (TryUnlockValidateOnLockedEmployeeLeftOrg(employeeViewModel) && TryPrepareEmployeeAccount(employeeViewModel))
                    {
                        // Маппинг
                        new EmployeeMap(serviceProvider, context).UnlockOnLockedEmployeeLeftOrg(employeeViewModel.UserAccountExists);

                        // Попытка сделать коммит
                        if (viewModelsTF.TryCommit(transaction, this.errors))
                        {
                            viewModelsTF.Close(transaction);
                            if (employeeViewModel.UserAccountExists)
                                SendOrgInvite();
                            OnUnlockSuccess(ref employeeViewModel, employee);
                            errors = this.errors;
                            return true;
                        }
                    }
                    break;
            }

            // Иначе данные из бд преобразуются в данные для отображения без прикрепления контактов и должностей
            errors = this.errors;
            employeeViewModel = map.DataToViewModel(employee);
            viewModelsTF.Close(transaction, TransactionStatus.Error);
            return false;
        }

        /// <summary>
        /// Метод выполняет необходимые действия в случае упсешной разблокировки 
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <param name="employee"></param>
        private void OnUnlockSuccess(ref EmployeeViewModel employeeViewModel, Employee employee)
        {
            employeeViewModel = map.DataToViewModel(employee);
            AttachPositions(employeeViewModel);
            AttachContacts(employeeViewModel);
        }

        /// <summary>
        /// Метод выполняет подготовительные действия, необходимые при создании аккаунта сотрудника
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <returns></returns>
        private bool TryPrepareEmployeeAccount(EmployeeViewModel employeeViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (!employeeViewModel.UserAccountExists)
                        TryCreateUserAccount(employeeViewModel);
                },
                () => {
                    // Если аккаунт существует, то добавляется организация в список организаций пользователя с признаком, что приглашение в организацию не было принято
                    if (employeeViewModel.UserAccountExists)
                        CreateUserOrganization(false);
                    else
                    {
                        // Иначе организация добавляется в список организаций пользователя с признаком, что приглашение было приянто
                        // и добавляется настройка уведомлений от этой организации
                        CreateUserOrganization();
                        OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
                        organizationRepository.CreateOrgNotificationsSetting(transaction);
                    }
                }
            });
            return !errors.Any();
        }

        /// <summary>
        /// Метод проверяет существует ли пользователь с таким именем в организации, и если нет, создает его
        /// </summary>
        /// <param name="employeeViewModel"></param>
        private bool TryCreateUserAccount(EmployeeViewModel employeeViewModel)
        {
            // Создание пользователя
            AuthRepository authRepository = new AuthRepository(serviceProvider, context);
            User userAccount = authRepository.TryCreateUser(new UserViewModel()
            {
                UserName = employeeViewModel.UserName,
                Email = employeeViewModel.Email,
                Password = employeeViewModel.Password,
                ConfirmPassword = employeeViewModel.ConfirmPassword,
                EmailConfirmed = true
            }, errors, transaction).Result;

            // Если не было ошибок
            if (userAccount != null)
            {
                // Проставление Id основной организации новому сотруднику
                if (userAccount.PrimaryOrganizationId == Guid.Empty)
                    userAccount.PrimaryOrganizationId = currentUser.PrimaryOrganizationId;
                transaction.AddParameter("UserAccount", userAccount);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Метод добавляет организацию в список организаций пользователя
        /// </summary>
        /// <param name="acceptedFlag">Признак, принял ли пользователь приглашение в организацию</param>
        private void CreateUserOrganization(bool acceptedFlag = true)
        {
            User userAccount = (User)transaction.GetParameterValue("UserAccount");
            Organization currentOrganization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
            UserOrganization newUserOrganization = new UserOrganization()
            {
                Accepted = acceptedFlag,
                Organization = currentOrganization,
                OrganizationId = currentOrganization.Id,
                User = userAccount,
                UserId = userAccount.Id
            };
            transaction.AddChange(newUserOrganization, EntityState.Added);
            transaction.AddParameter("UserOrganization", newUserOrganization);
        }

        /// <summary>
        /// Метод отсылает пользователю приглашение вступить в организацию
        /// </summary>
        private void SendOrgInvite()
        {
            Organization currentOrganization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
            User existsUserAccount = (User)transaction.GetParameterValue("UserAccount");
            OrgInviteParams orgInviteParams = new OrgInviteParams()
            {
                Organization = currentOrganization,
                OrgInviteUrl = urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = currentOrganization.Id }, httpContext.Request.Scheme)
            };
            new OrgInviteNotFactory(serviceProvider, context, orgInviteParams).Send(Guid.Parse(existsUserAccount.Id));
        }

        /// <summary>
        /// Вызывается для корректного удаления сотрудника
        /// Метод проверяет необходимость установить id основного сотрудника в null для должностей,
        /// где удаляемый сотрудник является основным
        /// После этого помечает должность на удаление
        /// </summary>
        /// <param name="employee"></param>
        public void CheckEmployeePositions(Employee employee)
        {
            employee.AddEmployeePositions(context).EmployeePositions.ForEach(employeePosition =>
            {
                Position position = context.Positions.AsNoTracking().FirstOrDefault(i => i.Id == employeePosition.PositionId);
                if (position.PrimaryEmployeeId == employee.Id)
                {
                    position.PrimaryEmployeeId = null;
                    transaction.AddChange(position, EntityState.Modified);
                }
                transaction.AddChange(employeePosition, EntityState.Deleted);
            });
        }
        #endregion
    }
}
