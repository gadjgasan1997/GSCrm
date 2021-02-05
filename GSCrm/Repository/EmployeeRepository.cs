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
using static GSCrm.CommonConsts;
using static GSCrm.Utils.CollectionsUtils;
using GSCrm.Data;
using GSCrm.Transactions;
using GSCrm.Models.ViewTypes;
using GSCrm.Models.Enums;
using GSCrm.Notifications.Factories.UserNotFactories0;
using GSCrm.Notifications.Params;
using Microsoft.AspNetCore.Mvc;

namespace GSCrm.Repository
{
    public class EmployeeRepository : BaseRepository<Employee, EmployeeViewModel>
    {
        #region Declarations
        private readonly UserManager<User> userManager;
        /// <summary>
        /// Все основные типы представлений, связанные с сотрудником
        /// </summary>
        public static EmployeeViewType[] EmpBaseViewTypes => new EmployeeViewType[] {
            EmployeeViewType.EMP_POSITIONS, EmployeeViewType.EMP_CONTACTS, EmployeeViewType.EMP_SUBS };
        #endregion

        #region Construts
        public EmployeeRepository(IServiceProvider serviceProvider, ApplicationDbContext context, UserManager<User> userManager = null)
            : base(serviceProvider, context)
        {
            this.userManager = serviceProvider.GetService(typeof(UserManager<User>)) as UserManager<User>;
        }
        #endregion

        #region Override Methods
        public override bool HasPermissionsForSeeItem(Employee employee)
            => new OrganizationRepository(serviceProvider, context).HasPermissionsForSeeOrgItem();

        protected override bool RespsIsCorrectOnCreate(EmployeeViewModel employeeViewModel)
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("EmpCreate", transaction);

        protected override bool TryCreatePrepare(EmployeeViewModel employeeViewModel)
        {
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (currentOrganization == null)
                        errors.Add("OrganizationNotFound", resManager.GetString("OrganizationNotFound"));
                },
                () => CheckEmployeeAccount(employeeViewModel),
                () => {
                    new PersonValidator(resManager)
                        .CheckPersonName(employeeViewModel.FirstName, employeeViewModel.LastName, employeeViewModel.MiddleName, ref errors);
                },
                () => {
                    new PersonValidator(resManager).CheckPersonEmail(employeeViewModel.Email, errors);
                },
                () => CheckDivisionLength(employeeViewModel),
                () => DivisionExists(currentOrganization.GetDivisions(context), employeeViewModel.DivisionName),
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
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("EmpUpdate", transaction);

