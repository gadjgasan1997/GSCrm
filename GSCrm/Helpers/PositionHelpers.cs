using System;
using System.Linq;
using System.Collections.Generic;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GSCrm.Helpers
{
    public static class PositionHelpers
    {
        #region Organizaqtions
        public static Organization GetOrganization(this Position position, ApplicationDbContext context)
            => context.Organizations.AsNoTracking().FirstOrDefault(org => org.Id == position.OrganizationId);
        #endregion

        #region Divisions
        public static Division GetDivision(this Position position, ApplicationDbContext context)
            => context.Divisions.AsNoTracking().FirstOrDefault(i => i.Id == position.DivisionId);
        #endregion

        #region Employees
        public static List<Employee> GetEmployees(this PositionViewModel positionViewModel, ApplicationDbContext context) => GetEmployees(positionViewModel.Id, context);
        public static List<Employee> GetEmployees(this Position position, ApplicationDbContext context) => GetEmployees(position.Id, context);
        private static List<Employee> GetEmployees(Guid positionId, ApplicationDbContext context)
        {
            return context.EmployeePositions
                .AsNoTracking()
                .Include(emp => emp.Employee)
                .Where(posId => posId.PositionId == positionId).ToList()
                .Select(emp => emp.Employee).ToList();
        }
        public static Employee GetPrimaryEmployee(this Position position, ApplicationDbContext context)
            => context.Employees.AsNoTracking().FirstOrDefault(i => i.Id == position.PrimaryEmployeeId);
        public static Employee GetPrimaryEmployee(this PositionViewModel positionViewModel, ApplicationDbContext context)
            => context.Employees.AsNoTracking().FirstOrDefault(i => i.Id == positionViewModel.PrimaryEmployeeId);
        #endregion

        #region Positions
        public static Position GetParentPosition(this Position position, ApplicationDbContext context)
            => context.Positions.AsNoTracking().FirstOrDefault(i => i.Id == position.ParentPositionId);
        public static Position GetParentPosition(this PositionViewModel positionViewModel, Division division, ApplicationDbContext context)
            => division.GetPositions(context).FirstOrDefault(n => n.Name == positionViewModel.ParentPositionName);
        public static List<Position> GetSubPositions(this Position position, ApplicationDbContext context)
            => context.Positions.AsNoTracking().Where(pos => pos.ParentPositionId == position.Id).ToList();
        public static List<Position> GetSubPositions(this PositionViewModel positionViewModel, ApplicationDbContext context)
            => context.Positions.AsNoTracking().Where(pos => pos.ParentPositionId == positionViewModel.Id).ToList();
        #endregion

        #region Actions
        public static void Lock(this Position position, PositionLockReason lockReason = PositionLockReason.None)
        {
            position.PositionStatus = PositionStatus.Lock;
            position.PositionLockReason = lockReason;
        }

        public static void Unlock(this Position position)
        {
            position.PositionStatus = PositionStatus.Active;
            position.PositionLockReason = PositionLockReason.None;
        }

        public static PositionViewModel Refresh(this PositionViewModel positionViewModel, PositionViewModel cachedViewModel)
        {
            if (cachedViewModel == null)
                return positionViewModel;

            positionViewModel.SearchEmployeeInitialName = cachedViewModel.SearchEmployeeInitialName;
            positionViewModel.SearchSubPositionName = cachedViewModel.SearchSubPositionName;
            positionViewModel.SearchSubPositionPrimaryEmployee = cachedViewModel.SearchSubPositionPrimaryEmployee;
            return positionViewModel;
        }

        public static void Normalize(this PositionViewModel positionViewModel)
        {
            positionViewModel.Name = positionViewModel.Name?.TrimStartAndEnd();
            positionViewModel.ParentPositionName = positionViewModel.ParentPositionName?.TrimStartAndEnd();
            positionViewModel.DivisionName = positionViewModel.DivisionName?.TrimStartAndEnd();
            positionViewModel.PrimaryEmployeeInitialName = positionViewModel.PrimaryEmployeeInitialName?.TrimStartAndEnd();
        }

        public static void NormalizeSearch(this PositionViewModel positionViewModel)
        {
            positionViewModel.SearchEmployeeInitialName = positionViewModel.SearchEmployeeInitialName?.ToLower().TrimStartAndEnd();
            positionViewModel.SearchSubPositionName = positionViewModel.SearchSubPositionName?.ToLower().TrimStartAndEnd();
            positionViewModel.SearchSubPositionPrimaryEmployee = positionViewModel.SearchSubPositionPrimaryEmployee?.ToLower().TrimStartAndEnd();
        }
        #endregion
    }
}
