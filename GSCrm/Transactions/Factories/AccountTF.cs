using System;
using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using GSCrm.Models.Enums;
using GSCrm.Notifications.Params;
using GSCrm.Notifications.Factories.OrgNotFactories;
using System.Collections.Generic;

namespace GSCrm.Transactions.Factories
{
    public class AccountTF : TransactionFactory<AccountViewModel>
    {
        public AccountTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void CreateHandler(OperationType operationType, AccountViewModel accountViewModel)
        {
            if (operationType.IsInList(OperationType.Update, OperationType.ChangeAccountPrimaryContact, OperationType.UnlockAccount))
            {
                Account currentAccount = cachService.GetMainEntity(currentUser, MainEntityType.AccountData) as Account;
                transaction.AddParameter("CurrentAccount", currentAccount);
            }
        }

        protected override void CreateHandler(OperationType operationType, string recordId)
        {
            if (operationType == OperationType.Delete)
            {
                Account currentAccount = cachService.GetMainEntity(currentUser, MainEntityType.AccountData) as Account;
                transaction.AddParameter("CurrentAccount", currentAccount);
                Organization ownerOrg = context.Organizations.AsNoTracking().FirstOrDefault(org => org.Id == currentAccount.OrganizationId);
                transaction.AddParameter("OwnerOrg", ownerOrg);
            }
        }

        protected override void BeforeCommit(OperationType operationType)
        {
            if (operationType == OperationType.Delete)
            {
                // Перед удалением клиента необходимо запомнить всю команду по клиенту, чтобы после успешного удаления разослать им уведомления
                Account account = (Account)transaction.GetParameterValue("RecordToRemove");
                List<Employee> managers = context.AccountManagers.AsNoTracking()
                    .Include(accMan => accMan.Manager)
                    .Where(accMan => accMan.AccountId == account.Id)
                    .Select(accMan => accMan.Manager).ToList();
                transaction.AddParameter("Managers", managers);
            }
        }

        protected override void CloseHandler(TransactionStatus transactionStatus, OperationType operationType)
        {
            if (transactionStatus == TransactionStatus.Success)
            {
                Organization ownerOrg = (Organization)transaction.GetParameterValue("OwnerOrg");
                switch (operationType)
                {
                    case OperationType.Delete:
                        SendAccDeletenotifications(ownerOrg);
                        break;
                }
            }
        }

        /// <summary>
        /// Метод рассылает уведомления при удалении клиента
        /// </summary>
        /// <param name="ownerOrg"></param>
        private void SendAccDeletenotifications(Organization ownerOrg)
        {
            List<Employee> managers = (List<Employee>)transaction.GetParameterValue("Managers");
            if (managers.Count > 0)
            {
                Account account = (Account)transaction.GetParameterValue("RecordToRemove");
                AccDeleteParams accDeleteParams = new AccDeleteParams()
                {
                    OwnerOrg = ownerOrg,
                    RemovedAccount = account
                };
                AccDeleteNotFactory accDeleteNotFactory = new AccDeleteNotFactory(serviceProvider, context, accDeleteParams);
                accDeleteNotFactory.Send(ownerOrg.Id, managers);
            }
        }
    }
}
