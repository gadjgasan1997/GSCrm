using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using GSCrm.Notifications.Params;
using GSCrm.Notifications.Factories.OrgNotFactories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;
using Microsoft.AspNetCore.Mvc;

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
            Organization currentOrganization = default;
            if (operationType.IsInList(baseOperationTypes.With(OperationType.UnlockEmployee, OperationType.ChangeEmployeeDivision)))
            {
                currentOrganization = cachService.GetMainEntity(currentUser, MainEntityType.OrganizationData) as Organization;
                transaction.AddParameter("CurrentOrganization", currentOrganization);
            }
            if (operationType.IsInList(OperationType.UnlockEmployee, OperationType.ChangeEmployeeDivision))
            {
                List<Division> allDivisions = currentOrganization.GetDivisions(context);
                transaction.AddParameter("AllDivisions", allDivisions);
                Employee employee = context.Employees.AsNoTracking().FirstOrDefault(i => i.Id == employeeViewModel.Id);
                transaction.AddParameter("Employee", employee);
            }
        }

        protected override void CreateHandler(OperationType operationType, string recordId)
        {
            Organization currentOrganization = cachService.GetMainEntity(currentUser, MainEntityType.OrganizationData) as Organization;
            transaction.AddParameter("CurrentOrganization", currentOrganization);
        }

        protected override void BeforeCommit(OperationType operationType)
        {
            // Перед удалением сотрудника необходимо запомнить его настройки уведомлений для организации, так как они будут удалены в момент отправки уведомления
            if (operationType == OperationType.Delete)
            {
                Employee employee = (Employee)transaction.GetParameterValue("RecordToRemove");
                Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
                Func<UserOrganization, bool> predicate = userOrg => userOrg.UserId == employee.UserId.ToString() && userOrg.OrganizationId == currentOrganization.Id;
                UserOrganization userOrganization = context.UserOrganizations.AsNoTracking().Include(userOrg => userOrg.OrgNotificationsSetting).FirstOrDefault(predicate);
                transaction.AddParameter("OrgNotificationsSetting", userOrganization?.OrgNotificationsSetting);
            }
        }

        protected override void CloseHandler(TransactionStatus transactionStatus, OperationType operationType)
        {
            if (transactionStatus == TransactionStatus.Error)
            {
                User user = transaction.GetParameterValue("UserAccount") as User;
                if (user != null) userManager.DeleteAsync(user);
            }
            else if (transactionStatus == TransactionStatus.Success)
            {
                switch (operationType)
                {
                    case OperationType.Delete:
                        SendEmpDeleteNotifications();
                        break;
                }
            }
        }

        /// <summary>
        /// Метод рассылает уведомления при удалении сотрудника
        /// </summary>
        private void SendEmpDeleteNotifications()
        {
            // TODO Сделать логику удаления уведомлений от лица организации удаленному сотруднику(чистка данных в бд)
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
            Employee employee = (Employee)transaction.GetParameterValue("RecordToRemove");
            EmpDeleteParams empDeleteParams = new EmpDeleteParams()
            {
                Organization = currentOrganization,
                OrganizationUrl = urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = currentOrganization.Id }, httpContext.Request.Scheme),
                RemovedEmployee = employee
            };
            EmpDeleteNotFactory empDeleteNotFactory = new EmpDeleteNotFactory(serviceProvider, context, empDeleteParams);
            empDeleteNotFactory.Send(transaction, employee.UserId);
        }
    }
}
