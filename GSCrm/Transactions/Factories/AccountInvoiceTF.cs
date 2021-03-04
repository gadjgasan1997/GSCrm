using System;
using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System.Collections.Generic;
using GSCrm.Notifications.Params.AccUpdate;
using GSCrm.Notifications.Factories.OrgNotFactories.AccUpdate;

namespace GSCrm.Transactions.Factories
{
    public class AccountInvoiceTF : TransactionFactory<AccountInvoiceViewModel>
    {
        public AccountInvoiceTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void CreateHandler(OperationType operationType, AccountInvoiceViewModel invoiceViewModel)
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
        /// Метод рассылает уведомления при добавлении банковских реквизитов
        /// </summary>
        /// <param name="currentAccount"></param>
        /// <param name="ownerOrg"></param>
        /// <param name="managers"></param>
        private void SendCreateNotifications(Account currentAccount, Organization ownerOrg, List<Employee> managers)
        {
            if (managers.Count > 0)
            {
                AddInvoiceParams addInvoiceParams = new AddInvoiceParams()
                {
                    OwnerOrg = ownerOrg,
                    ChangedAccount = currentAccount,
                    NewAccountInvoice = (AccountInvoice)transaction.GetParameterValue("NewRecord")
                };
                AddInvoiceNotFactory addInvoiceNotFactory = new AddInvoiceNotFactory(serviceProvider, context, addInvoiceParams);
                addInvoiceNotFactory.Send(ownerOrg.Id, managers);
            }
        }

        /// <summary>
        /// Метод рассылает уведомления при изменении банковских реквизитов
        /// </summary>
        /// <param name="currentAccount"></param>
        /// <param name="ownerOrg"></param>
        /// <param name="managers"></param>
        private void SendUpdateNotifications(Account currentAccount, Organization ownerOrg, List<Employee> managers)
        {
            if (managers.Count > 0)
            {
                AccountInvoice invoiceBeforeChange = (AccountInvoice)transaction.GetParameterValue("RecordToChange");
                AccountInvoice invoiceAfterChange = (AccountInvoice)transaction.GetParameterValue("ChangedRecord");
                UpdateInvoiceParams updateInvoiceParams = new UpdateInvoiceParams()
                {
                    OwnerOrg = ownerOrg,
                    ChangedAccount = currentAccount,
                    OldAccountInvoice = invoiceBeforeChange,
                    NewAccountInvoice = invoiceAfterChange
                };
                UpdateInvoiceNotFactory updateInvoiceNotFactory = new UpdateInvoiceNotFactory(serviceProvider, context, updateInvoiceParams);
                updateInvoiceNotFactory.Send(ownerOrg.Id, managers);
            }
        }

        /// <summary>
        /// Метод рассылает уведомления при удалении банковских реквизитов
        /// </summary>
        /// <param name="currentAccount"></param>
        /// <param name="ownerOrg"></param>
        /// <param name="managers"></param>
        private void SendDeleteNotifications(Account currentAccount, Organization ownerOrg, List<Employee> managers)
        {
            if (managers.Count > 0)
            {
                DeleteInvoiceParams deleteInvoiceParams = new DeleteInvoiceParams()
                {
                    OwnerOrg = ownerOrg,
                    ChangedAccount = currentAccount,
                    RemovedInvoice = (AccountInvoice)transaction.GetParameterValue("RecordToRemove")
                };
                DeleteInvoiceNotFactory deleteInvoiceNotFactory = new DeleteInvoiceNotFactory(serviceProvider, context, deleteInvoiceParams);
                deleteInvoiceNotFactory.Send(ownerOrg.Id, managers);
            }
        }
    }
}
