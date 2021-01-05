using GSCrm.Data;
using GSCrm.Mapping;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Models.ViewTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using GSCrm.Transactions;
using GSCrm.Models.Enums;
using static GSCrm.CommonConsts;
using static GSCrm.Utils.CollectionsUtils;
using GSCrm.Notifications;

namespace GSCrm.Repository
{
    public class OrganizationRepository : BaseRepository<Organization, OrganizationViewModel>
    {
        #region Declarations
        private const int ORGANIZATION_NAME_MIN_LENGTH = 3;
        /// <summary>
        /// Все типы представлений, связанные с организацией
        /// </summary>
        public static OrganizationViewType[] OrgAllViewTypes => new OrganizationViewType[] {
            OrganizationViewType.DIVISIONS, OrganizationViewType.POSITIONS, OrganizationViewType.EMPLOYEES, OrganizationViewType.RESPONSIBILITIES };
        /// <summary>
        /// Количество одновременно отоброжаемых полномочий организации
        /// Уровень доступа public, чтобы можно бьыло обратиться из представления
        /// </summary>
        public const int RESPONSIBILITIES_COUNT = 5;
        #endregion

        #region Constructs
        public OrganizationRepository(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(serviceProvider, context)
        { }
        #endregion

        #region Override Methods
        public override bool HasPermissionsForSeeItem(Organization organization)
        {
            if (organization == null) return false;
            cachService.CacheItem(currentUser.Id, $"{PC}Organization", organization);
            List<UserOrganization> userOrganizations = context.UserOrganizations.Where(userOrg => userOrg.UserId == currentUser.Id).ToList();
            cachService.CacheItems(currentUser.Id, $"{PC}UserOrganizations", userOrganizations);
            return userOrganizations.Select(userOrg => userOrg.OrganizationId).Contains(organization.Id);
        }

        protected override bool RespsIsCorrectOnCreate(OrganizationViewModel entityToCreate) => true;

        protected override bool TryCreatePrepare(OrganizationViewModel orgViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CheckOrganizationLength(orgViewModel),
                () => CheckOrganizationNotExists(orgViewModel)
            });
            return !errors.Any();
        }

        protected override bool RespsIsCorrectOnDelete(Organization organization) => organization.OwnerId == currentUser.Id;

        protected override void HasNotPermissionsForDelete()
            => errors.Add("AnotherOrgIsReadonly", resManager.GetString("AnotherOrgIsReadonly").Replace("&OrgName", recordToRemove.Name));
        #endregion

        #region Searching
        /// <summary>
        /// Метод очищает поиск по организациям
        /// </summary>
        public void ClearSearch()
        {
            OrganizationsViewModel orgsViewModelCash = cachService.GetCachedItem<OrganizationsViewModel>(currentUser.Id, ORGANIZATIONS);
            orgsViewModelCash.SearchName = default;
            cachService.CacheItem(currentUser.Id, ORGANIZATIONS, orgsViewModelCash);
        }

        /// <summary>
        /// Метод очищает кеш для модели с подразделениями
        /// </summary>
        public void ClearDivisionSearch()
        {
            OrganizationViewModel orgViewModelCash = cachService.GetCachedItem<OrganizationViewModel>(currentUser.Id, DIVISIONS);
            orgViewModelCash.SearchDivName = default;
            orgViewModelCash.SearchParentDivName = default;
            cachService.CacheItem(currentUser.Id, DIVISIONS, orgViewModelCash);
        }

        /// <summary>
        /// Метод очищает кеш для модели с должностями
        /// </summary>
        public void ClearPositionSearch()
        {
            OrganizationViewModel orgViewModelCash = cachService.GetCachedItem<OrganizationViewModel>(currentUser.Id, POSITIONS);
            orgViewModelCash.SearchPosName = default;
            orgViewModelCash.SeacrhPositionDivName = default;
            orgViewModelCash.SearchPrimaryEmployeeName = default;
            orgViewModelCash.SearchParentPosName = default;
            cachService.CacheItem(currentUser.Id, POSITIONS, orgViewModelCash);
        }

