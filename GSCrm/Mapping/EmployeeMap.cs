using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using GSCrm.Models.ViewTypes;
using GSCrm.Transactions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;

namespace GSCrm.Mapping
{
    public class EmployeeMap : BaseMap<Employee, EmployeeViewModel>
    {
        public EmployeeMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base (serviceProvider, context)
        { }

        public override Employee OnModelCreate(EmployeeViewModel employeeViewModel)
        {
            base.OnModelCreate(employeeViewModel);
            Division division = (Division)transaction.GetParameterValue("Division");
            Position primaryPosition = (Position)transaction.GetParameterValue("PrimaryPosition");
            User userAccount = (User)transaction.GetParameterValue("UserAccount");

            Employee employee = new Employee()
            {
                FirstName = employeeViewModel.FirstName,
                LastName = employeeViewModel.LastName,
                MiddleName = employeeViewModel.MiddleName,
                PrimaryPositionId = primaryPosition.Id,
                DivisionId = division.Id,
                OrganizationId = division.OrganizationId,
                UserId = Guid.Parse(userAccount.Id)
            };

            if (employeeViewModel.UserAccountExists)
                employee.EmployeeStatus = EmployeeStatus.AwaitingInvitationAcceptance;
            else employee.EmployeeStatus = EmployeeStatus.Active;

            EmployeePosition employeePosition = new EmployeePosition()
            {
                Employee = employee,
                EmployeeId = employee.Id,
                Position = primaryPosition,
                PositionId = primaryPosition.Id
            };

            transaction.AddChange(division, EntityState.Unchanged);
            transaction.AddChange(employeePosition, EntityState.Added);
            return employee;
        }

        public override Employee OnModelUpdate(EmployeeViewModel employeeViewModel)
        {
            Employee oldEmployee = base.OnModelUpdate(employeeViewModel);
            oldEmployee.FirstName = employeeViewModel.FirstName;
            oldEmployee.LastName = employeeViewModel.LastName;
            oldEmployee.MiddleName = employeeViewModel.MiddleName;
            return oldEmployee;
        }

        public override EmployeeViewModel DataToViewModel(Employee employee)
        {
            Position primaryPosition = employee.GetPrimaryPosition(context);
            Employee supervisor = employee.GetSupervisor(context);
            Division division = employee.GetDivision(context);
            Organization organization = context.Organizations.AsNoTracking().FirstOrDefault(org => org.Id == employee.OrganizationId);
            return new EmployeeViewModel()
            {
                Id = employee.Id,
                OrganizationId = organization.Id,
                OrganizationName = organization.Name,
                DivisionId = division?.Id,
                DivisionName = division?.Name,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                MiddleName = employee.MiddleName,
                FullName = employee.GetFullName(),
                FullInitialName = employee.GetIntialsFullName(),
                PrimaryPositionId = primaryPosition?.Id,
                PrimaryPositionName = primaryPosition?.Name,
                EmployeeStatus = employee.EmployeeStatus,
                EmployeeLockReason = employee.EmployeeLockReason,
                SupervisorId = supervisor?.Id.ToString(),
                SupervisorInitialName = supervisor?.GetIntialsFullName()
            };
        }
        
        /// <summary>
         /// Метод обновляет данные сотрудника из кеша
         /// </summary>
         /// <param name="employeeViewModel"></param>
         /// <param name="employeeViewTypes"></param>
         /// <returns></returns>
        public EmployeeViewModel Refresh(EmployeeViewModel employeeViewModel, User currentUser, params EmployeeViewType[] employeeViewTypes)
        {
            employeeViewTypes.ToList().ForEach(employeeViewType =>
            {
                switch (employeeViewType)
                {
                    // Восстановление данных поиска по должностям
                    case EmployeeViewType.EMP_POSITIONS:
                        EmployeeViewModel employeePosCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, EMP_POSITIONS);
                        employeeViewModel.SearchPosName = employeePosCash.SearchPosName;
                        employeeViewModel.SearchParentPosName = employeePosCash.SearchParentPosName;
                        break;

                    // Восстановление данных поиска по контактам
                    case EmployeeViewType.EMP_CONTACTS:
                        EmployeeViewModel employeeContactCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, EMP_CONTACTS);
                        employeeViewModel.SearchContactType = employeeContactCash.SearchContactType;
                        employeeViewModel.SearchContactPhone = employeeContactCash.SearchContactPhone;
                        employeeViewModel.SearchContactEmail = employeeContactCash.SearchContactEmail;
                        break;

                    // Восстановление данных поиска по подчиненным
                    case EmployeeViewType.EMP_SUBS:
                        EmployeeViewModel employeeSubCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, EMP_SUBS);
                        employeeViewModel.SearchSubordinateFullName = employeeSubCash.SearchSubordinateFullName;
                        break;

                    // Восстановление данных поиска по всем должностям организации для модального окна управления должностями
                    case EmployeeViewType.ALL_EMP_POSS:
                        EmployeeViewModel allEmployeePossCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, ALL_EMP_POSS);
                        employeeViewModel.SearchAllPosName = allEmployeePossCash.SearchAllPosName;
                        employeeViewModel.SearchAllParentPosName = allEmployeePossCash.SearchAllParentPosName;
                        break;

