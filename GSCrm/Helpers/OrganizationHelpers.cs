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
            => context.Divisions.AsNoTracking().Where(i => i.OrganizationId == orgViewModel.Id).ToList();
        public static List<Division> GetDivisions(this Organization organization, ApplicationDbContext context)
            => context.Divisions.AsNoTracking().Where(i => i.OrganizationId == organization.Id).ToList();
        #endregion

        #region Positions
        public static List<Position> GetPositions(this OrganizationViewModel orgViewModel, ApplicationDbContext context)
            => orgViewModel.GetDivisions(context).GetPositions(context);
        public static List<Position> GetPositions(this Organization organization, ApplicationDbContext context)
            => organization.GetDivisions(context).GetPositions(context);
        private static List<Position> GetPositions(this List<Division> allDivisions, ApplicationDbContext context)
        {
            List<Position> positions = new List<Position>();
            allDivisions.ForEach(division => positions.AddRange(division.GetPositions(context)));
            return positions;
        }
        #endregion

        #region Employees
        public static List<Employee> GetEmployees(this OrganizationViewModel orgViewModel, ApplicationDbContext context)
            => orgViewModel.GetDivisions(context).GetEmployees(context);
        public static List<Employee> GetEmployees(this Organization organization, ApplicationDbContext context)
            => organization.GetDivisions(context).GetEmployees(context);
        private static List<Employee> GetEmployees(this List<Division> allDivisions, ApplicationDbContext context)
        {
            List<Employee> divisionsEmployees = new List<Employee>();
            allDivisions.ForEach(division => divisionsEmployees.AddRange(division.GetEmployees(context)));
            return divisionsEmployees;
        }
        #endregion

        #region Responsibilities
        public static List<Responsibility> GetResponsibilities(this OrganizationViewModel orgViewModel, ApplicationDbContext context)
            => context.Responsibilities.AsNoTracking().Where(resp => resp.OrganizationId == orgViewModel.Id).ToList();
        public static List<Responsibility> GetResponsibilities(this Organization organization, ApplicationDbContext context)
            => context.Responsibilities.AsNoTracking().Where(resp => resp.OrganizationId == organization.Id).ToList();
        #endregion
    }
}