        /// <summary>
        /// Метод очищает кеш для модели с сотрудниками
        /// </summary>
        public void ClearEmployeeSearch()
        {
            OrganizationViewModel orgViewModelCash = cachService.GetCachedItem<OrganizationViewModel>(currentUser.Id, EMPLOYEES);
            orgViewModelCash.SearchEmployeeName = default;
            orgViewModelCash.SearchEmployeePrimaryPosName = default;
            orgViewModelCash.SeacrhEmployeeDivName = default;
            cachService.CacheItem(currentUser.Id, EMPLOYEES, orgViewModelCash);
        }

        /// <summary>
        /// Метод очищает кеш для модели с полномочиями
        /// </summary>
        public void ClearResponsibilitySearch()
        {
            OrganizationViewModel orgViewModelCash = cachService.GetCachedItem<OrganizationViewModel>(currentUser.Id, RESPONSIBILITIES);
            orgViewModelCash.SeacrhResponsibilityName = default;
            cachService.CacheItem(currentUser.Id, RESPONSIBILITIES, orgViewModelCash);
        }
        #endregion

        #region Attaching Organizations
        /// <summary>
        /// Метод добавляет список моделей представления организаций
        /// </summary>
        /// <param name="orgsViewModel"></param>
        public void AttachOrganizations(ref OrganizationsViewModel orgsViewModel)
        {
            orgsViewModel.OrganizationViewModels = context.GetOrganizations(currentUser)
                .MapToViewModels(new OrganizationMap(serviceProvider, context), GetLimitedList);
        }

        /// <summary>
        /// Метод ограничивает список организаций по существующему фильтру
        /// </summary>
        /// <param name="organizations"></param>
        /// <returns></returns>
        private List<Organization> GetLimitedList(List<Organization> organizations)
        {
            List<Organization> limitedOrgs = GetLimitedOrgsList(organizations);
            LimitListByPageNumber(ORGANIZATIONS, ref limitedOrgs);
            return limitedOrgs;
        }

        private List<Organization> GetLimitedOrgsList(List<Organization> orgsToLimit)
        {
            OrganizationsViewModel orgsViewModelCash = cachService.GetCachedItem<OrganizationsViewModel>(currentUser.Id, ORGANIZATIONS);
            if (!string.IsNullOrEmpty(orgsViewModelCash.SearchName))
                orgsToLimit = orgsToLimit.Where(n => n.Name.ToLower().Contains(orgsViewModelCash.SearchName)).ToList();
            return orgsToLimit;
        }
        #endregion

        #region Attaching Divions
        /// <summary>
        /// Добавляет подразделения к организации, преобразую данные в отображение для выбранной страницы
        /// </summary>
        /// <returns></returns>
        public void AttachDivisions(OrganizationViewModel orgViewModel)
        {
            orgViewModel.Divisions = orgViewModel.GetDivisions(context)
                .MapToViewModels(new DivisionMap(serviceProvider, context), GetLimitedDivisionsList);
        }

        private List<Division> GetLimitedDivisionsList(List<Division> divisions)
        {
            OrganizationViewModel orgViewModelCash = cachService.GetCachedItem<OrganizationViewModel>(currentUser.Id, DIVISIONS);
            List<Division> limitedDivisions = divisions;
            LimitDivByName(orgViewModelCash, ref limitedDivisions);
            LimitDivByParent(orgViewModelCash, divisions, ref limitedDivisions);
            LimitListByPageNumber(DIVISIONS, ref limitedDivisions);
            return limitedDivisions;
        }

