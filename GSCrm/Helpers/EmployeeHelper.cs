using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSCrm.Helpers
{
    public static class EmployeeHelper
    {
        /// <summary>
        /// Методы возвращают организацию сотрудника
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <returns></returns>
        public static Organization GetOrganization(this EmployeeViewModel employeeViewModel, ApplicationDbContext context)
        {
            return context.Organizations
                .Include(userOrg => userOrg.UserOrganizations)
                .Include(div => div.Divisions)
                    .ThenInclude(pos => pos.Positions)
                .FirstOrDefault(i => i.Id == employeeViewModel.OrganizationId);
        }

        public static Organization GetOrganization(this Employee employee, ApplicationDbContext context)
        {
            Division employeeDivision = employee.GetDivision(context);
            return context.Organizations.FirstOrDefault(i => i.Id == employeeDivision.OrganizationId);
        }

        /// <summary>
        /// Методы возвращают подразделение сотрудника
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <param name="currentOrganization"></param>
        /// <returns></returns>
        public static Division GetDivision(this EmployeeViewModel employeeViewModel, Organization currentOrganization)
            => currentOrganization.Divisions.FirstOrDefault(n => n.Name == employeeViewModel.DivisionName);
        public static Division GetDivision(this Employee employee, ApplicationDbContext context)
            => context.Divisions.Include(pos => pos.Positions).FirstOrDefault(i => i.Id == employee.DivisionId);
        public static Division GetDivision(this EmployeeViewModel employeeViewModel, ApplicationDbContext context)
            => context.Divisions.Include(pos => pos.Positions).FirstOrDefault(i => i.Id == employeeViewModel.DivisionId);

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

        /// <summary>
        /// Методы возвращают основную должность сотрудника
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Position GetPrimaryPosition(this Employee employee, ApplicationDbContext context)
        {
            return context.Positions
                .Include(div => div.Division)
                    .ThenInclude(pos => pos.Positions)
                .FirstOrDefault(i => i.Id == employee.PrimaryPositionId);
        }
        
        public static Position GetPrimaryPosition(this EmployeeViewModel employeeViewModel, Division division)
            => division.Positions.FirstOrDefault(n => n.Name == employeeViewModel.PrimaryPositionName);

        /// <summary>
        /// Метод возвращает список долнжостей сотрудника
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<EmployeePosition> GetPositions(this EmployeeViewModel employeeViewModel, ApplicationDbContext context)
            => context.EmployeePositions.Where(empId => empId.EmployeeId == employeeViewModel.Id).ToList();

        /// <summary>
        /// Метод возвращает список контактов сотрудника
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<EmployeeContact> GetContacts(this EmployeeViewModel employeeViewModel, ApplicationDbContext context)
            => context.EmployeeContacts.Where(empId => empId.EmployeeId == employeeViewModel.Id).ToList();

        /// <summary>
        /// Методы возвращают список всех подчиненных
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<Employee> GetSubordinates(this Employee employee, ApplicationDbContext context)
            => GetSubordinates(context, employee.DivisionId, employee.PrimaryPositionId);

        public static List<Employee> GetSubordinates(this EmployeeViewModel employeeViewModel, ApplicationDbContext context)
            => GetSubordinates(context, employeeViewModel.DivisionId, employeeViewModel.PrimaryPositionId);

        private static List<Employee> GetSubordinates(ApplicationDbContext context, Guid divisionId, Guid? primaryPositionId)
        {
            Division division = context.Divisions.FirstOrDefault(i => i.Id == divisionId);
            List<Position> childPositions = division.GetPositions(context).Where(pos => pos.ParentPositionId == primaryPositionId).ToList();
            List<Employee> subordinates = new List<Employee>();
            childPositions.ForEach(childPosition =>
            {
                if (childPosition.PrimaryEmployeeId != null)
                {
                    Employee subordinate = childPosition.GetPrimaryEmployee(context);
                    if (subordinate != null && !subordinates.Contains(subordinate, new EmployeeComparer()))
                        subordinates.Add(subordinate);
                }
            });
            return subordinates;
        }

        /// <summary>
        /// Методы лочат/разблокировывают сотрудника
        /// </summary>
        /// <param name="employee"></param>
        public static void Lock(this Employee employee) => employee.EmployeeStatus = EmployeeStatus.Lock;
        public static void Unlock(this Employee employee) => employee.EmployeeStatus = EmployeeStatus.Active;
    }
}
