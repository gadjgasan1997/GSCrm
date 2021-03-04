using System;
using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Notifications.Params.AccUpdate;
using GSCrm.Notifications.Factories.OrgNotFactories.AccUpdate;
using System.Collections.Generic;

namespace GSCrm.Transactions.Factories
{
    public class AccountContactTF : TransactionFactory<AccountContactViewModel>
    {
        public AccountContactTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void CreateHandler(OperationType operationType, AccountContactViewModel entity)
        {
            if (operationType.IsInList(baseOperationTypes))
                transaction.RememberAccountCommonParams(cachService, context, currentUser);
        }

        protected override void CreateHandler(OperationType operationType, string recordId)
        {
            if (operationType == OperationType.Delete)
                transaction.RememberAccountCommonParams(cachService, context, currentUser);
        }

        protected override void CloseHandler(TransactionStatus transactionStatus, OperationType operationType)
        {
            if (transactionStatus == TransactionStatus.Success)
            {
                Account currentAccount = cachService.GetCachedCurrentEntity<Account>(currentUser);
                Organization ownerOrg = (Organization)transaction.GetParameterValue("OwnerOrg");
                List<Employee> managers = currentAccount.GetManagers(context);
                switch (operationType)
                {
                    case OperationType.Create:
                        SendCreateNotifications(currentAccount, ownerOrg, managers);
                        break;
                    case OperationType.Update:
                        SendUpdateNotifications(currentAccount, ownerOrg, managers);
                        break;
                    case OperationType.Delete:
                        SendDeleteNotifications(currentAccount, ownerOrg, managers);
                        break;
                }
            }
        }

        /// <summary>
        /// Метод рассылает уведомления при создании контакта
        /// </summary>
        /// <param name="currentAccount"></param>
        /// <param name="ownerOrg"></param>
        /// <param name="managers"></param>
        private void SendCreateNotifications(Account currentAccount, Organization ownerOrg, List<Employee> managers)
        {
            if (managers.Count > 0)
            {
                AccountContact newAccountContact = (AccountContact)transaction.GetParameterValue("NewRecord");
                AddContactParams addContactParams = new AddContactParams()
                {
                    OwnerOrg = ownerOrg,
                    ChangedAccount = currentAccount,
                    NewAccountContact = newAccountContact
                };
                AddContactNotFactory addContactNotFactory = new AddContactNotFactory(serviceProvider, context, addContactParams);
                addContactNotFactory.Send(ownerOrg.Id, managers);
            }
        }

        /// <summary>
        /// Метод рассылает уведомления при изменении контакта
        /// </summary>
        /// <param name="currentAccount"></param>
        /// <param name="ownerOrg"></param>
        /// <param name="managers"></param>
        private void SendUpdateNotifications(Account currentAccount, Organization ownerOrg, List<Employee> managers)
        {
            if (managers.Count > 0)
            {
                AccountContact contactBeforeChange = (AccountContact)transaction.GetParameterValue("RecordToChange");
                AccountContact contactAfterChange = (AccountContact)transaction.GetParameterValue("ChangedRecord");
                UpdateContactParams updateContactParams = new UpdateContactParams()
                {
                    OwnerOrg = ownerOrg,
                    ChangedAccount = currentAccount,
                    OldAccountContact = contactBeforeChange,
                    NewAccountContact = contactAfterChange
                };
                UpdateContactNotFactory updateContactNotFactory = new UpdateContactNotFactory(serviceProvider, context, updateContactParams);
                updateContactNotFactory.Send(ownerOrg.Id, managers);
            }
        }

        /// <summary>
        /// Метод рассылает уведомления при удалении контакта
        /// </summary>
        /// <param name="currentAccount"></param>
        /// <param name="ownerOrg"></param>
        /// <param name="managers"></param>
        private void SendDeleteNotifications(Account currentAccount, Organization ownerOrg, List<Employee> managers)
        {
            if (managers.Count > 0)
            {
                AccountContact removedContact = (AccountContact)transaction.GetParameterValue("RecordToRemove");
                DeleteContactParams deleteContactParams = new DeleteContactParams()
                {
                    OwnerOrg = ownerOrg,
                    ChangedAccount = currentAccount,
                    RemovedContact = removedContact
                };
                DeleteContactNotFactory deleteContactNotFactory = new DeleteContactNotFactory(serviceProvider, context, deleteContactParams);
                deleteContactNotFactory.Send(ownerOrg.Id, managers);
            }
        }
    }
}
