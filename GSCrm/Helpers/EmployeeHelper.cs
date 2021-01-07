using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GSCrm.Helpers
{
    public static class EmployeeHelper
    {

        /// <summary>
        /// Получает инициалы для объекта типа Employee
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public static string GetIntialsFullName(this Employee employee)
        {
            if (employee == null) return string.Empty;
            if (string.IsNullOrEmpty(employee.FirstName) || string.IsNullOrEmpty(employee.LastName))
                return string.Empty;
            if (string.IsNullOrEmpty(employee.MiddleName))
                return new StringBuilder().Append(employee.LastName).Append(" ").Append(employee.FirstName[0]).Append(".").ToString();
            return new StringBuilder().Append(employee.LastName).Append(" ").Append(employee.FirstName[0]).Append(".").Append(" ").Append(employee.MiddleName[0]).Append(".").ToString();
        }

        /// <summary>
        /// Получает полное имя объекта Employee
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public static string GetFullName(this Employee employee)
        {
            if (employee == null) return string.Empty;
            if (string.IsNullOrEmpty(employee.FirstName) || string.IsNullOrEmpty(employee.LastName))
                return string.Empty;
            if (string.IsNullOrEmpty(employee.MiddleName))
                return new StringBuilder().Append(employee.LastName).Append(" ").Append(employee.FirstName).ToString();
            return new StringBuilder().Append(employee.LastName).Append(" ").Append(employee.FirstName).Append(" ").Append(employee.MiddleName).ToString();
        }

        public static Organization GetOrganization(this Employee employee, ApplicationDbContext context)
            => context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == employee.OrganizationId);

        public static Division GetDivision(this Employee employee, ApplicationDbContext context)
            => context.Divisions.AsNoTracking().FirstOrDefault(i => i.Id == employee.DivisionId);

        public static Division GetDivision(this EmployeeViewModel employeeViewModel, ApplicationDbContext context)
            => context.Divisions.AsNoTracking().FirstOrDefault(i => i.Id == employeeViewModel.DivisionId);

        public static Position GetPrimaryPosition(this Employee employee, ApplicationDbContext context)
            => context.Positions.AsNoTracking().FirstOrDefault(i => i.Id == employee.PrimaryPositionId);

        public static List<EmployeePosition> GetPositions(this EmployeeViewModel employeeViewModel, ApplicationDbContext context)
            => context.EmployeePositions.AsNoTracking().Where(empId => empId.EmployeeId == employeeViewModel.Id).ToList();
        public static List<EmployeePosition> GetPositions(this Employee employee, ApplicationDbContext context)
            => context.EmployeePositions.AsNoTracking().Where(empId => empId.EmployeeId == employee.Id).ToList();

        public static List<EmployeeResponsibility> GetResponsibilities(this Employee employee, ApplicationDbContext context)
            => context.EmployeeResponsibilities.AsNoTracking().Where(empId => empId.EmployeeId == employee.Id).ToList();

        public static List<EmployeeContact> GetContacts(this EmployeeViewModel employeeViewModel, ApplicationDbContext context)
            => context.EmployeeContacts.AsNoTracking().Where(empId => empId.EmployeeId == employeeViewModel.Id).ToList();

        public static List<Employee> GetSubordinates(this Employee employee, ApplicationDbContext context)
            => GetSubordinates(context, employee.Id, (Guid)employee.DivisionId, employee.PrimaryPositionId);

        public static List<Employee> GetSubordinates(this EmployeeViewModel employeeViewModel, ApplicationDbContext context)
            => GetSubordinates(context, employeeViewModel.Id, employeeViewModel.DivisionId, employeeViewModel.PrimaryPositionId);

        private static List<Employee> GetSubordinates(ApplicationDbContext context, Guid employeeId, Guid? divisionId, Guid? primaryPositionId)
        {
            if (divisionId == null) return new List<Employee>();
            List<Employee> subordinates = new List<Employee>();
            Division division = context.Divisions.AsNoTracking().FirstOrDefault(i => i.Id == divisionId);
            List<Position> childPositions = division.GetPositions(context).Where(pos => pos.ParentPositionId == primaryPositionId).ToList();
            childPositions.ForEach(childPosition =>
            {
                // Для каждой должности находится список сотрудников, которые ее занимают
                // Из списка исключается сам сотрудник, для которого ищутся подчиненные и дубликаты
                // Также необходимо брать только сотрудников, у которых основной должностью является текущая дочерняя
                childPosition.GetEmployees(context)
                    .Where(sub => sub != null &&
                        sub.Id != employeeId &&
                        sub.PrimaryPositionId == childPosition.Id &&
                        !subordinates.Contains(sub, new EmployeeComparer()))
                    .ToList().ForEach(subordinate => subordinates.Add(subordinate));
            });
            return subordinates;
        }

        public static Employee GetSupervisor(this Employee employee, ApplicationDbContext context)
        {
            Position primaryPosition = employee.GetPrimaryPosition(context);
            if (primaryPosition == null) return null;
            Position parentPosition = primaryPosition.GetParentPosition(context);
            if (parentPosition == null) return null;
            return context.Employees.AsNoTracking().FirstOrDefault(i => i.Id == parentPosition.PrimaryEmployeeId && i.Id != employee.Id);
        }

        /// <summary>
        /// Метод проверяет, имеет ли сотрудник разрешение на выполнение поданной на вход операции
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="actionName">Название действие, которое хочет выполнить сотрудник</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool HasPermissionFor(this Employee employee, string actionName, ApplicationDbContext context)
        {
            // Получение списка всех полномочий сотрудника
            List<Responsibility> responsibilities = context.EmployeeResponsibilities
                .AsNoTracking()
                .Include(resp => resp.Responsibility)
                .Where(emp => emp.EmployeeId == employee.Id)
                .Select(resp => resp.Responsibility).ToList();

            // В каждом полномочии проверяется, доступно ли действие для исппользования
            // Если хотя бы в одном доступно, возвращается true
            foreach (Responsibility responsibility in responsibilities)
            {
                // Получение свойства по названию
                PropertyInfo propertyInfo = responsibility.GetType().GetProperty(actionName);
                if (propertyInfo == null) continue;

                // Получение значения свойства и попытка приведения его к bool
                var propertyValue = propertyInfo.GetValue(responsibility, null);
                bool hasPermission = false;
                try
                {
                    hasPermission = (bool)propertyValue;
                }
                catch (InvalidCastException) { }
                if (hasPermission) return true;
            };
            return false;
        }

        public static void Lock(this Employee employee, EmployeeLockReason lockReason = EmployeeLockReason.None)
        {
            employee.EmployeeStatus = EmployeeStatus.Lock;
            employee.EmployeeLockReason = lockReason;
        }

        public static void Unlock(this Employee employee)
        {
            employee.EmployeeStatus = EmployeeStatus.Active;
            employee.EmployeeLockReason = EmployeeLockReason.None;
        }
    }
}
