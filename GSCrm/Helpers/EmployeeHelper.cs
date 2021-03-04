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
        #region Base
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
        #endregion

        #region Divisions
        public static Division GetDivision(this Employee employee, ApplicationDbContext context)
            => context.Divisions.AsNoTracking().FirstOrDefault(i => i.Id == employee.DivisionId);

        public static Division GetDivision(this EmployeeViewModel employeeViewModel, ApplicationDbContext context)
            => context.Divisions.AsNoTracking().FirstOrDefault(i => i.Id == employeeViewModel.DivisionId);
        #endregion

        #region Positions
        public static Position GetPrimaryPosition(this Employee employee, ApplicationDbContext context)
            => context.Positions.AsNoTracking().FirstOrDefault(i => i.Id == employee.PrimaryPositionId);
        public static List<EmployeePosition> GetPositions(this EmployeeViewModel employeeViewModel, ApplicationDbContext context)
            => context.EmployeePositions.AsNoTracking().Where(empId => empId.EmployeeId == employeeViewModel.Id).ToList();
        public static List<EmployeePosition> GetPositions(this Employee employee, ApplicationDbContext context)
            => context.EmployeePositions.AsNoTracking().Where(empId => empId.EmployeeId == employee.Id).ToList();
        #endregion

        #region Employees
        public static List<Employee> GetSubordinates(this Employee employee, ApplicationDbContext context)
            => GetSubordinates(context, employee.Id, (Guid)employee.DivisionId, employee.PrimaryPositionId);

        public static List<Employee> GetSubordinates(this EmployeeViewModel employeeViewModel, ApplicationDbContext context)
            => GetSubordinates(context, employeeViewModel.Id, employeeViewModel.DivisionId, employeeViewModel.PrimaryPositionId);

        private static List<Employee> GetSubordinates(ApplicationDbContext context, Guid employeeId, Guid? divisionId, Guid? primaryPositionId)
        {
            if (divisionId == null || primaryPositionId == null)
                return new List<Employee>();

            // Проверка, что этот сотрудник установлен на должности как основной
            Position position = context.Positions.AsNoTracking().FirstOrDefault(i => i.Id == primaryPositionId);
            if (position == null || position.PrimaryEmployeeId != employeeId)
                return new List<Employee>();

            // Получение подчиненных
            List<Employee> subordinates = new List<Employee>();
            Division division = context.Divisions.AsNoTracking().FirstOrDefault(i => i.Id == divisionId);
            List<Position> childPositions = division.GetPositions(context).Where(pos => pos.ParentPositionId == primaryPositionId).ToList();
            childPositions.ForEach(childPosition =>
            {
                // Для каждой дочерней должности находится список сотрудников, которые ее занимают
                // Из списка исключается сам сотрудник, для которого ищутся подчиненные
                // Необходимо брать только сотрудников, у которых основной должностью является текущая дочерняя
                // Также надо исключить дубликаты
                Func<Employee, bool> predicate = subordinate => subordinate != null &&
                    subordinate.Id != employeeId &&
                    subordinate.PrimaryPositionId == childPosition.Id &&
                    !subordinates.Contains(subordinate, new EmployeeComparer());

                // Добавление полдчиненных в список
                subordinates.AddRange(childPosition.GetEmployees(context).Where(predicate));
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
        #endregion

        #region Actions
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

        /// <summary>
        /// Метод добавляет к сотруднику список должностей, которые он занимает
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Employee AddEmployeePositions(this Employee employee, ApplicationDbContext context)
        {
            List<EmployeePosition> employeePositions = context.EmployeePositions.AsNoTracking().Where(empPos => empPos.EmployeeId == employee.Id).ToList();
            employee.EmployeePositions.AddRange(employeePositions);
            return employee;
        }

        public static EmployeeViewModel Refresh(this EmployeeViewModel employeeViewModel, EmployeeViewModel cachedViewModel)
        {
            if (cachedViewModel == null)
                return employeeViewModel;

            employeeViewModel.SearchPosName = cachedViewModel.SearchPosName;
            employeeViewModel.SearchParentPosName = cachedViewModel.SearchParentPosName;
            employeeViewModel.SearchContactType = cachedViewModel.SearchContactType;
            employeeViewModel.SearchContactPhone = cachedViewModel.SearchContactPhone;
            employeeViewModel.SearchContactEmail = cachedViewModel.SearchContactEmail;
            employeeViewModel.SearchSubordinateFullName = cachedViewModel.SearchSubordinateFullName;
            employeeViewModel.SearchAllPosName = cachedViewModel.SearchAllPosName;
            employeeViewModel.SearchAllParentPosName = cachedViewModel.SearchAllParentPosName;
            employeeViewModel.SearchSelectedPosName = cachedViewModel.SearchSelectedPosName;
            employeeViewModel.SearchSelectedParentPosName = cachedViewModel.SearchSelectedParentPosName;
            employeeViewModel.SearchAllRespName = cachedViewModel.SearchAllRespName;
            employeeViewModel.SearchSelectedRespName = cachedViewModel.SearchSelectedRespName;
            return employeeViewModel;
        }

        public static void Normalize(this EmployeeViewModel employeeViewModel)
        {
            employeeViewModel.UserName = employeeViewModel.UserName?.TrimStartAndEnd();
            employeeViewModel.Email = employeeViewModel.Email?.TrimStartAndEnd();
            employeeViewModel.Password = employeeViewModel.Password?.TrimStartAndEnd();
            employeeViewModel.ConfirmPassword = employeeViewModel.ConfirmPassword?.TrimStartAndEnd();
            employeeViewModel.FirstName = employeeViewModel.FirstName?.TrimStartAndEnd();
            employeeViewModel.LastName = employeeViewModel.LastName?.TrimStartAndEnd();
            employeeViewModel.MiddleName = employeeViewModel.MiddleName?.TrimStartAndEnd();
            employeeViewModel.DivisionName = employeeViewModel.DivisionName?.TrimStartAndEnd();
            employeeViewModel.PrimaryPositionName = employeeViewModel.PrimaryPositionName?.TrimStartAndEnd();
        }

        public static void NormalizeSearch(this EmployeeViewModel employeeViewModel)
        {
            employeeViewModel.SearchPosName = employeeViewModel.SearchPosName?.ToLower().TrimStartAndEnd();
            employeeViewModel.SearchParentPosName = employeeViewModel.SearchParentPosName?.ToLower().TrimStartAndEnd();
            employeeViewModel.SearchContactType = employeeViewModel.SearchContactType;
            employeeViewModel.SearchContactPhone = employeeViewModel.SearchContactPhone?.ToLower().TrimStartAndEnd();
            employeeViewModel.SearchContactEmail = employeeViewModel.SearchContactEmail?.ToLower().TrimStartAndEnd();
            employeeViewModel.SearchSubordinateFullName = employeeViewModel.SearchSubordinateFullName?.ToLower().TrimStartAndEnd();
            employeeViewModel.SearchAllPosName = employeeViewModel.SearchAllPosName?.ToLower().TrimStartAndEnd();
            employeeViewModel.SearchAllParentPosName = employeeViewModel.SearchAllParentPosName?.ToLower().TrimStartAndEnd();
            employeeViewModel.SearchSelectedPosName = employeeViewModel.SearchSelectedPosName?.ToLower().TrimStartAndEnd();
            employeeViewModel.SearchSelectedParentPosName = employeeViewModel.SearchSelectedParentPosName?.ToLower().TrimStartAndEnd();
            employeeViewModel.SearchAllRespName = employeeViewModel.SearchAllRespName?.ToLower().TrimStartAndEnd();
            employeeViewModel.SearchSelectedRespName = employeeViewModel.SearchSelectedRespName?.ToLower().TrimStartAndEnd();
        }
        #endregion

        #region Other Methods
        public static Organization GetOrganization(this Employee employee, ApplicationDbContext context)
            => context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == employee.OrganizationId);
        public static List<EmployeeResponsibility> GetResponsibilities(this Employee employee, ApplicationDbContext context)
            => context.EmployeeResponsibilities.AsNoTracking().Where(empId => empId.EmployeeId == employee.Id).ToList();
        public static List<EmployeeContact> GetContacts(this EmployeeViewModel employeeViewModel, ApplicationDbContext context)
            => context.EmployeeContacts.AsNoTracking().Where(empId => empId.EmployeeId == employeeViewModel.Id).ToList();

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
        #endregion
    }
}
