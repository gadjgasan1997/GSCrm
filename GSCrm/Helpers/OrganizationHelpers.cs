using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace GSCrm.Helpers
{
    public static class OrganizationHelpers
    {
        #region Divisions
        public static List<Division> GetDivisions(this OrganizationViewModel orgViewModel, ApplicationDbContext context)
            => context.Divisions.AsNoTracking().Where(div => div.OrganizationId == orgViewModel.Id).ToList();
        public static List<Division> GetDivisions(this Organization organization, ApplicationDbContext context)
            => context.Divisions.AsNoTracking().Where(div => div.OrganizationId == organization.Id).ToList();
        #endregion

        #region Positions
        public static List<Position> GetAllPositions(this OrganizationViewModel orgViewModel, ApplicationDbContext context)
            => context.Positions.AsNoTracking().Where(pos => pos.OrganizationId == orgViewModel.Id).ToList();
        public static List<Position> GetPositions(this Organization organization, ApplicationDbContext context)
            => context.Positions.AsNoTracking().Where(pos => pos.OrganizationId == organization.Id).ToList();
        #endregion

        #region Employees
        public static List<Employee> GetAllEmployees(this OrganizationViewModel orgViewModel, ApplicationDbContext context)
            => context.Employees.AsNoTracking().Where(emp => emp.OrganizationId == orgViewModel.Id).ToList();
        public static List<Employee> GetEmployees(this Organization organization, ApplicationDbContext context)
            => context.Employees.AsNoTracking().Where(emp => emp.OrganizationId == organization.Id).ToList();
        #endregion

        #region Responsibilities
        public static List<Responsibility> GetResponsibilities(this OrganizationViewModel orgViewModel, ApplicationDbContext context)
            => context.Responsibilities.AsNoTracking().Where(resp => resp.OrganizationId == orgViewModel.Id).ToList();
        public static List<Responsibility> GetResponsibilities(this Organization organization, ApplicationDbContext context)
            => context.Responsibilities.AsNoTracking().Where(resp => resp.OrganizationId == organization.Id).ToList();
        #endregion

        #region Prducts
        public static List<ProductCategory> GetProductCategories(this Organization organization, ApplicationDbContext context)
            => context.ProductCategories.AsNoTracking().Where(prodCat => prodCat.OrganizationId == organization.Id).ToList();
        #endregion

        public static OrganizationViewModel Refresh(this OrganizationViewModel orgViewModel, OrganizationViewModel cachedViewModel = null)
        {
            if (cachedViewModel == null)
                return orgViewModel;

            orgViewModel.SearchDivName = cachedViewModel.SearchDivName;
            orgViewModel.SearchParentDivName = cachedViewModel.SearchParentDivName;
            orgViewModel.SearchPosName = cachedViewModel.SearchPosName;
            orgViewModel.SeacrhPositionDivName = cachedViewModel.SeacrhPositionDivName;
            orgViewModel.SearchParentPosName = cachedViewModel.SearchParentPosName;
            orgViewModel.SearchPrimaryEmployeeName = cachedViewModel.SearchPrimaryEmployeeName;
            orgViewModel.SearchEmployeeName = cachedViewModel.SearchEmployeeName;
            orgViewModel.SeacrhEmployeeDivName = cachedViewModel.SeacrhEmployeeDivName;
            orgViewModel.SearchEmployeePrimaryPosName = cachedViewModel.SearchEmployeePrimaryPosName;
            orgViewModel.SeacrhResponsibilityName = cachedViewModel.SeacrhResponsibilityName;
            return orgViewModel;
        }

        public static void NormalizeSearch(this OrganizationsViewModel organizationsViewModel)
        {
            organizationsViewModel.SearchName = organizationsViewModel.SearchName?.ToLower().TrimStartAndEnd();
        }

        public static void Normalize(this OrganizationViewModel organizationViewModel)
        {
            organizationViewModel.Name = organizationViewModel.Name?.TrimStartAndEnd();
        }

        public static void NormalizeSearch(this OrganizationViewModel organizationViewModel)
        {
            organizationViewModel.SearchDivName = organizationViewModel.SearchDivName?.ToLower().TrimStartAndEnd();
            organizationViewModel.SearchParentDivName = organizationViewModel.SearchParentDivName?.ToLower().TrimStartAndEnd();
            organizationViewModel.SearchPosName = organizationViewModel.SearchPosName?.ToLower().TrimStartAndEnd();
            organizationViewModel.SeacrhPositionDivName = organizationViewModel.SeacrhPositionDivName?.ToLower().TrimStartAndEnd();
            organizationViewModel.SearchParentPosName = organizationViewModel.SearchParentPosName?.ToLower().TrimStartAndEnd();
            organizationViewModel.SearchPrimaryEmployeeName = organizationViewModel.SearchPrimaryEmployeeName?.ToLower().TrimStartAndEnd();
            organizationViewModel.SearchEmployeeName = organizationViewModel.SearchEmployeeName?.ToLower().TrimStartAndEnd();
            organizationViewModel.SearchEmployeePrimaryPosName = organizationViewModel.SearchEmployeePrimaryPosName?.ToLower().TrimStartAndEnd();
            organizationViewModel.SeacrhEmployeeDivName = organizationViewModel.SeacrhEmployeeDivName?.ToLower().TrimStartAndEnd();
            organizationViewModel.SeacrhResponsibilityName = organizationViewModel.SeacrhResponsibilityName?.ToLower().TrimStartAndEnd();
        }
    }
}
