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
using GSCrm.Notifications.Auxiliary;
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
            if (cachService.TryGetEntityCache(currentUser, out Organization currentOrganization))
            {
                if (operationType.IsInList(baseOperationTypes.With(OperationType.UnlockEmployee, OperationType.ChangeEmployeeDivision)))
                    transaction.AddParameter("CurrentOrganization", currentOrganization);
                if (operationType.IsInList(OperationType.UnlockEmployee, OperationType.ChangeEmployeeDivision))
                {
                    List<Division> allDivisions = currentOrganization.GetDivisions(context);
                    transaction.AddParameter("AllDivisions", allDivisions);
                    Employee employee = context.Employees.AsNoTracking().FirstOrDefault(i => i.Id == employeeViewModel.Id);
                    transaction.AddParameter("Employee", employee);
                }
            }
        }

        protected override void CreateHandler(OperationType operationType, string recordId)
        {
            if (cachService.TryGetEntityCache(currentUser, out Organization currentOrganization))
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
            switch (transactionStatus)
            {
                case TransactionStatus.Error:
                    {
                        User user = transaction.GetParameterValue("UserAccount") as User;
                        if (user != null) userManager.DeleteAsync(user);
                    }
                    break;
                case TransactionStatus.Success:
                    {
                        switch (operationType)
                        {
                            case OperationType.Update:
                                SendEmpUpdateNotifications((Employee)transaction.GetParameterValue("ChangedRecord"));
                                break;
                            case OperationType.ChangeEmployeeDivision:
                                SendEmpChangeDivisionNotifications((Employee)transaction.GetParameterValue("Employee"));
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
        private void SendEmpUpdateNotifications(Employee employee)
        {
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
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
        /// <param name="employee"></param>
        private void SendEmpChangeDivisionNotifications(Employee employee)
        {
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
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
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
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
