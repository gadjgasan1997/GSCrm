using GSCrm.Data;
using GSCrm.Data.ApplicationInfo;
using GSCrm.DataTransformers;
using GSCrm.Helpers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using static GSCrm.CommonConsts;
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Repository
{
    public class OrganizationRepository : GenericRepository<Organization, OrganizationViewModel, OrganizationValidatior, OrganizationTransformer>
    {
        private readonly User currentUser;
        public static OrganizationViewModel CurrentOrganization { get; set; }
        public OrganizationRepository(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager, HttpContext httpContext = null)
            : base(context, viewsInfo, resManager, new OrganizationValidatior(context, resManager), new OrganizationTransformer(context, resManager, httpContext))
        {
            if (httpContext != null)
                currentUser = httpContext.GetCurrentUser(context);
        }

        #region Searching
        /// <summary>
        /// Метод устанавливает значения для поиска по организациям
        /// </summary>
        /// <param name="orgsViewModel"></param>
        /// <returns></returns>
        public void Search(OrganizationsViewModel orgsViewModel)
        {
            //viewsInfo.Reset(ORGANIZATIONS);
            OrganizationsViewModel orgsViewModelCash = ModelCash<OrganizationsViewModel>.GetViewModel(currentUser.Id, ORGANIZATIONS);
            orgsViewModelCash.SearchNameCash.AddOrReplace(currentUser.Id, orgsViewModel.SearchName?.ToLower().TrimStartAndEnd());
            ModelCash<OrganizationsViewModel>.SetViewModel(currentUser.Id, ORGANIZATIONS, orgsViewModelCash);
        }

        /// <summary>
        /// Метод очищает поиск по организациям
        /// </summary>
        public void ClearSearch()
        {
            OrganizationsViewModel orgsViewModelCash = ModelCash<OrganizationsViewModel>.GetViewModel(currentUser.Id, ORGANIZATIONS);
            orgsViewModelCash.SearchNameCash.AddOrReplace(currentUser.Id, default);
            ModelCash<OrganizationsViewModel>.SetViewModel(currentUser.Id, ORGANIZATIONS, orgsViewModelCash);
        }

        /// <summary>
        /// Метод для поиска подразделения
        /// </summary>
        /// <param name="orgViewModel"></param>
        public void SearchDivision(OrganizationViewModel orgViewModel)
        {
            //viewsInfo.Reset(DIVISIONS);
            OrganizationViewModel orgViewModelCash = ModelCash<OrganizationViewModel>.GetViewModel(currentUser.Id, DIVISIONS);
            orgViewModelCash.IdCash.AddOrReplace(currentUser.Id, orgViewModel.Id);
            orgViewModelCash.SearchDivNameCash.AddOrReplace(currentUser.Id, orgViewModel.SearchDivName?.ToLower().TrimStartAndEnd());
            orgViewModelCash.SearchParentDivNameCash.AddOrReplace(currentUser.Id, orgViewModel.SearchParentDivName?.ToLower().TrimStartAndEnd());
            ModelCash<OrganizationViewModel>.SetViewModel(currentUser.Id, DIVISIONS, orgViewModelCash);
        }

        /// <summary>
        /// Метод очищает кеш для модели с подразделениями
        /// </summary>
        public void ClearDivisionSearch()
        {
            OrganizationViewModel orgViewModelCash = ModelCash<OrganizationViewModel>.GetViewModel(currentUser.Id, DIVISIONS);
            orgViewModelCash.SearchDivNameCash.AddOrReplace(currentUser.Id, default);
            orgViewModelCash.SearchParentDivNameCash.AddOrReplace(currentUser.Id, default);
            ModelCash<OrganizationViewModel>.SetViewModel(currentUser.Id, DIVISIONS, orgViewModelCash);
        }

        /// <summary>
        /// Метод для поиска должности
        /// </summary>
        /// <param name="orgViewModel"></param>
        public void SearchPosition(OrganizationViewModel orgViewModel)
        {
            //viewsInfo.Reset(POSITIONS);
            OrganizationViewModel orgViewModelCash = ModelCash<OrganizationViewModel>.GetViewModel(currentUser.Id, POSITIONS);
            orgViewModelCash.IdCash.AddOrReplace(currentUser.Id, orgViewModel.Id);
            orgViewModelCash.SearchPosNameCash.AddOrReplace(currentUser.Id, orgViewModel.SearchPosName?.ToLower().TrimStartAndEnd());
            orgViewModelCash.SeacrhPositionDivNameCash.AddOrReplace(currentUser.Id, orgViewModel.SeacrhPositionDivName?.ToLower().TrimStartAndEnd());
            orgViewModelCash.SearchPrimaryEmployeeNameCash.AddOrReplace(currentUser.Id, orgViewModel.SearchPrimaryEmployeeName?.ToLower().TrimStartAndEnd());
            orgViewModelCash.SearchParentPosNameCash.AddOrReplace(currentUser.Id, orgViewModel.SearchParentPosName?.ToLower().TrimStartAndEnd());
            ModelCash<OrganizationViewModel>.SetViewModel(currentUser.Id, POSITIONS, orgViewModelCash);
        }

        /// <summary>
        /// Метод очищает кеш для модели с должностями
        /// </summary>
        public void ClearPositionSearch()
        {
            OrganizationViewModel orgViewModelCash = ModelCash<OrganizationViewModel>.GetViewModel(currentUser.Id, POSITIONS);
            orgViewModelCash.SearchPosNameCash.AddOrReplace(currentUser.Id, default);
            orgViewModelCash.SeacrhPositionDivNameCash.AddOrReplace(currentUser.Id, default);
            orgViewModelCash.SearchPrimaryEmployeeNameCash.AddOrReplace(currentUser.Id, default);
            orgViewModelCash.SearchParentPosNameCash.AddOrReplace(currentUser.Id, default);
            ModelCash<OrganizationViewModel>.SetViewModel(currentUser.Id, POSITIONS, orgViewModelCash);
        }

        /// <summary>
        /// Метод для поиска сотрудника
        /// </summary>
        /// <param name="orgViewModel"></param>
        public void SearchEmployee(OrganizationViewModel orgViewModel)
        {
            //viewsInfo.Reset(EMPLOYEES);
            OrganizationViewModel orgViewModelCash = ModelCash<OrganizationViewModel>.GetViewModel(currentUser.Id, EMPLOYEES);
            orgViewModelCash.IdCash.AddOrReplace(currentUser.Id, orgViewModel.Id);
            orgViewModelCash.SearchEmployeeNameCash.AddOrReplace(currentUser.Id, orgViewModel.SearchEmployeeName?.ToLower().TrimStartAndEnd());
            orgViewModelCash.SearchEmployeePrimaryPosNameCash.AddOrReplace(currentUser.Id, orgViewModel.SearchEmployeePrimaryPosName?.ToLower().TrimStartAndEnd());
            orgViewModelCash.SeacrhEmployeeDivNameCash.AddOrReplace(currentUser.Id, orgViewModel.SeacrhEmployeeDivName?.ToLower().TrimStartAndEnd());
            ModelCash<OrganizationViewModel>.SetViewModel(currentUser.Id, EMPLOYEES, orgViewModelCash);
        }

        /// <summary>
        /// Метод очищает кеш для модели с сотрудниками
        /// </summary>
        public void ClearEmployeeSearch()
        {
            OrganizationViewModel orgViewModelCash = ModelCash<OrganizationViewModel>.GetViewModel(currentUser.Id, EMPLOYEES);
            orgViewModelCash.SearchEmployeeNameCash.AddOrReplace(currentUser.Id, default);
            orgViewModelCash.SearchEmployeePrimaryPosNameCash.AddOrReplace(currentUser.Id, default);
            orgViewModelCash.SeacrhEmployeeDivNameCash.AddOrReplace(currentUser.Id, default);
            ModelCash<OrganizationViewModel>.SetViewModel(currentUser.Id, EMPLOYEES, orgViewModelCash);
        }

        /// <summary>
        /// Метод для поиска полномочия
        /// </summary>
        /// <param name="orgViewModel"></param>
        public void SearchResponsibility(OrganizationViewModel orgViewModel)
        {
            //viewsInfo.Reset(RESPONSIBILITIES);
            OrganizationViewModel orgViewModelCash = ModelCash<OrganizationViewModel>.GetViewModel(currentUser.Id, RESPONSIBILITIES);
            orgViewModelCash.IdCash.AddOrReplace(currentUser.Id, orgViewModel.Id);
            orgViewModelCash.SeacrhResponsibilityNameCash.AddOrReplace(currentUser.Id, orgViewModel.SeacrhResponsibilityName?.ToLower().TrimStartAndEnd());
            ModelCash<OrganizationViewModel>.SetViewModel(currentUser.Id, RESPONSIBILITIES, orgViewModelCash);
        }

        /// <summary>
        /// Метод очищает кеш для модели с полномочиями
        /// </summary>
        public void ClearResponsibilitySearch()
        {
            OrganizationViewModel orgViewModelCash = ModelCash<OrganizationViewModel>.GetViewModel(currentUser.Id, RESPONSIBILITIES);
            orgViewModelCash.SeacrhResponsibilityNameCash.AddOrReplace(currentUser.Id, default);
            ModelCash<OrganizationViewModel>.SetViewModel(currentUser.Id, RESPONSIBILITIES, orgViewModelCash);
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
                .TransformToViewModels<Organization, OrganizationViewModel, OrganizationTransformer>(
                    transformer: new OrganizationTransformer(context, resManager),
                    limitingFunc: GetLimitedList);
        }

        /// <summary>
        /// Метод ограничивает список организаций по существующему фильтру
        /// </summary>
        /// <param name="organizations"></param>
        /// <returns></returns>
        private List<Organization> GetLimitedList(List<Organization> organizations)
        {
            List<Organization> limitedOrgs = GetLimitedOrgsList(organizations);
            LimitListByPageNumber(currentUser.Id, ORGANIZATIONS, ref limitedOrgs);
            return limitedOrgs;
        }

        private List<Organization> GetLimitedOrgsList(List<Organization> orgsToLimit)
        {
            OrganizationsViewModel orgsViewModelCash = ModelCash<OrganizationsViewModel>.GetViewModel(currentUser.Id, ORGANIZATIONS);
            string searchName = orgsViewModelCash.SearchNameCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchName))
                orgsToLimit = orgsToLimit.Where(n => n.Name.ToLower().Contains(searchName)).ToList();
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
                .TransformToViewModels<Division, DivisionViewModel, DivisionTransformer>(
                    transformer: new DivisionTransformer(context, resManager),
                    limitingFunc: GetLimitedDivisionsList);
        }

        private List<Division> GetLimitedDivisionsList(List<Division> divisions)
        {
            OrganizationViewModel orgViewModelCash = ModelCash<OrganizationViewModel>.GetViewModel(currentUser.Id, DIVISIONS);
            List<Division> limitedDivisions = divisions;
            LimitDivByName(orgViewModelCash, ref limitedDivisions);
            LimitDivByParent(orgViewModelCash, divisions, ref limitedDivisions);
            LimitListByPageNumber(currentUser.Id, DIVISIONS, ref limitedDivisions);
            return limitedDivisions;
        }

        /// <summary>
        /// Ограничение списка подразделений по названию
        /// </summary>
        /// <param name="orgViewModelCash"></param>
        /// <param name="divisionsToLimit"></param>
        private void LimitDivByName(OrganizationViewModel orgViewModelCash, ref List<Division> divisionsToLimit)
        {
            string searchDivName = orgViewModelCash.SearchDivNameCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchDivName))
                divisionsToLimit = divisionsToLimit.Where(n => n.Name.ToLower().Contains(searchDivName)).ToList();
        }

        /// <summary>
        /// Ограничение списка подразделений по названию родительского
        /// </summary>
        /// <param name="orgViewModelCash"></param>
        /// <param name="allDivisions">Список всех подразделений организации</param>
        /// <param name="divisionsToLimit"></param>
        private void LimitDivByParent(OrganizationViewModel orgViewModelCash, List<Division> allDivisions, ref List<Division> divisionsToLimit)
        {
            string searchParentDivName = orgViewModelCash.SearchParentDivNameCash.GetValueOrDefault(currentUser.Id);
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
                .TransformToViewModels<Position, PositionViewModel, PositionTransformer>(
                    transformer: new PositionTransformer(context, resManager),
                    limitingFunc: GetLimitedPositionsList);
        }

        private List<Position> GetLimitedPositionsList(List<Position> positions)
        {
            OrganizationViewModel orgViewModelCash = ModelCash<OrganizationViewModel>.GetViewModel(currentUser.Id, POSITIONS);
            List<Position> limitedPositions = positions;
            LimitPosByName(orgViewModelCash, ref limitedPositions);
            LimitPosByDivision(orgViewModelCash, ref limitedPositions);
            LimitPosByPrimaryEmployee(orgViewModelCash, ref limitedPositions);
            LimitPosByParent(orgViewModelCash, ref limitedPositions);
            LimitListByPageNumber(currentUser.Id, POSITIONS, ref limitedPositions);
            return limitedPositions;
        }

        /// <summary>
        /// Ограничение списка должностей по названию
        /// </summary>
        /// <param name="orgViewModelCash"></param>
        /// <param name="positionsToLimit"></param>
        private void LimitPosByName(OrganizationViewModel orgViewModelCash, ref List<Position> positionsToLimit)
        {
            string searchPosName = orgViewModelCash.SearchPosNameCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchPosName))
                positionsToLimit = positionsToLimit.Where(n => n.Name.ToLower().Contains(searchPosName)).ToList();
        }

        /// <summary>
        /// Ограничение списка должностей по названию подразделения
        /// </summary>
        /// <param name="orgViewModelCash"></param>
        /// <param name="positionsToLimit"></param>
        private void LimitPosByDivision(OrganizationViewModel orgViewModelCash, ref List<Position> positionsToLimit)
        {
            string seacrhPositionDivName = orgViewModelCash.SeacrhPositionDivNameCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(seacrhPositionDivName))
            {
                TransformCollection(
                    collectionToLimit: ref positionsToLimit,
                    limitingCollection: context.GetOrgDivisions(orgViewModelCash.IdCash.GetValueOrDefault(currentUser.Id)),
                    limitCondition: n => n.Name.ToLower().Contains(seacrhPositionDivName),
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
            string searchPrimaryEmployeeName = orgViewModelCash.SearchPrimaryEmployeeNameCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchPrimaryEmployeeName))
            {
                TransformCollection(
                    collectionToLimit: ref positionsToLimit,
                    limitingCollection: context.GetOrgEmployees(orgViewModelCash.IdCash.GetValueOrDefault(currentUser.Id)),
                    limitCondition: n => n.GetFullName().ToLower().Contains(searchPrimaryEmployeeName),
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
            string searchParentPosName = orgViewModelCash.SearchParentPosNameCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchParentPosName))
            {
                TransformCollection(
                    collectionToLimit: ref positionsToLimit,
                    limitingCollection: context.GetOrgPositions(orgViewModelCash.IdCash.GetValueOrDefault(currentUser.Id)),
                    limitCondition: n => n.Name.ToLower().Contains(searchParentPosName),
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
                .TransformToViewModels<Employee, EmployeeViewModel, EmployeeTransformer>(
                    transformer: new EmployeeTransformer(context, resManager),
                    limitingFunc: GetLimitedEmployeesList);
        }

        private List<Employee> GetLimitedEmployeesList(List<Employee> employees)
        {
            OrganizationViewModel orgViewModelCash = ModelCash<OrganizationViewModel>.GetViewModel(currentUser.Id, EMPLOYEES);
            List<Employee> limitedEmployees = employees;
            LimitEmpByName(orgViewModelCash, ref limitedEmployees);
            LimitEmpByPrimaryPosition(orgViewModelCash, ref limitedEmployees);
            LimitEmpByDivision(orgViewModelCash, ref limitedEmployees);
            LimitListByPageNumber(currentUser.Id, EMPLOYEES, ref limitedEmployees);
            return limitedEmployees;
        }

        /// <summary>
        /// Ограничение списка сотрудников по фио
        /// </summary>
        /// <param name="orgViewModelCash"></param>
        /// <param name="employeesToLimit"></param>
        private void LimitEmpByName(OrganizationViewModel orgViewModelCash, ref List<Employee> employeesToLimit)
        {
            string searchEmployeeName = orgViewModelCash.SearchEmployeeNameCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchEmployeeName))
                employeesToLimit = employeesToLimit.Where(n => n.GetFullName().ToLower().Contains(searchEmployeeName)).ToList();
        }

        /// <summary>
        /// Ограничение списка сотрудников по названию основной должности
        /// </summary>
        /// <param name="orgViewModelCash"></param>
        /// <param name="employeesToLimit"></param>
        private void LimitEmpByPrimaryPosition(OrganizationViewModel orgViewModelCash, ref List<Employee> employeesToLimit)
        {
            string searchEmployeePrimaryPosName = orgViewModelCash.SearchEmployeePrimaryPosNameCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchEmployeePrimaryPosName))
            {
                TransformCollection(
                    collectionToLimit: ref employeesToLimit,
                    limitingCollection: context.GetOrgPositions(orgViewModelCash.IdCash.GetValueOrDefault(currentUser.Id)),
                    limitCondition: n => n.Name.ToLower().Contains(searchEmployeePrimaryPosName),
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
            string seacrhEmployeeDivName = orgViewModelCash.SeacrhEmployeeDivNameCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(seacrhEmployeeDivName))
            {
                TransformCollection(
                    collectionToLimit: ref employeesToLimit,
                    limitingCollection: context.GetOrgDivisions(orgViewModelCash.IdCash.GetValueOrDefault(currentUser.Id)),
                    limitCondition: n => n.Name.ToLower().Contains(seacrhEmployeeDivName),
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
        /*public void AttachResponsibilities(OrganizationViewModel orgViewModel)
        {
            orgViewModel.Responsibilities = orgViewModel.GetResponsibilities(context)
                .TransformToViewModels<Responsibility, ResponsibilityViewModel, ResponsibilityTransformer>(
                    transformer: new ResponsibilityTransformer(context, resManager),
                    limitingFunc: GetLimitedResponsibilitiesList);
        }

        private List<Responsibility> GetLimitedResponsibilitiesList(List<Responsibility> responsibilities)
        {
            OrganizationViewModel orgViewModelCash = ModelCash<OrganizationViewModel>.GetViewModel(RESPONSIBILITIES);
            List<Responsibility> limitedResponsibilities = responsibilities;
            LimitRespByName(orgViewModelCash, ref limitedResponsibilities);
            LimitListByPageNumber(RESPONSIBILITIES, ref limitedResponsibilities);
            return limitedResponsibilities;
        }*/

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

        #region Other Methods
        /// <summary>
        /// Метод возвращает список моделей представления сотрудника, ограниченнный по id организации и имени
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="employeePart"></param>
        /// <returns></returns>
        public List<EmployeeViewModel> GetOrgEmployeeViewModels(Guid organizationId, string employeePart)
        {
            List<EmployeeViewModel> employeeViewModels = context.GetOrgEmployees(organizationId)
                .TransformToViewModels<Employee, EmployeeViewModel, EmployeeTransformer>(
                    transformer: new EmployeeTransformer(context, resManager),
                    limitingFunc: n => n.GetFullName().ToLower().Contains(employeePart.ToLower().TrimStartAndEnd()) && n.EmployeeStatus != EmployeeStatus.Lock);
            return employeeViewModels;
        }
        #endregion
    }
}