        /// <summary>
        /// Ограничение списка подразделений по названию
        /// </summary>
        /// <param name="orgViewModelCash"></param>
        /// <param name="divisionsToLimit"></param>
        private void LimitDivByName(OrganizationViewModel orgViewModelCash, ref List<Division> divisionsToLimit)
        {
            if (!string.IsNullOrEmpty(orgViewModelCash.SearchDivName))
                divisionsToLimit = divisionsToLimit.Where(n => n.Name.ToLower().Contains(orgViewModelCash.SearchDivName)).ToList();
        }

        /// <summary>
        /// Ограничение списка подразделений по названию родительского
        /// </summary>
        /// <param name="orgViewModelCash"></param>
        /// <param name="allDivisions">Список всех подразделений организации</param>
        /// <param name="divisionsToLimit"></param>
        private void LimitDivByParent(OrganizationViewModel orgViewModelCash, List<Division> allDivisions, ref List<Division> divisionsToLimit)
        {
            string searchParentDivName = orgViewModelCash.SearchParentDivName;
            if (!string.IsNullOrEmpty(searchParentDivName))
            {
                TransformCollection(
                    collectionToLimit: ref divisionsToLimit,
                    limitingCollection: allDivisions.Where(n => n.Name.Contains(searchParentDivName)).ToList(),
                    limitCondition: n => n.Name.ToLower().Contains(searchParentDivName),
                    selectCondition: i => i.Id,
                    removeCondition: (parentDivisionIdList, division) => division.ParentDivisionId == null || !parentDivisionIdList.Contains((Guid)division.ParentDivisionId));
            }
        }
        #endregion

        #region Attaching Positions
        /// <summary>
        /// Добавляет должности к организации, преобразую данные в отображение для выбранной страницы
        /// </summary>
        /// <returns></returns>
        public void AttachPositions(OrganizationViewModel orgViewModel)
        {
            orgViewModel.Positions = orgViewModel.GetPositions(context)
                .MapToViewModels(new PositionMap(serviceProvider, context), GetLimitedPositionsList);
        }

        private List<Position> GetLimitedPositionsList(List<Position> positions)
        {
            OrganizationViewModel orgViewModelCash = cachService.GetCachedItem<OrganizationViewModel>(currentUser.Id, POSITIONS);
            List<Position> limitedPositions = positions;
            LimitPosByName(orgViewModelCash, ref limitedPositions);
            LimitPosByDivision(orgViewModelCash, ref limitedPositions);
            LimitPosByPrimaryEmployee(orgViewModelCash, ref limitedPositions);
            LimitPosByParent(orgViewModelCash, ref limitedPositions);
            LimitListByPageNumber(POSITIONS, ref limitedPositions);
            return limitedPositions;
        }

        /// <summary>
        /// Ограничение списка должностей по названию
        /// </summary>
        /// <param name="orgViewModelCash"></param>
        /// <param name="positionsToLimit"></param>
        private void LimitPosByName(OrganizationViewModel orgViewModelCash, ref List<Position> positionsToLimit)
        {
            if (!string.IsNullOrEmpty(orgViewModelCash.SearchPosName))
                positionsToLimit = positionsToLimit.Where(n => n.Name.ToLower().Contains(orgViewModelCash.SearchPosName)).ToList();
        }

        /// <summary>
        /// Ограничение списка должностей по названию подразделения
        /// </summary>
        /// <param name="orgViewModelCash"></param>
        /// <param name="positionsToLimit"></param>
        private void LimitPosByDivision(OrganizationViewModel orgViewModelCash, ref List<Position> positionsToLimit)
        {
            if (!string.IsNullOrEmpty(orgViewModelCash.SeacrhPositionDivName))
            {
                TransformCollection(
                    collectionToLimit: ref positionsToLimit,
                    limitingCollection: context.GetOrgDivisions(orgViewModelCash.Id),
                    limitCondition: n => n.Name.ToLower().Contains(orgViewModelCash.SeacrhPositionDivName),
                    selectCondition: i => i.Id,
                    removeCondition: (divisionIdList, position) => !divisionIdList.Contains(position.DivisionId));
            }
        }

