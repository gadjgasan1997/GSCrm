using GSCrm.Data;
using GSCrm.Data.ApplicationInfo;
using GSCrm.DataTransformers;
using GSCrm.Helpers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Repository
{
    public class EmployeeRepository : GenericRepository<Employee, EmployeeViewModel, EmployeeValidator, EmployeeTransformer>
    {
        private User userAccount;
        private readonly User currentUser;
        private readonly UserManager<User> userManager;
        private Organization currentOrganization;
        public static EmployeeViewModel CurrentEmployee { get; set; }
        public EmployeeRepository(ApplicationDbContext context, ResManager resManager) : base(context, resManager) { }
        public EmployeeRepository(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager, HttpContext httpContext = null, UserManager<User> userManager = null)
            : base(context, viewsInfo, resManager, new EmployeeValidator(userManager, context, resManager), new EmployeeTransformer(context, resManager, httpContext))
        {
            this.userManager = userManager;
            if (httpContext != null)
                currentUser = httpContext.GetCurrentUser(context);
        }

        #region Override Methods
        public override bool TryCreatePrepare(EmployeeViewModel employeeViewModel, ModelStateDictionary modelState)
        {
            if (!base.TryCreatePrepare(employeeViewModel, modelState)) return false;
            SetUpCurrentOrganization(employeeViewModel);
            if (!TryPrepareUserAccount(employeeViewModel, modelState)) return false;
            SetUpUserOrganization(userAccount);
            employeeViewModel.UserId = userAccount.Id;

            // Проставление Id основной организации новому сотруднику
            if (userAccount.PrimaryOrganizationId == Guid.Empty)
                userAccount.PrimaryOrganizationId = currentUser.PrimaryOrganizationId;
            context.Users.Update(userAccount);
            return true;
        }

        public override void FailureUpdateHandler(EmployeeViewModel employeeViewModel, Action<EmployeeViewModel> handler = null)
        {
            if (TryGetItemById(employeeViewModel.Id, out Employee employee))
            {
                employeeViewModel = transformer.DataToViewModel(employee);
                employeeViewModel = transformer.UpdateViewModelFromCash(employeeViewModel);
                AttachContacts(employeeViewModel);
                AttachPositions(employeeViewModel);
            }
        }

        public override bool TryDeletePrepare(Guid id, Employee employee, ModelStateDictionary modelState)
        {
            if (!base.TryDeletePrepare(id, employee, modelState)) return false;
            RemoveUserOrganization(employee);
            new AccountRepository(context, resManager).CheckAccountsForLock(employee);
            return true;
        }
        #endregion

        #region Searching
        /// <summary>
        /// Метод устанавливает значения для поиска по должностям
        /// </summary>
        /// <param name="empViewModelCash"></param>
        /// <returns></returns>
        public void SearchPosition(EmployeeViewModel employeeViewModel)
        {
            //viewsInfo.Reset(EMP_POSITIONS);
            EmployeeViewModel empViewModelCash = ModelCash<EmployeeViewModel>.GetViewModel(currentUser.Id, EMP_POSITIONS);
            empViewModelCash.DivisionIdCash.AddOrReplace(currentUser.Id, employeeViewModel.DivisionId);
            empViewModelCash.SearchPosNameCash.AddOrReplace(currentUser.Id, employeeViewModel.SearchPosName?.ToLower().TrimStartAndEnd());
            empViewModelCash.SearchParentPosNameCash.AddOrReplace(currentUser.Id, employeeViewModel.SearchParentPosName?.ToLower().TrimStartAndEnd());
            ModelCash<EmployeeViewModel>.SetViewModel(currentUser.Id, EMP_POSITIONS, empViewModelCash);
        }

        /// <summary>
        /// Метод очищает поиск по должностям
        /// </summary>
        public void ClearPositionSearch()
        {
            EmployeeViewModel empViewModelCash = ModelCash<EmployeeViewModel>.GetViewModel(currentUser.Id, EMP_POSITIONS);
            empViewModelCash.SearchPosNameCash.AddOrReplace(currentUser.Id, default);
            empViewModelCash.SearchParentPosNameCash.AddOrReplace(currentUser.Id, default);
            ModelCash<EmployeeViewModel>.SetViewModel(currentUser.Id, EMP_POSITIONS, empViewModelCash);
        }

        /// <summary>
        /// Метод устанавливает значения для поиска по должностям
        /// </summary>
        /// <param name="empViewModelCash"></param>
        /// <returns></returns>
        public void SearchContact(EmployeeViewModel employeeViewModel)
        {
            //viewsInfo.Reset(EMP_CONTACTS);
            EmployeeViewModel empViewModelCash = ModelCash<EmployeeViewModel>.GetViewModel(currentUser.Id, EMP_CONTACTS);
            empViewModelCash.DivisionIdCash.AddOrReplace(currentUser.Id, employeeViewModel.DivisionId);
            empViewModelCash.SearchContactTypeCash.AddOrReplace(currentUser.Id, employeeViewModel.SearchContactType);
            empViewModelCash.SearchContactPhoneCash.AddOrReplace(currentUser.Id, employeeViewModel.SearchContactPhone?.ToLower().TrimStartAndEnd());
            empViewModelCash.SearchContactEmailCash.AddOrReplace(currentUser.Id, employeeViewModel.SearchContactEmail?.ToLower().TrimStartAndEnd());
            ModelCash<EmployeeViewModel>.SetViewModel(currentUser.Id, EMP_CONTACTS, empViewModelCash);
        }

        /// <summary>
        /// Метод очищает поиск по должностям
        /// </summary>
        public void ClearContactSearch()
        {
            EmployeeViewModel empViewModelCash = ModelCash<EmployeeViewModel>.GetViewModel(currentUser.Id, EMP_CONTACTS);
            empViewModelCash.SearchContactTypeCash.AddOrReplace(currentUser.Id, string.Empty);
            empViewModelCash.SearchContactPhoneCash.AddOrReplace(currentUser.Id, default);
            empViewModelCash.SearchContactEmailCash.AddOrReplace(currentUser.Id, default);
            ModelCash<EmployeeViewModel>.SetViewModel(currentUser.Id, EMP_CONTACTS, empViewModelCash);
        }

        /// <summary>
        /// Метод устанавливает значения для поиска по подчиненным
        /// </summary>
        /// <param name="empViewModelCash"></param>
        /// <returns></returns>
        public void SearchSubordinate(EmployeeViewModel employeeViewModel)
        {
            //viewsInfo.Reset(EMP_SUBS);
            EmployeeViewModel empViewModelCash = ModelCash<EmployeeViewModel>.GetViewModel(currentUser.Id, EMP_SUBS);
            empViewModelCash.SearchSubordinateFullNameCash.AddOrReplace(currentUser.Id, employeeViewModel.SearchSubordinateFullName?.ToLower().TrimStartAndEnd());
            ModelCash<EmployeeViewModel>.SetViewModel(currentUser.Id, EMP_SUBS, empViewModelCash);
        }

        /// <summary>
        /// Метод очищает поиск по подчиненным
        /// </summary>
        public void ClearSubordinateSearch()
        {
            EmployeeViewModel empViewModelCash = ModelCash<EmployeeViewModel>.GetViewModel(currentUser.Id, EMP_SUBS);
            empViewModelCash.SearchSubordinateFullNameCash.AddOrReplace(currentUser.Id, default);
            ModelCash<EmployeeViewModel>.SetViewModel(currentUser.Id, EMP_SUBS, empViewModelCash);
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
                .TransformToViewModels<EmployeePosition, EmployeePositionViewModel, EmployeePositionTransformer>(
                    transformer: new EmployeePositionTransformer(context, resManager),
                    limitingFunc: GetLimitedPositionsList);
        }

        private List<EmployeePosition> GetLimitedPositionsList(List<EmployeePosition> positions)
        {
            EmployeeViewModel employeeViewModelCash = ModelCash<EmployeeViewModel>.GetViewModel(currentUser.Id, EMP_POSITIONS);
            List<EmployeePosition> limitedPositions = positions;
            LimitPosByName(employeeViewModelCash, ref limitedPositions);
            LimitPosByParent(employeeViewModelCash, ref limitedPositions);
            LimitListByPageNumber(currentUser.Id, EMP_POSITIONS, ref limitedPositions);
            return limitedPositions;
        }

        /// <summary>
        /// Ограничение списка подразделений по названию
        /// </summary>
        /// <param name="employeeViewModelCash"></param>
        /// <param name="positionsToLimit"></param>
        private void LimitPosByName(EmployeeViewModel employeeViewModelCash, ref List<EmployeePosition> positionsToLimit)
        {
            string searchPosName = employeeViewModelCash.SearchPosNameCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchPosName))
            {
                employeeViewModelCash.DivisionId = employeeViewModelCash.DivisionIdCash.GetValueOrDefault(currentUser.Id);
                TransformCollection(
                    collectionToLimit: ref positionsToLimit,
                    limitingCollection: employeeViewModelCash.GetDivision(context).Positions,
                    limitCondition: n => n.Name.ToLower().Contains(searchPosName),
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
            string searchParentPosName = employeeViewModelCash.SearchParentPosNameCash.GetValueOrDefault(currentUser.Id);
            positionsToLimit = positionsToLimit.LimitByParent(context, employeeViewModelCash, searchParentPosName, currentUser.Id);
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
                .TransformToViewModels<EmployeeContact, EmployeeContactViewModel, EmployeeContactTransformer>(
                    transformer: new EmployeeContactTransformer(context, resManager),
                    limitingFunc: GetLimitedContactsList);
        }

        private List<EmployeeContact> GetLimitedContactsList(List<EmployeeContact> contacts)
        {
            EmployeeViewModel employeeViewModelCash = ModelCash<EmployeeViewModel>.GetViewModel(currentUser.Id, EMP_CONTACTS);
            List<EmployeeContact> limitedContacts = contacts;
            LimitContactsByType(employeeViewModelCash, ref limitedContacts);
            LimitContactsByPhone(employeeViewModelCash, ref limitedContacts);
            LimitContactsByEmail(employeeViewModelCash, ref limitedContacts);
            LimitListByPageNumber(currentUser.Id, EMP_CONTACTS, ref limitedContacts);
            return limitedContacts;
        }

        /// <summary>
        /// Ограничение списка контактов по типу
        /// </summary>
        /// <param name="employeeViewModelCash"></param>
        /// <param name="contactsToLimit"></param>
        private void LimitContactsByType(EmployeeViewModel employeeViewModelCash, ref List<EmployeeContact> contactsToLimit)
        {
            string searchContactType = employeeViewModelCash.SearchContactTypeCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchContactType))
                contactsToLimit = contactsToLimit.Where(t => t.ContactType.ToString() == searchContactType).ToList();
        }

        /// <summary>
        /// Ограничение списка контактов по телефону
        /// </summary>
        /// <param name="employeeViewModelCash"></param>
        /// <param name="contactsToLimit"></param>
        private void LimitContactsByPhone(EmployeeViewModel employeeViewModelCash, ref List<EmployeeContact> contactsToLimit)
        {
            string searchContactPhone = employeeViewModelCash.SearchContactPhoneCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchContactPhone))
                contactsToLimit = contactsToLimit.Where(p => p.PhoneNumber.ToLower().Contains(searchContactPhone)).ToList();
        }

        /// <summary>
        /// Ограничение списка контактов по почте
        /// </summary>
        /// <param name="employeeViewModelCash"></param>
        /// <param name="contactsToLimit"></param>
        private void LimitContactsByEmail(EmployeeViewModel employeeViewModelCash, ref List<EmployeeContact> contactsToLimit)
        {
            string searchContactEmail = employeeViewModelCash.SearchContactEmailCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchContactEmail))
                contactsToLimit = contactsToLimit.Where(p => p.Email.ToLower().Contains(searchContactEmail)).ToList();
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
                .TransformToViewModels<Employee, EmployeeViewModel, EmployeeTransformer>(
                    transformer: new EmployeeTransformer(context, resManager),
                    limitingFunc: GetLimitedSubordinatesList);
        }

        private List<Employee> GetLimitedSubordinatesList(List<Employee> employees)
        {
            EmployeeViewModel employeeViewModelCash = ModelCash<EmployeeViewModel>.GetViewModel(currentUser.Id, EMP_SUBS);
            List<Employee> limitedSubordinates = employees;
            LimitSubordinatesByFullName(employeeViewModelCash, ref limitedSubordinates);
            LimitListByPageNumber(currentUser.Id, EMP_SUBS, ref limitedSubordinates);
            return limitedSubordinates;
        }

        /// <summary>
        /// Метод ограничивает список подчиненных по полному имени
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <param name="employeesToLimit"></param>
        private void LimitSubordinatesByFullName(EmployeeViewModel employeeViewModelCash, ref List<Employee> employeesToLimit)
        {
            string searchSubordinateFullName = employeeViewModelCash.SearchSubordinateFullNameCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchSubordinateFullName))
                employeesToLimit = employeesToLimit.Where(emp => emp.GetFullName().ToLower().Contains(searchSubordinateFullName)).ToList();
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
            UserOrganization userOrganization = context.UserOrganizations.FirstOrDefault(predicate);
            context.Entry(userOrganization).State = EntityState.Deleted;
            context.UserOrganizations.Remove(userOrganization);
        }

        /// <summary>
        /// Метод выполняет попытку смены подразделения и, в случае неуспеха, записывает в модель состояния ошибки
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public bool TryChangeDivision(EmployeeViewModel employeeViewModel, ModelStateDictionary modelState)
        {
            if (TryChangeDivisionValidate(employeeViewModel, modelState))
            {
                SetUpCurrentOrganization(employeeViewModel);
                Division newDivision = GetNewDivision(employeeViewModel);
                Position newPrimaryPosition = GetNewPrimaryPosition(employeeViewModel, newDivision);
                Employee employee = context.Employees.Include(pos => pos.EmployeePositions).FirstOrDefault(i => i.Id == employeeViewModel.Id);
                SetNewDivision(employee, newDivision);
                RemoveOldEmployeePositions(employee);
                AddEmployeePosition(employee, newPrimaryPosition);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Метод пытает произвести разблокировку сотрудника, возникшую в результате отсутствия у него должности и/или подразделения
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public bool TryUnlock(ref EmployeeViewModel employeeViewModel, ModelStateDictionary modelState)
        {
            // Получение сотрудника из бд
            Guid employeeId = employeeViewModel.Id;
            Employee employee = context.Employees
                .Include(empPos => empPos.EmployeePositions)
                .FirstOrDefault(i => i.Id == employeeId);

            /* Если валидация проходит успешно, происходят действия по разблокировке и
            данные из бд преобразуются в данные для отображения с прикреплением контактов и должностей */
            if (TryUnlockValidate(employeeViewModel, modelState))
            {
                UnlockEmployeeSetDivision(employeeViewModel, employee);
                UnlockEmployeeSetPosition(employeeViewModel, employee);
                employee.Unlock();
                context.Update(employee);
                context.SaveChanges();

                employeeViewModel = transformer.DataToViewModel(employee);
                AttachPositions(employeeViewModel);
                AttachContacts(employeeViewModel);
                return true;
            }

            // Иначе данные из бд преобразуются в данные для отображения без прикрепления контактов и должностей
            else
            {
                employeeViewModel = transformer.DataToViewModel(employee);
                return false;
            }
        }

        /// <summary>
        /// Метод устанавливает новое подразделение при разблокировке сотруднкиа
        /// </summary>
        /// <param name="employeeViewModel"></param>
        private void UnlockEmployeeSetDivision(EmployeeViewModel employeeViewModel, Employee employee)
        {
            Division employeeDivision = employee.GetDivision(context);
            if (employeeDivision.Name != employeeViewModel.DivisionName)
            {
                List<Division> divisions = context.GetOrgDivisions(employeeViewModel.OrganizationId);
                Division newDivision = divisions.FirstOrDefault(n => n.Name == employeeViewModel.DivisionName);
                SetNewDivision(employee, newDivision);
            }
        }

        /// <summary>
        /// Метод устанавливает новую должность при разблокировке сотрудника
        /// </summary>
        /// <param name="employeeViewModel"></param>
        private void UnlockEmployeeSetPosition(EmployeeViewModel employeeViewModel, Employee employee)
        {
            Division employeeDivision = employee.GetDivision(context);
            Position newPosition = employeeDivision.Positions.FirstOrDefault(n => n.Name == employeeViewModel.PrimaryPositionName);
            if (!employee.EmployeePositions.ContainsPosition(newPosition))
                AddEmployeePosition(employee, newPosition);
            SetPrimaryPosition(employee, newPosition);
        }

        /// <summary>
        /// Устанаваливает текущую организацию
        /// </summary>
        /// <param name="employeeViewModel"></param>
        private void SetUpCurrentOrganization(EmployeeViewModel employeeViewModel) => currentOrganization = employeeViewModel.GetOrganization(context);

        /// <summary>
        /// Метод проверяет существует ли пользователь с таким именем в организации, и если нет, создает его
        /// </summary>
        /// <param name="employeeViewModel"></param>
        private bool TryPrepareUserAccount(EmployeeViewModel employeeViewModel, ModelStateDictionary modelState)
        {
            if (employeeViewModel.UserAccountExists)
                userAccount = context.Users.FirstOrDefault(n => n.UserName == employeeViewModel.UserName);
            else
            {
                userAccount = new User()
                {
                    UserName = employeeViewModel.UserName,
                    Email = employeeViewModel.Email,
                    EmailConfirmed = true
                };
                IdentityResult identityResult = userManager.CreateAsync(userAccount, employeeViewModel.Password).Result;
                if (!identityResult.Succeeded)
                {
                    foreach (IdentityError identityError in identityResult.Errors)
                        modelState.AddModelError(identityError.Code, identityError.Description);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Метод добавляет организацию в список организаций пользователя
        /// </summary>
        /// <param name="employeeViewModel"></param>
        private void SetUpUserOrganization(User user)
        {
            user.UserOrganizations.Add(new UserOrganization()
            {
                Organization = currentOrganization,
                OrganizationId = currentOrganization.Id,
                User = user,
                UserId = user.Id
            });
        }

        /// <summary>
        /// Метод проверяет модель при изменении подразделения на сотруднике
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        private bool TryChangeDivisionValidate(EmployeeViewModel employeeViewModel, ModelStateDictionary modelState)
        {
            Dictionary<string, string> errors = validator.ChangeDivisionCheck(employeeViewModel);
            if (errors.Any())
            {
                foreach (KeyValuePair<string, string> error in errors)
                    modelState.AddModelError(error.Key, error.Value);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Метод проверяет модель при разблокировке сотрудника
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        private bool TryUnlockValidate(EmployeeViewModel employeeViewModel, ModelStateDictionary modelState)
        {
            Dictionary<string, string> errors = validator.UnlockValidateCheck(employeeViewModel);
            if (errors.Any())
            {
                foreach (KeyValuePair<string, string> error in errors)
                    modelState.AddModelError(error.Key, error.Value);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Метод возвращает новое подразделение при смене подразделения на сотруднике
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <returns></returns>
        private Division GetNewDivision(EmployeeViewModel employeeViewModel)
        {
            string newDivisionName = employeeViewModel.DivisionName;
            return currentOrganization.Divisions.FirstOrDefault(n => n.Name == newDivisionName);
        }

        /// <summary>
        /// Метод возвращает новую должность при смене подразделения на сотруднике
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <param name="newDivision">Новое подразделение</param>
        /// <returns></returns>
        private Position GetNewPrimaryPosition(EmployeeViewModel employeeViewModel, Division newDivision)
        {
            string primaryPositionName = employeeViewModel.PrimaryPositionName;
            return newDivision.Positions.FirstOrDefault(n => n.Name == primaryPositionName);
        }

        /// <summary>
        /// Метод устанавливает новое подразделение у сотрудника
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="newDivision"></param>
        private void SetNewDivision(Employee employee, Division newDivision) => employee.DivisionId = newDivision.Id;

        /// <summary>
        /// Метод создает и добавляет новую должность в список должностей сотрудника
        /// и устанавливает пришедшую на вход должность как основную
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="newPrimaryPosition"></param>
        private void AddEmployeePosition(Employee employee, Position newPrimaryPosition)
        {
            EmployeePosition employeePosition = new EmployeePosition()
            {
                Id = Guid.NewGuid(),
                Employee = employee,
                EmployeeId = employee.Id,
                Position = newPrimaryPosition,
                PositionId = newPrimaryPosition.Id
            };
            SetPrimaryPosition(employee, newPrimaryPosition);
            context.EmployeePositions.Add(employeePosition);
        }

        /// <summary>
        /// Метод устанавливает для сотрудника основную должность
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="primaryPosition"></param>
        private void SetPrimaryPosition(Employee employee, Position primaryPosition) => employee.PrimaryPositionId = primaryPosition.Id;

        /// <summary>
        /// Удаление из контекста всех должностей сотрудника
        /// </summary>
        /// <param name="employee"></param>
        private void RemoveOldEmployeePositions(Employee employee)
        {
            employee.EmployeePositions.ForEach(employeePosition =>
            {
                context.EmployeePositions.Remove(employeePosition);
            });
        }
        #endregion
    }
}