        protected override bool TryUpdatePrepare(EmployeeViewModel employeeViewModel)
        {
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (currentOrganization == null)
                        errors.Add("OrganizationNotFound", resManager.GetString("OrganizationNotFound"));
                },
                () => {
                    new PersonValidator(resManager).CheckPersonName(employeeViewModel.FirstName,
                        employeeViewModel.LastName, employeeViewModel.MiddleName, ref errors);
                }
            });
            return !errors.Any();
        }

        protected override void FailureUpdateHandler(EmployeeViewModel employeeViewModel)
        {
            if (TryGetItemById(employeeViewModel.Id, out Employee employee))
            {
                employeeViewModel = map.DataToViewModel(employee);
                employeeViewModel = new EmployeeMap(serviceProvider, context).Refresh(employeeViewModel, currentUser, EmpBaseViewTypes);
                AttachPositions(employeeViewModel);
                AttachContacts(employeeViewModel);
                AttachSubordinates(employeeViewModel);
            }
        }

        protected override bool RespsIsCorrectOnDelete(Employee employee)
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("EmpDelete", transaction);

        protected override bool TryDeletePrepare(Employee employee)
        {
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
            if (currentOrganization == null)
            {
                errors.Add("OrganizationNotFound", resManager.GetString("OrganizationNotFound"));
                return false;
            }
            
            CheckEmployeePositions(employee);
            RemoveUserOrganization(employee);
            new AccountRepository(serviceProvider, context).CheckAccountsForLock(employee, transaction);
            return true;
        }
        #endregion

        #region Searching
        /// <summary>
        /// Метод очищает поиск по должностям
        /// </summary>
        public void ClearPositionSearch()
        {
            EmployeeViewModel empViewModelCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, EMP_POSITIONS);
            empViewModelCash.SearchPosName = default;
            empViewModelCash.SearchParentPosName = default;
            cachService.CacheItem(currentUser.Id, EMP_POSITIONS, empViewModelCash);
        }

        /// <summary>
        /// Метод очищает поиск по должностям
        /// </summary>
        public void ClearContactSearch()
        {
            EmployeeViewModel empViewModelCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, EMP_CONTACTS);
            empViewModelCash.SearchContactType = default;
            empViewModelCash.SearchContactPhone = default;
            empViewModelCash.SearchContactEmail = default;
            cachService.CacheItem(currentUser.Id, EMP_CONTACTS, empViewModelCash);
        }

        /// <summary>
        /// Метод очищает поиск по подчиненным
        /// </summary>
        public void ClearSubordinateSearch()
        {
            EmployeeViewModel empViewModelCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, EMP_SUBS);
            empViewModelCash.SearchSubordinateFullName = default;
            cachService.CacheItem(currentUser.Id, EMP_SUBS, empViewModelCash);
        }
        #endregion

        #region Attaching Positions
        /// <summary>
        /// Добавляет должности к сотруднику, преобразую данные в отображение для выбранной страницы
        /// </summary>
        /// <returns></returns>
        public void AttachPositions(EmployeeViewModel employeeViewModel)
        {
            employeeViewModel.EmployeePositionViewModels = employeeViewModel.GetPositions(context)
                .MapToViewModels(new EmployeePositionMap(serviceProvider, context), GetLimitedPositionsList);
        }

        private List<EmployeePosition> GetLimitedPositionsList(List<EmployeePosition> positions)
        {
            EmployeeViewModel employeeViewModelCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, EMP_POSITIONS);
            List<EmployeePosition> limitedPositions = positions;
            LimitPosByName(employeeViewModelCash, ref limitedPositions);
            LimitPosByParent(employeeViewModelCash, ref limitedPositions);
            LimitListByPageNumber(EMP_POSITIONS, ref limitedPositions);
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
        {
            employeeViewModel.EmployeeContactViewModels = employeeViewModel.GetContacts(context)
                .MapToViewModels(new EmployeeContactMap(serviceProvider, context), GetLimitedContactsList);
        }

        private List<EmployeeContact> GetLimitedContactsList(List<EmployeeContact> contacts)
        {
            EmployeeViewModel employeeViewModelCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, EMP_CONTACTS);
            List<EmployeeContact> limitedContacts = contacts;
            LimitContactsByType(employeeViewModelCash, ref limitedContacts);
            LimitContactsByPhone(employeeViewModelCash, ref limitedContacts);
            LimitContactsByEmail(employeeViewModelCash, ref limitedContacts);
            LimitListByPageNumber(EMP_CONTACTS, ref limitedContacts);
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
        {
            employeeViewModel.SubordinatesViewModels = employeeViewModel.GetSubordinates(context)
                .MapToViewModels(new EmployeeMap(serviceProvider, context), GetLimitedSubordinatesList);
        }

        private List<Employee> GetLimitedSubordinatesList(List<Employee> employees)
        {
            EmployeeViewModel employeeViewModelCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, EMP_SUBS);
            List<Employee> limitedSubordinates = employees;
            LimitSubordinatesByFullName(employeeViewModelCash, ref limitedSubordinates);
            LimitListByPageNumber(EMP_SUBS, ref limitedSubordinates);
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
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
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
                    if (!new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("EmpChangeDiv", transaction))
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
            Employee employee = (Employee)transaction.GetParameterValue("Employee");
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
                    if (!new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("EmpUnlock", transaction))
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
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (!new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("EmpUnlock", transaction))
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
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (!new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("EmpUnlock", transaction))
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
            errors = new Dictionary<string, string>();
            transaction = viewModelsTransactionFactory.Create(currentUser.Id, OperationType.ChangeEmployeeDivision, employeeViewModel);
            if (TryChangeDivisionValidate(employeeViewModel))
            {
                new EmployeeMap(serviceProvider, context).ChangeDivision(employeeViewModel);
                if (viewModelsTransactionFactory.TryCommit(transaction, this.errors))
                {
                    viewModelsTransactionFactory.Close(transaction);
                    return true;
                }
            }
            viewModelsTransactionFactory.Close(transaction, TransactionStatus.Error);
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
            // Получение сотрудника из бд
            transaction = viewModelsTransactionFactory.Create(currentUser.Id, OperationType.UnlockEmployee, employeeViewModel);
            Employee employee = (Employee)transaction.GetParameterValue("Employee");

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
                        if (viewModelsTransactionFactory.TryCommit(transaction, this.errors))
                        {
                            viewModelsTransactionFactory.Close(transaction);
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
                        if (viewModelsTransactionFactory.TryCommit(transaction, this.errors))
                        {
                            viewModelsTransactionFactory.Close(transaction);
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
                        if (viewModelsTransactionFactory.TryCommit(transaction, this.errors))
                        {
                            viewModelsTransactionFactory.Close(transaction);
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
            viewModelsTransactionFactory.Close(transaction, TransactionStatus.Error);
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
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
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
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
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