                    // Восстановление данных поиска по выбранным должностям сотрудника для модального окна управления должностями
                    case EmployeeViewType.SELECTED_EMP_POSS:
                        EmployeeViewModel selectedEmployeePossCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, SELECTED_EMP_POSS);
                        employeeViewModel.SearchSelectedPosName = selectedEmployeePossCash.SearchSelectedPosName;
                        employeeViewModel.SearchSelectedParentPosName = selectedEmployeePossCash.SearchSelectedParentPosName;
                        break;

                    // Восстановление данных поиска по всем полномочиям организации для модального окна управления полномочиями
                    case EmployeeViewType.ALL_EMP_RESPS:
                        EmployeeViewModel allEmployeeRespsCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, ALL_EMP_RESPS);
                        employeeViewModel.SearchAllRespName = allEmployeeRespsCash.SearchAllRespName;
                        break;

                    // Восстановление данных поиска по выбранным полномочиям сотрудника для модального окна управления полномочиями
                    case EmployeeViewType.SELECTED_EMP_RESPS:
                        EmployeeViewModel selectedEmployeeRespsCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, SELECTED_EMP_RESPS);
                        employeeViewModel.SearchSelectedRespName = selectedEmployeeRespsCash.SearchSelectedRespName;
                        break;

                    default:
                        break;
                }
            });

            return employeeViewModel;
        }

        /// <summary>
        /// Разблокировка сотрудника в случае блокировки из-за выхода сотрудника из организации
        /// </summary>
        public void UnlockEmployeeOnUserAccountAbsent(bool userAccountExists)
        {
            // Проставление сотруднику нового подразделения и должности
            SetTransaction(OperationType.UnlockEmployee);
            User user = (User)transaction.GetParameterValue("UserAccount");
            Employee employee = (Employee)transaction.GetParameterValue("Employee");
            employee.UserId = Guid.Parse(user.Id);
            if (userAccountExists)
                employee.EmployeeStatus = EmployeeStatus.AwaitingInvitationAcceptance;
            else employee.Unlock();
            transaction.AddChange(employee, EntityState.Modified);
        }

        /// <summary>
        /// Разблокировка сотрудника в случае блокировки из-за отсутствия должности
        /// </summary>
        public void UnlockEmployeeOnPositionAbsent()
        {
            // Проставление сотруднику нового подразделения и должности
            SetTransaction(OperationType.UnlockEmployee);
            Division newDivision = (Division)transaction.GetParameterValue("Division");
            Employee employee = (Employee)transaction.GetParameterValue("Employee");
            Position newPosition = (Position)transaction.GetParameterValue("PrimaryPosition");
            if (!employee.GetPositions(context).Select(posId => posId.PositionId).Contains(newPosition.Id))
                AddEmployeePosition(employee, newPosition);
            employee.DivisionId = newDivision.Id;
            employee.PrimaryPositionId = newPosition.Id;
            employee.Unlock();
            transaction.AddChange(employee, EntityState.Modified);
        }

        public void ChangeDivision(EmployeeViewModel employeeViewModel)
        {
            SetTransaction(OperationType.ChangeEmployeeDivision);
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
            List<Division> allDivisions = (List<Division>)transaction.GetParameterValue("AllDivisions");
            Division newDivision = allDivisions.FirstOrDefault(n => n.Name == employeeViewModel.DivisionName);
            Position newPrimaryPosition = (Position)transaction.GetParameterValue("PrimaryPosition");
            Employee employee = (Employee)transaction.GetParameterValue("Employee");
            RemoveOldEmployeePositions(employee);
            AddEmployeePosition(employee, newPrimaryPosition);
            employee.DivisionId = newDivision.Id;
            employee.PrimaryPositionId = newPrimaryPosition.Id;
            transaction.AddChange(employee, EntityState.Modified);
        }

        /// <summary>
        /// Метод создает и добавляет новую должность в список должностей сотрудника
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="newPrimaryPosition"></param>
        private void AddEmployeePosition(Employee employee, Position newPrimaryPosition)
        {
            EmployeePosition employeePosition = new EmployeePosition()
            {
                Id = Guid.NewGuid(),
                Employee = employee,
                EmployeeId = employee.Id,
                Position = newPrimaryPosition,
                PositionId = newPrimaryPosition.Id
            };
            transaction.AddChange(newPrimaryPosition, EntityState.Unchanged);
            transaction.AddChange(employeePosition, EntityState.Added);
        }

        /// <summary>
        /// Удаление из контекста всех должностей сотрудника
        /// </summary>
        /// <param name="employee"></param>
        private void RemoveOldEmployeePositions(Employee employee)
        {
            employee.EmployeePositions.ForEach(employeePosition =>
            {
                transaction.AddChange(employeePosition, EntityState.Deleted);
            });
        }
    }
}
