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
    }
}
