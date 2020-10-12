using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GSCrm.Helpers
{
    public static class PositionHelpers
    {
        /// <summary>
        /// Получает текущую выбранную организацию для объекта PositionViewModel
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="divisionName"></param>
        /// <returns></returns>
        public static Organization GetOrganization(this PositionViewModel positionViewModel, ApplicationDbContext context)
            => GetOrganization(positionViewModel.OrganizationId, context);

        public static Organization GetOrganization(this Position position, ApplicationDbContext context)
        {
            Division positionDiv = context.Divisions.FirstOrDefault(i => i.Id == position.DivisionId);
            return GetOrganization(positionDiv.OrganizationId, context);
        }

        private static Organization GetOrganization(Guid organizationId, ApplicationDbContext context)
        {
            return context.Organizations
                .Include(div => div.Divisions)
                    .ThenInclude(pos => pos.Positions)
                .FirstOrDefault(i => i.Id == organizationId);
        }

        /// <summary>
        /// Методы возвращают подразделение для должности
        /// </summary>
        /// <param name="positionViewModel"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Division GetDivision(this PositionViewModel positionViewModel, ApplicationDbContext context)
        {
            Organization organization = positionViewModel.GetOrganization(context);
            return organization.Divisions.FirstOrDefault(n => n.Name == positionViewModel.DivisionName);
        }

        public static Division GetDivision(this Position position, ApplicationDbContext context)
            => context.Divisions.Include(pos => pos.Positions).FirstOrDefault(i => i.Id == position.DivisionId);

        /// <summary>
        /// Методы возвращают родительскую должность
        /// </summary>
        /// <param name="position"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Position GetParentPosition(this Position position, ApplicationDbContext context)
            => context.Positions.Include(div => div.Division).FirstOrDefault(i => i.Id == position.ParentPositionId);
        public static Position GetParentPosition(this PositionViewModel positionViewModel, Division division)
            => division.Positions.FirstOrDefault(n => n.Name == positionViewModel.ParentPositionName);

        /// <summary>
        /// Методы возвращают список всез дочерних должностей
        /// </summary>
        /// <param name="positionViewModel"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<Position> GetSubPositions(this Position position, ApplicationDbContext context)
            => context.Positions.Where(pos => pos.ParentPositionId == position.Id).ToList();
        public static List<Position> GetSubPositions(this PositionViewModel positionViewModel, ApplicationDbContext context)
            => context.Positions.Where(pos => pos.ParentPositionId == positionViewModel.Id).ToList();

        /// <summary>
        /// Метод возвращает признак, указывающий, является ли должность, на которой был вызван метод родительской для той, которая в метод передается
        /// </summary>
        /// <param name="currentPosition">Текущая должность</param>
        /// <param name="position">Должность, отношение которой к текущей необходимо выяснить</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsParentPositionFor(this Position currentPosition, Position position, ApplicationDbContext context)
        {
            PositionRepository positionRepository = new PositionRepository(context);
            List<Position> parentPositions = positionRepository.GetParentPositionsHierarchy(position);
            if (parentPositions.Contains(currentPosition)) return true;
            return false;
        }

        /// <summary>
        /// Методы возвращают основного сотрудника для должности
        /// </summary>
        /// <param name="position"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Employee GetPrimaryEmployee(this Position position, ApplicationDbContext context)
            => context.Employees.FirstOrDefault(i => i.Id == position.PrimaryEmployeeId);
        public static Employee GetPrimaryEmployee(this PositionViewModel positionViewModel, ApplicationDbContext context)
            => context.Employees.FirstOrDefault(i => i.Id == positionViewModel.PrimaryEmployeeId);

        /// <summary>
        /// Метод возвращает список всех сотрудников, находящихся на этой должности
        /// </summary>
        /// <param name="positionViewModel"></param>
        /// <param name="context"></param>
        /// <returns></returns>

        public static List<Employee> GetEmployees(this PositionViewModel positionViewModel, ApplicationDbContext context)
        {
            return context.EmployeePositions
                .Include(emp => emp.Employee)
                .Where(posId => posId.PositionId == positionViewModel.Id).ToList()
                .Select(emp => emp.Employee).ToList();
        }
    }
}