        /// <summary>
        /// Ограничение списка должностей по фио основного сотрудника
        /// </summary>
        /// <param name="orgViewModelCash"></param>
        /// <param name="positionsToLimit"></param>
        private void LimitPosByPrimaryEmployee(OrganizationViewModel orgViewModelCash, ref List<Position> positionsToLimit)
        {
            if (!string.IsNullOrEmpty(orgViewModelCash.SearchPrimaryEmployeeName))
            {
                TransformCollection(
                    collectionToLimit: ref positionsToLimit,
                    limitingCollection: context.GetOrgEmployees(orgViewModelCash.Id),
                    limitCondition: n => n.GetFullName().ToLower().Contains(orgViewModelCash.SearchPrimaryEmployeeName),
                    selectCondition: i => i.Id,
                    removeCondition: (employeeIdList, position) => position.PrimaryEmployeeId == null || !employeeIdList.Contains((Guid)position.PrimaryEmployeeId));
            }
        }

        /// <summary>
        /// Ограничение списка должностей по названию родительской
        /// </summary>
        /// <param name="orgViewModelCash"></param>
        /// <param name="positionsToLimit"></param>
        private void LimitPosByParent(OrganizationViewModel orgViewModelCash, ref List<Position> positionsToLimit)
        {
            if (!string.IsNullOrEmpty(orgViewModelCash.SearchParentPosName))
            {
                TransformCollection(
                    collectionToLimit: ref positionsToLimit,
                    limitingCollection: context.GetOrgPositions(orgViewModelCash.Id),
                    limitCondition: n => n.Name.ToLower().Contains(orgViewModelCash.SearchParentPosName),
                    selectCondition: i => i.Id,
                    removeCondition: (positionIdList, position) => position.ParentPositionId == null || !positionIdList.Contains((Guid)position.ParentPositionId));
            }
        }
        #endregion

        #region Attaching Employees
        /// <summary>
        /// Добавляет сотрудников к организации, преобразую данные в отображение для выбранной страницы
        /// </summary>
        /// <returns></returns>
        public void AttachEmployees(OrganizationViewModel orgViewModel)
        {
            orgViewModel.Employees = orgViewModel.GetEmployees(context)
                .MapToViewModels(new EmployeeMap(serviceProvider, context), GetLimitedEmployeesList);
        }

        private List<Employee> GetLimitedEmployeesList(List<Employee> employees)
        {
            OrganizationViewModel orgViewModelCash = cachService.GetCachedItem<OrganizationViewModel>(currentUser.Id, EMPLOYEES);
            List<Employee> limitedEmployees = employees;
            LimitEmpByName(orgViewModelCash, ref limitedEmployees);
            LimitEmpByPrimaryPosition(orgViewModelCash, ref limitedEmployees);
            LimitEmpByDivision(orgViewModelCash, ref limitedEmployees);
            LimitListByPageNumber(EMPLOYEES, ref limitedEmployees);
            return limitedEmployees;
        }

        /// <summary>
        /// Ограничение списка сотрудников по фио
        /// </summary>
        /// <param name="orgViewModelCash"></param>
        /// <param name="employeesToLimit"></param>
        private void LimitEmpByName(OrganizationViewModel orgViewModelCash, ref List<Employee> employeesToLimit)
        {
            if (!string.IsNullOrEmpty(orgViewModelCash.SearchEmployeeName))
                employeesToLimit = employeesToLimit.Where(n => n.GetFullName().ToLower().Contains(orgViewModelCash.SearchEmployeeName)).ToList();
        }

