using System;
using System.Linq;
using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using GSCrm.Notifications.Params.EmpUpdate;
using GSCrm.Notifications.Factories.OrgNotFactories.EmpUpdate;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GSCrm.Transactions.Factories
{
    public class EmployeeContactTF : TransactionFactory<EmployeeContactViewModel>
    {
        public EmployeeContactTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void CreateHandler(OperationType operationType, EmployeeContactViewModel contactViewModel)
        {
            if (operationType.IsInList(baseOperationTypes))
            {
                Organization currentOrganization = cachService.GetMainEntity(currentUser, MainEntityType.OrganizationData) as Organization;
                transaction.AddParameter("CurrentOrganization", currentOrganization);
                Employee employee = context.Employees.AsNoTracking().FirstOrDefault(emp => emp.Id == Guid.Parse(contactViewModel.EmployeeId));
                transaction.AddParameter("Employee", employee);
            }
        }

        protected override void CreateHandler(OperationType operationType, string recordId)
        {
            Organization currentOrganization = cachService.GetMainEntity(currentUser, MainEntityType.OrganizationData) as Organization;
            transaction.AddParameter("CurrentOrganization", currentOrganization);
        }

        protected override void CloseHandler(TransactionStatus transactionStatus, OperationType operationType)
        {
            if (transactionStatus == TransactionStatus.Success)
            {
                Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
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
            Employee employee = (Employee)transaction.GetParameterValue("Employee");
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
            Employee employee = (Employee)transaction.GetParameterValue("Employee");
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
            EmployeeContact employeeContact = (EmployeeContact)transaction.GetParameterValue("RecordToRemove");
            Employee employee = context.Employees.AsNoTracking().FirstOrDefault(emp => emp.Id == employeeContact.EmployeeId);
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
