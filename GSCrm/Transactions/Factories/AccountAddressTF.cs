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
    public class AccountAddressTF : TransactionFactory<AccountAddressViewModel>
    {
        public AccountAddressTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void CreateHandler(OperationType operationType, AccountAddressViewModel addressViewModel)
        {
            if (operationType.IsInList(baseOperationTypes.With(OperationType.ChangeAccountLegalAddress)))
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
                    case OperationType.ChangeAccountLegalAddress:
                        SendChangeLegalAddressNotifications(currentAccount, ownerOrg, managers);
                        break;
                }
            }
        }

        /// <summary>
        /// Метод рассылает уведомления при создании адреса
        /// </summary>
        /// <param name="currentAccount"></param>
        /// <param name="ownerOrg"></param>
        /// <param name="managers"></param>
        private void SendCreateNotifications(Account currentAccount, Organization ownerOrg, List<Employee> managers)
        {
            if (managers.Count > 0)
            {
                AccountAddress newAccountAddress = (AccountAddress)transaction.GetParameterValue("NewRecord");
                AddAddressParams addAddressParams = new AddAddressParams()
                {
                    OwnerOrg = ownerOrg,
                    ChangedAccount = currentAccount,
                    NewAccountAddress = newAccountAddress
                };
                AddAddressNotFactory addAddressNotFactory = new AddAddressNotFactory(serviceProvider, context, addAddressParams);
                addAddressNotFactory.Send(ownerOrg.Id, managers);
            }
        }

        /// <summary>
        /// Метод рассылает уведомления при изменении адреса
        /// </summary>
        /// <param name="currentAccount"></param>
        /// <param name="ownerOrg"></param>
        /// <param name="managers"></param>
        private void SendUpdateNotifications(Account currentAccount, Organization ownerOrg, List<Employee> managers)
        {
            if (managers.Count > 0)
            {
                AccountAddress addressBeforeChange = (AccountAddress)transaction.GetParameterValue("RecordToChange");
                AccountAddress addressAfterChange = (AccountAddress)transaction.GetParameterValue("ChangedRecord");
                UpdateAddressParams updateAddressParams = new UpdateAddressParams()
                {
                    OwnerOrg = ownerOrg,
                    ChangedAccount = currentAccount,
                    OldAccountAddress = addressBeforeChange,
                    NewAccountAddress = addressAfterChange
                };
                UpdateAddressNotFactory updateAddressNotFactory = new UpdateAddressNotFactory(serviceProvider, context, updateAddressParams);
                updateAddressNotFactory.Send(ownerOrg.Id, managers);
            }
        }

        /// <summary>
        /// Метод рассылает уведомления при удалении адреса
        /// </summary>
        /// <param name="currentAccount"></param>
        /// <param name="ownerOrg"></param>
        /// <param name="managers"></param>
        private void SendDeleteNotifications(Account currentAccount, Organization ownerOrg, List<Employee> managers)
        {
            if (managers.Count > 0)
            {
                AccountAddress removedAddress = (AccountAddress)transaction.GetParameterValue("RecordToRemove");
                DeleteAddressParams deleteAddressParams = new DeleteAddressParams()
                {
                    OwnerOrg = ownerOrg,
                    ChangedAccount = currentAccount,
                    RemovedAddress = removedAddress
                };
                DeleteAddressNotFactory deleteAddressNotFactory = new DeleteAddressNotFactory(serviceProvider, context, deleteAddressParams);
                deleteAddressNotFactory.Send(ownerOrg.Id, managers);
            }
        }

        /// <summary>
        /// Метод рассылает уведомления при изменении юридического адреса клиента
        /// </summary>
        /// <param name="currentAccount"></param>
        /// <param name="ownerOrg"></param>
        /// <param name="managers"></param>
        private void SendChangeLegalAddressNotifications(Account currentAccount, Organization ownerOrg, List<Employee> managers)
        {
            if (managers.Count > 0)
            {
                BaseUpdateParams baseUpdateParams = new BaseUpdateParams()
                {
                    OwnerOrg = ownerOrg,
                    ChangedAccount = currentAccount
                };
                BaseUpdateNotFactory baseUpdateNotFactory = new BaseUpdateNotFactory(serviceProvider, context, baseUpdateParams);
                baseUpdateNotFactory.Send(ownerOrg.Id, managers);
            }
        }
    }
}
