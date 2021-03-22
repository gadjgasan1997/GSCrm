﻿using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Notifications.Params;
using GSCrm.Notifications.Factories.OrgNotFactories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using GSCrm.Notifications.Factories.OrgNotFactories.EmpUpdate;
using GSCrm.Notifications.Params.EmpUpdate;

namespace GSCrm.Transactions.Factories
{
    public class EmployeeTF : TransactionFactory<EmployeeViewModel>
    {
        private readonly UserManager<User> userManager;
        public EmployeeTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        {
            userManager = serviceProvider.GetService(typeof(UserManager<User>)) as UserManager<User>;
        }

        protected override void CreateHandler(OperationType operationType, EmployeeViewModel employeeViewModel)
        {
            if (operationType.IsInList(OperationType.UnlockEmployee, OperationType.ChangeEmployeeDivision))
            {
                Organization currentOrganization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
                List<Division> allDivisions = currentOrganization.GetDivisions(context);
                transaction.AddParameter("AllDivisions", allDivisions);
            }
        }

        protected override void BeforeCommit(OperationType operationType)
        {
            // Перед удалением сотрудника необходимо запомнить его настройки уведомлений для организации, так как они будут удалены в момент отправки уведомления
            if (operationType == OperationType.Delete)
            {
                Employee employee = (Employee)transaction.GetParameterValue("RecordToRemove");
                Organization currentOrganization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
                Func<UserOrganization, bool> predicate = userOrg => userOrg.UserId == employee.UserId.ToString() && userOrg.OrganizationId == currentOrganization.Id;
                UserOrganization userOrganization = context.UserOrganizations.AsNoTracking().Include(userOrg => userOrg.OrgNotificationsSetting).FirstOrDefault(predicate);
                transaction.AddParameter("OrgNotificationsSetting", userOrganization?.OrgNotificationsSetting);
            }
        }

        protected override void CloseHandler(TransactionStatus transactionStatus, OperationType operationType)
        {
            switch (transactionStatus)
            {
                case TransactionStatus.Error:
                    if (transaction.GetParameterValue("UserAccount") is User user)
                        userManager.DeleteAsync(user);
                    break;
                case TransactionStatus.Success:
                    {
                        switch (operationType)
                        {
                            case OperationType.Update:
                                SendEmpUpdateNotifications();
                                break;
                            case OperationType.ChangeEmployeeDivision:
                                SendEmpChangeDivisionNotifications();
                                break;
                            case OperationType.Delete:
                                SendEmpDeleteNotifications();
                                break;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Метод рассылает уведомления при изменении данных сотрудника
        /// </summary>
        /// <param name="employee"></param>
        private void SendEmpUpdateNotifications()
        {
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);
            Organization currentOrganization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
            BaseUpdateParams baseUpdateParams = new BaseUpdateParams()
            {
                Organization = currentOrganization,
                ChangedEmployee = employee
            };

            BaseUpdateNotFactory baseUpdateNotFactory = new BaseUpdateNotFactory(serviceProvider, context, baseUpdateParams);
            baseUpdateNotFactory.Send(currentOrganization.Id, new List<Employee>() { employee });
        }

        /// <summary>
        /// Метод рассылает уведомления при переводе сотрудника в другое подразделение
        /// </summary>
        private void SendEmpChangeDivisionNotifications()
        {
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);
            Organization currentOrganization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
            ChangeDivisionParams changeDivisionParams = new ChangeDivisionParams()
            {
                Organization = currentOrganization,
                ChangedEmployee = employee,
                NewEmployeeDivision = (Division)transaction.GetParameterValue("Division"),
                NewEmployeePosition = (Position)transaction.GetParameterValue("PrimaryPosition")
            };

            ChangeDivisionNotFactory changeDivisionNotFactory = new ChangeDivisionNotFactory(serviceProvider, context, changeDivisionParams);
            changeDivisionNotFactory.Send(currentOrganization.Id, new List<Employee>() { employee });
        }

        /// <summary>
        /// Метод рассылает уведомления при удалении сотрудника
        /// </summary>
        private void SendEmpDeleteNotifications()
        {
            // TODO Сделать логику удаления уведомлений от лица организации удаленному сотруднику(чистка данных в бд)
            Organization currentOrganization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
            Employee employee = (Employee)transaction.GetParameterValue("RecordToRemove");
            EmpDeleteParams empDeleteParams = new EmpDeleteParams()
            {
                Organization = currentOrganization,
                RemovedEmployee = employee
            };
            EmpDeleteNotFactory empDeleteNotFactory = new EmpDeleteNotFactory(serviceProvider, context, empDeleteParams);
            empDeleteNotFactory.Send(transaction, employee.UserId);
        }
    }
}
