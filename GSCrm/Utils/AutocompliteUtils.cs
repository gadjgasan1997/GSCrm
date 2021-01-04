using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Mapping;
using GSCrm.Models;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

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
            => GetDivisionEmployees(orgId, divNamePart, employeePart).MapToViewModels(employeeMap,
                emp => emp.GetFullName().ToLower().Contains(employeePart.TrimStartAndEnd().ToLower()));

        public List<EmployeeViewModel> GetEmployeesByInitials(string orgId, string divNamePart, string employeePart)
            => GetDivisionEmployees(orgId, divNamePart, employeePart).MapToViewModels(employeeMap,
                emp => emp.GetIntialsFullName().ToLower().Contains(employeePart.TrimStartAndEnd().ToLower()));

        private List<Employee> GetDivisionEmployees(string orgId, string divNamePart, string employeePart)
        {
            if (!string.IsNullOrEmpty(divNamePart) && !string.IsNullOrEmpty(employeePart) && Guid.TryParse(orgId, out Guid guid))
            {
                Division selectedDivision = context.GetOrgDivisions(guid).FirstOrDefault(n => n.Name == divNamePart);
                if (selectedDivision == null) return new List<Employee>();
                return selectedDivision.GetEmployees(context);
            }
            return new List<Employee>();
        }

        public List<EmployeeViewModel> GetOrgEmployeesByFullName(string orgId, string employeePart)
        {
            if (!string.IsNullOrEmpty(orgId) && !string.IsNullOrEmpty(employeePart) && Guid.TryParse(orgId, out Guid organizationId))
                return GetOrgEmployeesByFullName(organizationId, employeePart);
            return new List<EmployeeViewModel>();
        }

        public List<EmployeeViewModel> GetOrgEmployeesByInitials(string orgId, string employeePart)
        {
            if (!string.IsNullOrEmpty(orgId) && !string.IsNullOrEmpty(employeePart) && Guid.TryParse(orgId, out Guid organizationId))
                return GetOrgEmployeesByInitials(organizationId, employeePart);
            return new List<EmployeeViewModel>();
        }

        public List<EmployeeViewModel> GetOrgEmployeesByFullName(User currentUser, string employeePart)
        {
            if (!string.IsNullOrEmpty(employeePart) && currentUser.PrimaryOrganizationId != null)
                return GetOrgEmployeesByFullName(currentUser.PrimaryOrganizationId, employeePart);
            return new List<EmployeeViewModel>();
        }

        public List<EmployeeViewModel> GetOrgEmployeesByInitials(User currentUser, string employeePart)
        {
            if (!string.IsNullOrEmpty(employeePart) && currentUser.PrimaryOrganizationId != null)
                return GetOrgEmployeesByInitials(currentUser.PrimaryOrganizationId, employeePart);
            return new List<EmployeeViewModel>();
        }

        private List<EmployeeViewModel> GetOrgEmployeesByFullName(Guid orgId, string employeePart)
            => context.GetOrgEmployees(orgId)
                    .MapToViewModels(employeeMap,
                        n => n.GetFullName().ToLower().Contains(employeePart.ToLower().TrimStartAndEnd()) && n.EmployeeStatus == EmployeeStatus.Active);

        private List<EmployeeViewModel> GetOrgEmployeesByInitials(Guid orgId, string employeePart)
            => context.GetOrgEmployees(orgId)
                    .MapToViewModels(employeeMap,
                        n => n.GetIntialsFullName().ToLower().Contains(employeePart.ToLower().TrimStartAndEnd()) && n.EmployeeStatus == EmployeeStatus.Active);

        public List<EmployeeViewModel> GetAccountManagersByFullName(Account account, string managerName)
            => account.AccountManagers.Select(man => man.Manager).ToList()
            .MapToViewModels(new EmployeeMap(serviceProvider, context), n => n.GetFullName().ToLower().Contains(managerName.ToLower().TrimStartAndEnd()));
    }
}
