using System;
using System.Linq;
using System.Collections.Generic;
using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using GSCrm.Transactions;
using Microsoft.EntityFrameworkCore;

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

        /* TO DELETE
         * public EmployeeViewModel Refresh(EmployeeViewModel employeeViewModel, User currentUser, params EmployeeViewType[] employeeViewTypes)
        {
            employeeViewTypes.ToList().ForEach(employeeViewType =>
            {
                switch (employeeViewType)
                {
                    // Восстановление данных поиска по должностям
                    case EmployeeViewType.EMP_POSITIONS:
                        if (cachService.TryGetEntityCacheObsolete(currentUser, out EmployeeViewModel employeePosCash, EMP_POSITIONS))
                        {
                            employeeViewModel.SearchPosName = employeePosCash.SearchPosName;
                            employeeViewModel.SearchParentPosName = employeePosCash.SearchParentPosName;
                        }
                        break;

                    // Восстановление данных поиска по контактам
                    case EmployeeViewType.EMP_CONTACTS:
                        if (cachService.TryGetEntityCacheObsolete(currentUser, out EmployeeViewModel employeeContactCash, EMP_CONTACTS))
                        {
                            employeeViewModel.SearchContactType = employeeContactCash.SearchContactType;
                            employeeViewModel.SearchContactPhone = employeeContactCash.SearchContactPhone;
                            employeeViewModel.SearchContactEmail = employeeContactCash.SearchContactEmail;
                        }
                        break;

                    // Восстановление данных поиска по подчиненным
                    case EmployeeViewType.EMP_SUBS:
                        if (cachService.TryGetEntityCacheObsolete(currentUser, out EmployeeViewModel employeeSubCash, EMP_SUBS))
                            employeeViewModel.SearchSubordinateFullName = employeeSubCash.SearchSubordinateFullName;
                        break;

                    // Восстановление данных поиска по всем должностям организации для модального окна управления должностями
                    case EmployeeViewType.ALL_EMP_POSS:
                        if (cachService.TryGetEntityCacheObsolete(currentUser, out EmployeeViewModel allEmployeePossCash, ALL_EMP_POSS))
                        {
                            employeeViewModel.SearchAllPosName = allEmployeePossCash.SearchAllPosName;
                            employeeViewModel.SearchAllParentPosName = allEmployeePossCash.SearchAllParentPosName;
                        }
                        break;

                    // Восстановление данных поиска по выбранным должностям сотрудника для модального окна управления должностями
                    case EmployeeViewType.SELECTED_EMP_POSS:
                        if (cachService.TryGetEntityCacheObsolete(currentUser, out EmployeeViewModel selectedEmployeePossCash, SELECTED_EMP_POSS))
                        {
                            employeeViewModel.SearchSelectedPosName = selectedEmployeePossCash.SearchSelectedPosName;
                            employeeViewModel.SearchSelectedParentPosName = selectedEmployeePossCash.SearchSelectedParentPosName;
                        }
                        break;

                    // Восстановление данных поиска по всем полномочиям организации для модального окна управления полномочиями
                    case EmployeeViewType.ALL_EMP_RESPS:
                        if (cachService.TryGetEntityCacheObsolete(currentUser, out EmployeeViewModel allEmployeeRespsCash, ALL_EMP_RESPS))
                            employeeViewModel.SearchAllRespName = allEmployeeRespsCash.SearchAllRespName;
                        break;

                    // Восстановление данных поиска по выбранным полномочиям сотрудника для модального окна управления полномочиями
                    case EmployeeViewType.SELECTED_EMP_RESPS:
                        if (cachService.TryGetEntityCacheObsolete(currentUser, out EmployeeViewModel selectedEmployeeRespsCash, SELECTED_EMP_RESPS))
                            employeeViewModel.SearchSelectedRespName = selectedEmployeeRespsCash.SearchSelectedRespName;
                        break;

                    default:
                        break;
                }
            });

            return employeeViewModel;
        }*/

        /// <summary>
        /// Разблокировка сотрудника в случае блокировки из-за отсутствия должности
        /// </summary>
        public void UnlockOnPositionAbsent()
        {
            SetTransaction(OperationType.UnlockEmployee);
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);
            BindDivisionAndPosition(employee);
            employee.Unlock();
            transaction.AddChange(employee, EntityState.Modified);
        }

        /// <summary>
        /// Разблокировка сотрудника в случае блокировки из-за выхода сотрудника из организации
        /// </summary>
        public void UnlockOnUserAccountAbsent(bool userAccountExists)
        {
            SetTransaction(OperationType.UnlockEmployee);
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);
            BindUserAccount(employee, userAccountExists);
            transaction.AddChange(employee, EntityState.Modified);
        }

        /// <summary>
        /// Разблокировка в случае, если уже заблокированный сотрудник покинул организацию
        /// </summary>
        public void UnlockOnLockedEmployeeLeftOrg(bool userAccountExists)
        {
            SetTransaction(OperationType.UnlockEmployee);
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);
            BindDivisionAndPosition(employee);
            BindUserAccount(employee, userAccountExists);
            transaction.AddChange(employee, EntityState.Modified);
        }

        /// <summary>
        /// Проставление сотруднику нового подразделения и должности
        /// </summary>
        /// <param name="employee"></param>
        private void BindDivisionAndPosition(Employee employee)
        {
            Division newDivision = (Division)transaction.GetParameterValue("Division");
            Position newPosition = (Position)transaction.GetParameterValue("PrimaryPosition");
            if (!employee.GetPositions(context).Select(posId => posId.PositionId).Contains(newPosition.Id))
                AddEmployeePosition(employee, newPosition);
            employee.DivisionId = newDivision.Id;
            employee.PrimaryPositionId = newPosition.Id;
        }

        /// <summary>
        /// Привязка аккаунта пользователя к сотруднику
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="userAccountExists"></param>
        private void BindUserAccount(Employee employee, bool userAccountExists)
        {
            User user = (User)transaction.GetParameterValue("UserAccount");
            employee.UserId = Guid.Parse(user.Id);
            if (userAccountExists)
                employee.EmployeeStatus = EmployeeStatus.AwaitingInvitationAcceptance;
            else employee.Unlock();
        }

        public void ChangeDivision(EmployeeViewModel employeeViewModel)
        {
            SetTransaction(OperationType.ChangeEmployeeDivision);
            Organization currentOrganization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);
            List<Division> allDivisions = (List<Division>)transaction.GetParameterValue("AllDivisions");
            Division newDivision = allDivisions.FirstOrDefault(n => n.Name == employeeViewModel.DivisionName);
            Position newPrimaryPosition = (Position)transaction.GetParameterValue("PrimaryPosition");
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
            employee.AddEmployeePositions(context).EmployeePositions.ForEach(employeePosition =>
            {
                transaction.AddChange(employeePosition, EntityState.Deleted);
            });
        }
    }
}