        /// <summary>
        /// Ограничение списка сотрудников по названию основной должности
        /// </summary>
        /// <param name="orgViewModelCash"></param>
        /// <param name="employeesToLimit"></param>
        private void LimitEmpByPrimaryPosition(OrganizationViewModel orgViewModelCash, ref List<Employee> employeesToLimit)
        {
            if (!string.IsNullOrEmpty(orgViewModelCash.SearchEmployeePrimaryPosName))
            {
                TransformCollection(
                    collectionToLimit: ref employeesToLimit,
                    limitingCollection: context.GetOrgPositions(orgViewModelCash.Id),
                    limitCondition: n => n.Name.ToLower().Contains(orgViewModelCash.SearchEmployeePrimaryPosName),
                    selectCondition: i => i.Id,
                    removeCondition: (positionIdList, employee) => employee.PrimaryPositionId == null || !positionIdList.Contains((Guid)employee.PrimaryPositionId));
            }
        }

        /// <summary>
        /// Ограничение списка сотрудников по названию подразделения
        /// </summary>
        /// <param name="orgViewModelCash"></param>
        /// <param name="employeesToLimit"></param>
        private void LimitEmpByDivision(OrganizationViewModel orgViewModelCash, ref List<Employee> employeesToLimit)
        {
            if (!string.IsNullOrEmpty(orgViewModelCash.SeacrhEmployeeDivName))
            {
                TransformCollection(
                    collectionToLimit: ref employeesToLimit,
                    limitingCollection: context.GetOrgDivisions(orgViewModelCash.Id),
                    limitCondition: n => n.Name.ToLower().Contains(orgViewModelCash.SeacrhEmployeeDivName),
                    selectCondition: i => i.Id,
                    removeCondition: (divisionIdList, employee) => !divisionIdList.Contains(employee.DivisionId));
            }
        }
        #endregion

        #region Attaching Responsibilities
        /// <summary>
        /// Добавляет полномочия к организации, преобразую данные в отображение для выбранной страницы
        /// </summary>
        /// <returns></returns>
        public void AttachResponsibilities(OrganizationViewModel orgViewModel, int pageNumber = DEFAULT_MIN_PAGE_NUMBER)
        {
            SetViewInfo(RESPONSIBILITIES, pageNumber, RESPONSIBILITIES_COUNT);
            orgViewModel.Responsibilities = orgViewModel.GetResponsibilities(context)
                .MapToViewModels(new ResponsibilityMap(serviceProvider, context), GetLimitedResponsibilitiesList);
        }

        private List<Responsibility> GetLimitedResponsibilitiesList(List<Responsibility> responsibilities)
        {
            OrganizationViewModel orgViewModelCash = cachService.GetCachedItem<OrganizationViewModel>(currentUser.Id, RESPONSIBILITIES);
            List<Responsibility> limitedResponsibilities = responsibilities;
            LimitRespByName(orgViewModelCash, ref limitedResponsibilities);
            LimitListByPageNumber(RESPONSIBILITIES, ref limitedResponsibilities);
            return limitedResponsibilities;
        }

        /// <summary>
        /// Ограничение списка полномочий по названию
        /// </summary>
        /// <param name="orgViewModelCash"></param>
        /// <param name="responsibilitiesToLimit"></param>
        private void LimitRespByName(OrganizationViewModel orgViewModelCash, ref List<Responsibility> responsibilitiesToLimit)
        {
            if (!string.IsNullOrEmpty(orgViewModelCash.SeacrhResponsibilityName))
                responsibilitiesToLimit = responsibilitiesToLimit.Where(n => n.Name.ToLower().Contains(orgViewModelCash.SeacrhResponsibilityName)).ToList();
        }

        #endregion

        #region Validations

        /// <summary>
        /// Проверка длины названия организации
        /// </summary>
        /// <param name="orgViewModel"></param>
        private void CheckOrganizationLength(OrganizationViewModel orgViewModel)
        {
            orgViewModel.Name = orgViewModel.Name.TrimStartAndEnd();
            if (string.IsNullOrEmpty(orgViewModel.Name) || orgViewModel.Name.Length < ORGANIZATION_NAME_MIN_LENGTH)
                errors.Add("OrganizationNameLength", resManager.GetString("OrganizationNameLength"));
        }

