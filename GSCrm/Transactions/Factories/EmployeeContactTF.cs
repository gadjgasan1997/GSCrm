using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Notifications.Params.EmpUpdate;
using GSCrm.Notifications.Factories.OrgNotFactories.EmpUpdate;
using System.Collections.Generic;

namespace GSCrm.Transactions.Factories
{
    public class EmployeeContactTF : TransactionFactory<EmployeeContactViewModel>
    {
        public EmployeeContactTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void CloseHandler(TransactionStatus transactionStatus, OperationType operationType)
        {
            if (transactionStatus == TransactionStatus.Success)
            {
                Organization currentOrganization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
                switch (operationType)
                {
                    case OperationType.Create:
                        SendContactCreateNotifications(currentOrganization);
                        break;
                    case OperationType.Update:
                        SendContactUpdateNotifications(currentOrganization);
                        break;
                    case OperationType.Delete:
                        SendContactDeleteNotifications(currentOrganization);
                        break;
                }
            }
        }

        /// <summary>
        /// Метод рассылает уведомления при создании контакта пользователя
        /// </summary>
        /// <param name="currentOrganization"></param>
        private void SendContactCreateNotifications(Organization currentOrganization)
        {
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);
            EmployeeContact employeeContact = (EmployeeContact)transaction.GetParameterValue("NewRecord");
            AddContactParams addContactParams = new AddContactParams()
            {
                ChangedEmployee = employee,
                NewEmployeeContact = employeeContact,
                Organization = currentOrganization
            };
            AddContactNotFactory addContactNotFactory = new AddContactNotFactory(serviceProvider, context, addContactParams);
            addContactNotFactory.Send(currentOrganization.Id, new List<Employee>() { employee });
        }

        /// <summary>
        /// Метод рассылает уведомления при изменении контакта пользователя
        /// </summary>
        /// <param name="currentOrganization"></param>
        private void SendContactUpdateNotifications(Organization currentOrganization)
        {
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);
            EmployeeContact contactBeforeChange = (EmployeeContact)transaction.GetParameterValue("RecordToChange");
            EmployeeContact contactAfterChange = (EmployeeContact)transaction.GetParameterValue("ChangedRecord");
            UpdateContactParams updateContactParams = new UpdateContactParams()
            {
                ChangedEmployee = employee,
                Organization = currentOrganization,
                OldEmployeeContact = contactBeforeChange,
                NewEmployeeContact = contactAfterChange
            };
            UpdateContactNotFactory updateContactNotFactory = new UpdateContactNotFactory(serviceProvider, context, updateContactParams);
            updateContactNotFactory.Send(currentOrganization.Id, new List<Employee>() { employee });
        }

        /// <summary>
        /// Метод рассылает уведомления при удалении контакта пользователя
        /// </summary>
        /// <param name="currentOrganization"></param>
        private void SendContactDeleteNotifications(Organization currentOrganization)
        {
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);
            DeleteContactParams deleteContactParams = new DeleteContactParams()
            {
                ChangedEmployee = employee,
                Organization = currentOrganization
            };
            DeleteContactNotFactory deleteContactNotFactory = new DeleteContactNotFactory(serviceProvider, context, deleteContactParams);
            deleteContactNotFactory.Send(currentOrganization.Id, new List<Employee>() { employee });
        }
    }
}
