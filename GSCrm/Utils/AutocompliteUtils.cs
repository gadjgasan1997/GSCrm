using System;
using System.Linq;
using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Mapping;
using GSCrm.Models;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using System.Collections.Generic;
using static GSCrm.CommonConsts;

namespace GSCrm.Utils
{
    public class AutocompliteUtils
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ApplicationDbContext context;
        private readonly EmployeeMap employeeMap;
        public AutocompliteUtils(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            this.serviceProvider = serviceProvider;
            this.context = context;
            employeeMap = new EmployeeMap(serviceProvider, context);
        }

        public List<EmployeeViewModel> GetEmployeesByFullName(string orgId, string divNamePart, string employeePart)
        {
            IEnumerable<Employee> divisionEmployees = GetDivisionEmployees(orgId, divNamePart).Take(AUTOCOMPLITE_ITEMS_DEF_COUNT);
            string employeeSearchName = employeePart.TrimStartAndEnd().ToLower();
            return employeeSearchName switch
            {
                "" => divisionEmployees.GetViewModelsFromData(employeeMap),
                _ => divisionEmployees.MapToViewModels(employeeMap, emp => emp.GetFullName().ToLower().Contains(employeeSearchName))
            };
        }

        public List<EmployeeViewModel> GetEmployeesByInitials(string orgId, string divNamePart, string employeePart)
        {
            IEnumerable<Employee> divisionEmployees = GetDivisionEmployees(orgId, divNamePart).Take(AUTOCOMPLITE_ITEMS_DEF_COUNT);
            string employeeSearchName = employeePart.TrimStartAndEnd().ToLower();
            return employeeSearchName switch
            {
                "" => divisionEmployees.GetViewModelsFromData(employeeMap),
                _ => divisionEmployees.MapToViewModels(employeeMap, emp => emp.GetIntialsFullName().ToLower().Contains(employeeSearchName))
            };
        }

        /// <summary>
        /// Получение списка сотрудников подразделения
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="divNamePart">Название подразделения</param>
        /// <returns></returns>
        private List<Employee> GetDivisionEmployees(string orgId, string divNamePart)
        {
            if (string.IsNullOrEmpty(divNamePart) || !Guid.TryParse(orgId, out Guid organizationId))
                return new List<Employee>();
             
            // Попытка получить пордразделение
            if (context.GetOrgDivisions(organizationId).FirstOrDefault(n => n.Name == divNamePart) is not Division selectedDivision)
                return new List<Employee>();
            return selectedDivision.GetEmployees(context);
        }

        public List<EmployeeViewModel> GetOrgEmployeesByFullName(string orgId, string employeePart)
        {
            if (!string.IsNullOrEmpty(orgId) && !string.IsNullOrEmpty(employeePart) && Guid.TryParse(orgId, out Guid organizationId))
                return GetOrgEmployeesByFullName(organizationId, employeePart);
            return new List<EmployeeViewModel>();
        }

        public List<EmployeeViewModel> GetOrgEmployeesByInitials(string orgId, string employeePart)
        {
            if (string.IsNullOrEmpty(orgId) || !Guid.TryParse(orgId, out Guid organizationId))
                return new List<EmployeeViewModel>();
            return GetOrgEmployeesByInitials(organizationId, employeePart);
        }

        public List<EmployeeViewModel> GetOrgEmployeesByFullName(User currentUser, string employeePart)
            => GetOrgEmployeesByInitials(currentUser.PrimaryOrganizationId, employeePart);

        public List<EmployeeViewModel> GetOrgEmployeesByInitials(User currentUser, string employeePart)
            => GetOrgEmployeesByInitials(currentUser.PrimaryOrganizationId, employeePart);

        private List<EmployeeViewModel> GetOrgEmployeesByFullName(Guid orgId, string employeePart)
            => string.IsNullOrEmpty(employeePart) switch
            {
                true => context.GetOrgEmployees(orgId).GetViewModelsFromData(employeeMap),
                _ => context.GetOrgEmployees(orgId).MapToViewModels(
                    map: employeeMap,
                    limitingFunc: n => n.GetFullName().ToLower().Contains(employeePart.ToLower().TrimStartAndEnd()) && n.EmployeeStatus == EmployeeStatus.Active)
            };

        private List<EmployeeViewModel> GetOrgEmployeesByInitials(Guid orgId, string employeePart)
            => string.IsNullOrEmpty(employeePart) switch
            {
                true => context.GetOrgEmployees(orgId).GetViewModelsFromData(employeeMap),
                _ => context.GetOrgEmployees(orgId).MapToViewModels(
                    map: employeeMap,
                    limitingFunc: n => n.GetIntialsFullName().ToLower().Contains(employeePart.ToLower().TrimStartAndEnd()) && n.EmployeeStatus == EmployeeStatus.Active)
            };

        public List<EmployeeViewModel> GetAccountManagersByFullName(Account account, string managerName)
            => string.IsNullOrEmpty(managerName) switch
            {
                true => account.AccountManagers.Select(man => man.Manager).GetViewModelsFromData(new EmployeeMap(serviceProvider, context)),
                _ => account.AccountManagers.Select(man => man.Manager)
                        .MapToViewModels(new EmployeeMap(serviceProvider, context),
                            n => n.GetFullName().ToLower().Contains(managerName.ToLower().TrimStartAndEnd()))
            };
    }
}
