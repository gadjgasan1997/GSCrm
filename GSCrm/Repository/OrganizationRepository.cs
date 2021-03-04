using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using GSCrm.Data;
using GSCrm.Mapping;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using GSCrm.Transactions;
using GSCrm.Models.Enums;
using GSCrm.Notifications;
using static GSCrm.CommonConsts;
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Repository
{
    public class OrganizationRepository : BaseRepository<Organization, OrganizationViewModel>
    {
        #region Declarations
        private const int ORGANIZATION_NAME_MIN_LENGTH = 3;
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
            cachService.AddOrUpdate(currentUser, $"{PC}Organization", organization);
            List<UserOrganization> userOrganizations = context.UserOrganizations.Where(userOrg => userOrg.UserId == currentUser.Id).ToList();
            cachService.AddOrUpdate(currentUser, $"{PC}UserOrganizations", userOrganizations);
            return userOrganizations.Select(userOrg => userOrg.OrganizationId).Contains(organization.Id);
        }

        protected override bool RespsIsCorrectOnCreate(OrganizationViewModel entityToCreate) => true;

        protected override bool TryCreatePrepare(OrganizationViewModel orgViewModel)
        {
            orgViewModel.Normalize();
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CheckOrganizationLength(orgViewModel),
                () => CheckOrganizationNotExists(orgViewModel)
            });
            return !errors.Any();
        }

        protected override bool RespsIsCorrectOnDelete(Organization organization) => organization.OwnerId == currentUser.Id;

        protected override void HasNotPermissionsForDelete()
            => errors.Add("AnotherOrgIsReadonly", resManager.GetString("AnotherOrgIsReadonly").Replace("&OrgName", RecordToRemove.Name));

        public OrganizationsViewModel LoadOrganizationsView()
        {
            OrganizationsViewModel orgsViewModel = cachService.GetCachedCurrentEntity<OrganizationsViewModel>(currentUser);
            AttachOrganizations(orgsViewModel);
            return orgsViewModel;
        }

        public override OrganizationViewModel LoadView(Organization organization)
        {
            OrganizationViewModel orgViewModel = cachService.GetCachedCurrentEntity<OrganizationViewModel>(currentUser);

            // Прикрепление всех сущностей
            AttachDivisions(orgViewModel);
            AttachPositions(orgViewModel);
            AttachEmployees(orgViewModel);
            AttachResponsibilities(orgViewModel);

            // Кеширование модели
            cachService.SetCurrentView(currentUser.Id, ORGANIZATION);
            cachService.CacheEntity(currentUser, orgViewModel);
            cachService.CacheCurrentEntity(currentUser, orgViewModel);
            return orgViewModel;
        }
        #endregion

        #region Searching
        public void Search(OrganizationsViewModel organizationsViewModel)
        {
            organizationsViewModel.NormalizeSearch();
            OrganizationsViewModel cachedViewModel = cachService.GetCachedCurrentEntity<OrganizationsViewModel>(currentUser);
            cachedViewModel.SearchName = organizationsViewModel.SearchName;
        }

        public void ClearSearch()
        {
            OrganizationsViewModel cachedViewModel = cachService.GetCachedCurrentEntity<OrganizationsViewModel>(currentUser);
            cachedViewModel.SearchName = default;
        }

        public void SearchDivision(OrganizationViewModel orgViewModel)
        {
            orgViewModel.NormalizeSearch();
            OrganizationViewModel cachedViewModel = cachService.GetCachedCurrentEntity<OrganizationViewModel>(currentUser);
            cachedViewModel.SearchDivName = orgViewModel.SearchDivName;
            cachedViewModel.SearchParentDivName = orgViewModel.SearchParentDivName;
        }

        public void ClearDivisionSearch()
        {
            OrganizationViewModel cachedViewModel = cachService.GetCachedCurrentEntity<OrganizationViewModel>(currentUser);
            cachedViewModel.SearchDivName = default;
            cachedViewModel.SearchParentDivName = default;
        }

        public void SearchPosition(OrganizationViewModel orgViewModel)
        {
            orgViewModel.NormalizeSearch();
            OrganizationViewModel cachedViewModel = cachService.GetCachedCurrentEntity<OrganizationViewModel>(currentUser);
            cachedViewModel.SearchPosName = orgViewModel.SearchPosName;
            cachedViewModel.SeacrhPositionDivName = orgViewModel.SeacrhPositionDivName;
            cachedViewModel.SearchParentPosName = orgViewModel.SearchParentPosName;
            cachedViewModel.SearchPrimaryEmployeeName = orgViewModel.SearchPrimaryEmployeeName;
        }

        public void ClearPositionSearch()
        {
            OrganizationViewModel cachedViewModel = cachService.GetCachedCurrentEntity<OrganizationViewModel>(currentUser);
            cachedViewModel.SearchPosName = default;
            cachedViewModel.SeacrhPositionDivName = default;
            cachedViewModel.SearchPrimaryEmployeeName = default;
            cachedViewModel.SearchParentPosName = default;
        }

        public void SearchEmployee(OrganizationViewModel orgViewModel)
        {
            orgViewModel.NormalizeSearch();
            OrganizationViewModel cachedViewModel = cachService.GetCachedCurrentEntity<OrganizationViewModel>(currentUser);
            cachedViewModel.SearchEmployeeName = orgViewModel.SearchEmployeeName;
            cachedViewModel.SeacrhEmployeeDivName = orgViewModel.SeacrhEmployeeDivName;
            cachedViewModel.SearchEmployeePrimaryPosName = orgViewModel.SearchEmployeePrimaryPosName;
        }

        public void ClearEmployeeSearch()
        {
            OrganizationViewModel cachedViewModel = cachService.GetCachedCurrentEntity<OrganizationViewModel>(currentUser);
            cachedViewModel.SearchEmployeeName = default;
            cachedViewModel.SearchEmployeePrimaryPosName = default;
            cachedViewModel.SeacrhEmployeeDivName = default;
        }

        public void SearchResponsibility(OrganizationViewModel orgViewModel)
        {
            orgViewModel.NormalizeSearch();
            OrganizationViewModel cachedViewModel = cachService.GetCachedCurrentEntity<OrganizationViewModel>(currentUser);
            cachedViewModel.SeacrhResponsibilityName = orgViewModel.SeacrhResponsibilityName;
        }

        public void ClearResponsibilitySearch()
        {
            OrganizationViewModel cachedViewModel = cachService.GetCachedCurrentEntity<OrganizationViewModel>(currentUser);
            cachedViewModel.SeacrhResponsibilityName = default;
        }
        #endregion

        #region Attaching Organizations
        /// <summary>
        /// Метод добавляет список моделей представления организаций
        /// </summary>
        /// <param name="orgsViewModel"></param>
        public void AttachOrganizations(OrganizationsViewModel orgsViewModel)
            => orgsViewModel.OrganizationViewModels = context.GetOrganizations(currentUser)
                .MapToViewModels(orgsViewModel, new OrganizationMap(serviceProvider, context), (orgsViewModel, organizations) =>
                    GetLimitedList(organizations, orgsViewModel));

        /// <summary>
        /// Метод ограничивает список организаций по существующему фильтру
        /// </summary>
        /// <param name="organizations"></param>
        /// <returns></returns>
        private List<Organization> GetLimitedList(List<Organization> organizations, OrganizationsViewModel orgsViewModel)
        {
            List<Organization> limitedOrgs = GetLimitedOrgsList(organizations, orgsViewModel);
            LimitViewItemsByPageNumber(ORGANIZATIONS, ref limitedOrgs);
            return limitedOrgs;
        }

        private List<Organization> GetLimitedOrgsList(List<Organization> orgsToLimit, OrganizationsViewModel orgsViewModel)
        {
            if (!string.IsNullOrEmpty(orgsViewModel.SearchName))
                orgsToLimit = orgsToLimit.Where(n => n.Name.ToLower().Contains(orgsViewModel.SearchName)).ToList();
            return orgsToLimit;
        }
        #endregion

        #region Attaching Divions
        /// <summary>
        /// Добавляет подразделения к организации, преобразую данные в отображение для выбранной страницы
        /// </summary>
        /// <returns></returns>
        public void AttachDivisions(OrganizationViewModel orgViewModel)
            => orgViewModel.Divisions = orgViewModel.GetDivisions(context)
                .MapToViewModels(orgViewModel, new DivisionMap(serviceProvider, context), (orgViewModel, divisions) =>
                    GetLimitedDivisionsList(orgViewModel, divisions));

        private List<Division> GetLimitedDivisionsList(OrganizationViewModel orgViewModel, List<Division> divisions)
        {
            List<Division> limitedDivisions = divisions;
            LimitDivByName(orgViewModel, ref limitedDivisions);
            LimitDivByParent(orgViewModel, divisions, ref limitedDivisions);
            LimitViewItemsByPageNumber(orgViewModel.Id, DIVISIONS, ref limitedDivisions);
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
            => orgViewModel.Positions = orgViewModel.GetAllPositions(context)
                .MapToViewModels(orgViewModel, new PositionMap(serviceProvider, context), (orgViewModel, positions) =>
                    GetLimitedPositionsList(orgViewModel, positions));

        private List<Position> GetLimitedPositionsList(OrganizationViewModel orgViewModel, List<Position> positions)
        {
            List<Position> limitedPositions = positions;
            LimitPosByName(orgViewModel, ref limitedPositions);
            LimitPosByDivision(orgViewModel, ref limitedPositions);
            LimitPosByPrimaryEmployee(orgViewModel, ref limitedPositions);
            LimitPosByParent(orgViewModel, ref limitedPositions);
            LimitViewItemsByPageNumber(orgViewModel.Id, POSITIONS, ref limitedPositions);
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
                    removeCondition: (divisionIdList, position) => !divisionIdList.Contains((Guid)position.DivisionId));
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
            => orgViewModel.Employees = orgViewModel.GetAllEmployees(context)
                .MapToViewModels(orgViewModel, new EmployeeMap(serviceProvider, context), (orgViewModel, employees) =>
                    GetLimitedEmployeesList(orgViewModel, employees));

        private List<Employee> GetLimitedEmployeesList(OrganizationViewModel orgViewModel, List<Employee> employees)
        {
            List<Employee> limitedEmployees = employees;
            LimitEmpByName(orgViewModel, ref limitedEmployees);
            LimitEmpByPrimaryPosition(orgViewModel, ref limitedEmployees);
            LimitEmpByDivision(orgViewModel, ref limitedEmployees);
            LimitViewItemsByPageNumber(orgViewModel.Id, EMPLOYEES, ref limitedEmployees);
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
                    removeCondition: (divisionIdList, employee) => !divisionIdList.Contains((Guid)employee.DivisionId));
            }
        }
        #endregion

        #region Attaching Responsibilities
        /// <summary>
        /// Добавляет полномочия к организации, преобразую данные в отображение для выбранной страницы
        /// </summary>
        /// <returns></returns>
        public void AttachResponsibilities(OrganizationViewModel orgViewModel)
            => orgViewModel.Responsibilities = orgViewModel.GetResponsibilities(context)
                .MapToViewModels(orgViewModel, new ResponsibilityMap(serviceProvider, context), (orgViewModel, responsibilities) =>
                    GetLimitedResponsibilitiesList(orgViewModel, responsibilities));

        private List<Responsibility> GetLimitedResponsibilitiesList(OrganizationViewModel orgViewModel, List<Responsibility> responsibilities)
        {
            List<Responsibility> limitedResponsibilities = responsibilities;
            LimitRespByName(orgViewModel, ref limitedResponsibilities);
            LimitViewItemsByPageNumber(orgViewModel.Id, RESPONSIBILITIES, ref limitedResponsibilities);
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
            orgViewModel.Name = orgViewModel.Name;
            if (string.IsNullOrEmpty(orgViewModel.Name) || orgViewModel.Name.Length < ORGANIZATION_NAME_MIN_LENGTH)
                errors.Add("OrganizationNameLength", resManager.GetString("OrganizationNameLength"));
        }

        /// <summary>
        /// Проверка на наличие организации с таким же названием, где владельцем является текущий пользователь
        /// </summary>
        /// <param name="orgViewModel"></param>
        private void CheckOrganizationNotExists(OrganizationViewModel orgViewModel)
        {
            string orgName = orgViewModel.Name;
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
        /// <returns></returns>
        public bool CheckPermissionForOrgGroup(string actionName)
        {
            Organization currentOrganization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
            if (currentOrganization != null)
            {
                if (!currentUser.NeedCheckResps(currentOrganization)) return true;
                Employee currentEmployee = context.GetCurrentEmployee(currentOrganization, Guid.Parse(currentUser.Id));
                return currentEmployee != null && currentEmployee.HasPermissionFor(actionName, context);
            }
            return false;
        }

        /// <summary>
        /// Метод пытается изменить основную организацию пользователя
        /// </summary>
        /// <param name="newPrimaryOrgId">Id новой основной организации</param>
        /// <returns></returns>
        public bool TryChangePrimaryOrg(string newPrimaryOrgId, out Dictionary<string, string> errors)
        {
            errors = this.errors;
            transaction = viewModelsTF.Create(currentUser.Id, OperationType.ChangePrimaryOrganization);

            // Проверки
            CheckOrganizationExists(newPrimaryOrgId);
            if (!errors.Any())
            {
                currentUser.PrimaryOrganizationId = (Guid)transaction.GetParameterValue("Organiztionid");
                transaction.AddChange(currentUser, EntityState.Modified);
                if (viewModelsTF.TryCommit(transaction, this.errors))
                {
                    viewModelsTF.Close(transaction);
                    return true;
                }
            }

            // Закрытие транзакрции и выход
            viewModelsTF.Close(transaction, TransactionStatus.Error);
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
            transaction = viewModelsTF.Create(currentUser.Id, OperationType.LeaveOrganization);

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

                // Если сотрудник в статусе активного, необходимо его заблокировать
                Employee currentEmployee = (Employee)transaction.GetParameterValue("CurrentEmployee");
                if (currentEmployee.EmployeeStatus == EmployeeStatus.Active)
                {
                    // Блокировка сотрудника
                    currentEmployee.UserId = Guid.Empty;
                    currentEmployee.Lock(EmployeeLockReason.EmployeeLeftOrganization);

                    // Удаление всех клиентов, где заблокированный сотрудник является единственным менеджером
                    new AccountRepository(serviceProvider, context).CheckAccountsForLock(currentEmployee, transaction);
                    transaction.AddChange(currentEmployee, EntityState.Modified);
                }
                // Иначе, если он уже был заблокирован, то в причину блокировки устанавливается значение "LockedEmployeeLeftOrg"
                else if (currentEmployee.EmployeeStatus == EmployeeStatus.Lock)
                {
                    // Блокировка сотрудника
                    currentEmployee = (Employee)transaction.GetParameterValue("CurrentEmployee");
                    currentEmployee.UserId = Guid.Empty;
                    currentEmployee.Lock(EmployeeLockReason.LockedEmployeeLeftOrg);
                    transaction.AddChange(currentEmployee, EntityState.Modified);
                }

                // Попытка коммита
                if (viewModelsTF.TryCommit(transaction, this.errors))
                {
                    viewModelsTF.Close(transaction);
                    return true;
                }
            }

            // Закрытие транзакции и выход
            errors = this.errors;
            viewModelsTF.Close(transaction, TransactionStatus.Error);
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
            transaction = viewModelsTF.Create(currentUser.Id, OperationType.AcceptInvite);

            // Вызов всех проверок
            CheckOrganizationExists(orgId);
            if (!errors.Any())
            {
                // Проставление флага "Accepted" в "true"
                UserOrganization userOrganization = (UserOrganization)transaction.GetParameterValue("UserOrganization");
                userOrganization.Accepted = true;
                transaction.AddChange(userOrganization, EntityState.Modified);

                // Проставление статуса сотрудника в "Active"
                Employee currentEmployee = context.GetCurrentEmployee(userOrganization.OrganizationId, Guid.Parse(currentUser.Id));
                currentEmployee.EmployeeStatus = EmployeeStatus.Active;
                transaction.AddChange(currentEmployee, EntityState.Modified);

                // Добавление настроек уведомлений для этой организации
                CreateOrgNotificationsSetting(transaction);

                // Попытка коммита
                if (viewModelsTF.TryCommit(transaction, this.errors))
                {
                    // Удаление уведомления в случае успеха
                    RemoveOrgIniteNot();
                    viewModelsTF.Close(transaction);
                    return true;
                }
            }

            // Закрытие транзакции и выход
            errors = this.errors;
            viewModelsTF.Close(transaction, TransactionStatus.Error);
            return false;
        }

        /// <summary>
        /// Метод отклоняет приглашение в организацию
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public void RejectInvite(string orgId)
        {
            transaction = viewModelsTF.Create(currentUser.Id, OperationType.AcceptInvite);

            // Вызов всех проверок
            CheckOrganizationExists(orgId);
            if (!errors.Any())
            {
                UserOrganization userOrganization = (UserOrganization)transaction.GetParameterValue("UserOrganization");
                Employee currentEmployee = context.GetCurrentEmployee(userOrganization.OrganizationId, Guid.Parse(currentUser.Id));
                currentEmployee.UserId = Guid.Empty;
                currentEmployee.Lock(EmployeeLockReason.RejectInvite);
                transaction.AddChange(currentEmployee, EntityState.Modified);
                transaction.AddChange(userOrganization, EntityState.Deleted);
            }

            // Удаление уведомления
            RemoveOrgIniteNot();

            // Попытка сделать коммит и закрытие транзакции
            viewModelsTF.TryCommit(transaction, errors);
            viewModelsTF.Close(transaction, TransactionStatus.Success);
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
        /// Метод проверяет, имеет ли пользователь право на просмотр любого относящегося к организации элемента(должность, сотрудник и т.д.)
        /// В отличие от метода <see cref="HasPermissionsForSeeItem(Organization)"/> проверяет, что организация не просто есть в списке организаций пользователя,
        /// но и то, что пользователь принял в нее приглашение
        /// </summary>
        /// <returns></returns>
        public bool HasPermissionsForSeeOrgItem()
        {
            if (cachService.TryGetValue(currentUser, $"{PC}Organization", out object orgValue) && orgValue is Organization organization &&
                cachService.TryGetValue(currentUser, $"{PC}UserOrganizations", out object userOrgsValue) && userOrgsValue is List<UserOrganization> userOrganizations)
            {
                return userOrganizations.Where(userOrg => userOrg.Accepted).Select(userOrg => userOrg.OrganizationId).Contains(organization.Id);
            }
            return false;
        }
        #endregion
    }
}