        /// <summary>
        /// Проверка на наличие организации с таким же названием, где владельцем является текущий пользователь
        /// </summary>
        /// <param name="orgViewModel"></param>
        private void CheckOrganizationNotExists(OrganizationViewModel orgViewModel)
        {
            string orgName = orgViewModel.Name.TrimStartAndEnd().ToLower();
            if (context.Organizations.AsNoTracking().FirstOrDefault(org => org.OwnerId == currentUser.Id && org.Name == orgName) != null)
                errors.Add("OrganizationAlreadyExists", resManager.GetString("OrganizationAlreadyExists"));
        }

        /// <summary>
        /// Метод выполняет проверки при выходе из организации
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        private bool TryLeaveOrgValidate(string orgId)
        {
            Guid organizationId = default;
            UserOrganization userOrganization = default;
            Employee currentEmployee = default;
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (string.IsNullOrEmpty(orgId) || !Guid.TryParse(orgId, out Guid guid))
                        errors.Add("RecordNotFound", resManager.GetString("RecordNotFound"));
                    else organizationId = guid;
                },
                // Поиск органзации, из которой хочет выйти пользователь
                () => {
                    userOrganization = context.UserOrganizations.AsNoTracking()
                        .Include(org => org.Organization)
                        .FirstOrDefault(userOrg => userOrg.UserId == currentUser.Id && userOrg.OrganizationId == organizationId);
                    // Ошибка если не найдена
                    if (userOrganization == null)
                        errors.Add("RecordNotFound", resManager.GetString("RecordNotFound"));
                    else transaction.AddParameter("UserOrganization", userOrganization);
                },
                // Ошибка, если организация принадлежит пользователю
                () => {
                    if (userOrganization.Organization.OwnerId == currentUser.Id)
                        errors.Add("CannotLeaveYourOrg", resManager.GetString("CannotLeaveYourOrg"));
                },
                // Поиск сотрудника в организации
                () => {
                    currentEmployee = context.GetCurrentEmployee(userOrganization.Organization, Guid.Parse(currentUser.Id));
                    // Ошибка если не найден
                    if (currentEmployee == null)
                        errors.Add("RecordNotFound", resManager.GetString("RecordNotFound"));
                    else transaction.AddParameter("CurrentEmployee", currentEmployee);
                }
            });
            return !errors.Any();
        }

        /// <summary>
        /// Метод по строковому ключу пытается получить организацию
        /// </summary>
        /// <param name="organizationId"></param>
        private void CheckOrganizationExists(string organizationId)
        {
            Guid organizationid = default;
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (string.IsNullOrEmpty(organizationId) || !Guid.TryParse(organizationId, out Guid guid))
                        errors.Add("RecordNotFound", resManager.GetString("InviteExpired"));
                    else
                    {
                        organizationid = guid;
                        transaction.AddParameter("Organiztionid", organizationid);
                    }
                },
                () => {
                    UserOrganization userOrganization = context.UserOrganizations.AsNoTracking()
                        .FirstOrDefault(userOrg => userOrg.UserId == currentUser.Id && userOrg.OrganizationId == organizationid);
                    if (userOrganization == null)
                        errors.Add("RecordNotFound", resManager.GetString("InviteExpired"));
                    else transaction.AddParameter("UserOrganization", userOrganization);
                }
            });
        }
        #endregion

        #region Other Methods
        /// <summary>
        /// Метод проверяет, имеет ли сотрудник разрешение на выполнение поданной на вход операции для всех сущностей, относящихся к организации
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public bool CheckPermissionForEmployeeGroup(string actionName, ITransaction transaction)
        {
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
            if (!currentUser.NeedCheckResps(currentOrganization)) return true;
            Employee currentEmployee = context.GetCurrentEmployee(currentOrganization, Guid.Parse(currentUser.Id));
            return currentEmployee != null && currentEmployee.HasPermissionFor(actionName, context);
        }

        /// <summary>
        /// Метод пытается изменить основную организацию пользователя
        /// </summary>
        /// <param name="newPrimaryOrgId">Id новой основной организации</param>
        /// <returns></returns>
        public bool TryChangePrimaryOrg(string newPrimaryOrgId, out Dictionary<string, string> errors)
        {
            errors = this.errors;
            transaction = viewModelsTransactionFactory.Create(currentUser.Id, OperationType.ChangePrimaryOrganization);

            // Проверки
            CheckOrganizationExists(newPrimaryOrgId);
            if (!errors.Any())
            {
                currentUser.PrimaryOrganizationId = (Guid)transaction.GetParameterValue("Organiztionid");
                transaction.AddChange(currentUser, EntityState.Modified);
                if (viewModelsTransactionFactory.TryCommit(transaction, this.errors))
                {
                    viewModelsTransactionFactory.Close(transaction);
                    return true;
                }
            }

            // Закрытие транзакрции и выход
            viewModelsTransactionFactory.Close(transaction, TransactionStatus.Error);
            errors = this.errors;
            return false;
        }

        /// <summary>
        /// Метод пытается исключить пользователя из организации
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public bool TryLeaveOrg(string orgId, out Dictionary<string, string> errors)
        {
            errors = this.errors;
            transaction = viewModelsTransactionFactory.Create(currentUser.Id, OperationType.LeaveOrganization);

            // Вызов всех проверок
            if (TryLeaveOrgValidate(orgId))
            {
                // Добавление организации на удаление и проставление Id основной организации пользователю в default, если организация, из которой выходит пользователь является основной
                UserOrganization userOrganization = (UserOrganization)transaction.GetParameterValue("UserOrganization");
                transaction.AddChange(userOrganization, EntityState.Deleted);
                if (currentUser.PrimaryOrganizationId == userOrganization.OrganizationId)
                {
                    currentUser.PrimaryOrganizationId = Guid.Empty;
                    transaction.AddChange(currentUser, EntityState.Modified);
                }

                // Блокировка сотрудника
                currentEmployee = (Employee)transaction.GetParameterValue("CurrentEmployee");
                currentEmployee.UserId = Guid.Empty;
                currentEmployee.Lock(EmployeeLockReason.EmployeeLeftOrganization);

                // Удаление всех клиентов, где заблокированный сотрудник является единственным менеджером
                new AccountRepository(serviceProvider, context).CheckAccountsForLock(currentEmployee, transaction);
                transaction.AddChange(currentEmployee, EntityState.Modified);

                // Попытка коммита
                if (viewModelsTransactionFactory.TryCommit(transaction, this.errors))
                {
                    viewModelsTransactionFactory.Close(transaction);
                    return true;
                }
            }

            // Закрытие транзакции и выход
            errors = this.errors;
            viewModelsTransactionFactory.Close(transaction, TransactionStatus.Error);
            return false;
        }

        /// <summary>
        /// Метод пытается принять сотруднкиа в организацию по приглашению и возвращает ошибки в случае неудачи
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public bool TryAcceptInvite(string orgId, out Dictionary<string, string> errors)
        {
            errors = this.errors;
            transaction = viewModelsTransactionFactory.Create(currentUser.Id, OperationType.AcceptInvite);

            // Удаление уведомления в любом случае
            RemoveOrgIniteNot();

            // Вызов всех проверок
            CheckOrganizationExists(orgId);
            if (!errors.Any())
            {
                // Проставление флага "Accepted" в "true"
                UserOrganization userOrganization = (UserOrganization)transaction.GetParameterValue("UserOrganization");
                userOrganization.Accepted = true;
                transaction.AddChange(userOrganization, EntityState.Modified);

                // Проставление статуса сотрудника в "Active"
                currentEmployee = context.GetCurrentEmployee(userOrganization.OrganizationId, Guid.Parse(currentUser.Id));
                currentEmployee.EmployeeStatus = EmployeeStatus.Active;
                transaction.AddChange(currentEmployee, EntityState.Modified);

                // Добавление настроек уведомлений для этой организации
                CreateOrgNotificationsSetting(transaction);

                // Попытка коммита
                if (viewModelsTransactionFactory.TryCommit(transaction, this.errors))
                {
                    viewModelsTransactionFactory.Close(transaction);
                    return true;
                }
            }

            // Закрытие транзакции и выход
            errors = this.errors;
            viewModelsTransactionFactory.Close(transaction, TransactionStatus.Error);
            return false;
        }

        /// <summary>
        /// Метод отклоняет приглашение в организацию
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public void RejectInvite(string orgId)
        {
            transaction = viewModelsTransactionFactory.Create(currentUser.Id, OperationType.AcceptInvite);

            // Вызов всех проверок
            CheckOrganizationExists(orgId);
            if (!errors.Any())
            {
                UserOrganization userOrganization = (UserOrganization)transaction.GetParameterValue("UserOrganization");
                currentEmployee = context.GetCurrentEmployee(userOrganization.OrganizationId, Guid.Parse(currentUser.Id));
                currentEmployee.UserId = Guid.Empty;
                currentEmployee.Lock(EmployeeLockReason.RejectInvite);
                transaction.AddChange(currentEmployee, EntityState.Modified);
                transaction.AddChange(userOrganization, EntityState.Deleted);
            }

            // Удаление уведомления
            RemoveOrgIniteNot();

            // Попытка сделать коммит и закрытие транзакции
            viewModelsTransactionFactory.TryCommit(transaction, errors);
            viewModelsTransactionFactory.Close(transaction, TransactionStatus.Success);
        }

        /// <summary>
        /// Метод удаляет уведомление о приглашении в организацию 
        /// </summary>
        private void RemoveOrgIniteNot()
        {
            // Удаление уведомления, если оно было присланов в Инбоксы
            Func<UserNotification, bool> predicate = userNot => userNot.Notification.NotificationType == NotificationType.OrgInvite;
            context.GetUserNotificationsExt(currentUser).Where(predicate).ToList().ForEach(userNot =>
            {
                if (userNot.Notification is InboxNotification inboxNotification)
                {
                    if (new InboxNotificationRepository(serviceProvider, context).TryDelete(inboxNotification))
                        new UserNotificationRepository(serviceProvider, context).OnUserNotRemoved(userNot);
                }
            });
        }

        /// <summary>
        /// Метод добавляет настройки уведомления для новой организации пользователя
        /// </summary>
        public void CreateOrgNotificationsSetting(ITransaction transaction)
        {
            UserOrganization userOrganization = (UserOrganization)transaction.GetParameterValue("UserOrganization");
            OrgNotificationsSetting orgNotificationsSetting = new OrgNotificationsSetting()
            {
                Id = Guid.NewGuid(),
                UserOrganization = userOrganization,
                UserOrganizationId = userOrganization.Id
            };
            orgNotificationsSetting = new OrgNotificationsSettingMap(serviceProvider, context).InitNotSetting(orgNotificationsSetting);
            transaction.AddChange(orgNotificationsSetting, EntityState.Added);
        }

        /// <summary>
        /// Метод проверяет, имеет ли пользователь право на просмотр любього относящегося к организации элемента(должность, сотрудник и т.д.)
        /// </summary>
        /// <returns></returns>
        public bool HasPermissionsForSeeOrgItem()
        {
            Organization organization = cachService.GetCachedItem<Organization>(currentUser.Id, $"{PC}Organization");
            List<UserOrganization> userOrganizations = cachService.GetCachedItems<UserOrganization>(currentUser.Id, $"{PC}UserOrganizations");
            return userOrganizations.Where(userOrg => userOrg.Accepted).Select(userOrg => userOrg.OrganizationId).Contains(organization.Id);
        }
        #endregion
    }
}
