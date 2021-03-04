using System;
using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Notifications.Params;
using GSCrm.Notifications.Auxiliary;
using GSCrm.Notifications.Params.AccUpdate;
using GSCrm.Notifications.Factories.OrgNotFactories;
using GSCrm.Notifications.Factories.OrgNotFactories.AccUpdate;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace GSCrm.Transactions.Factories
{
    public class AccountTF : TransactionFactory<AccountViewModel>
    {
        public AccountTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void CreateHandler(OperationType operationType, AccountViewModel accountViewModel)
        {
            if (operationType.IsInList(OperationType.Update, OperationType.ChangeAccountPrimaryContact, OperationType.UnlockAccount))
                transaction.RememberAccountCommonParams(cachService, context, currentUser);
        }

        protected override void CreateHandler(OperationType operationType, string recordId)
            => transaction.RememberAccountCommonParams(cachService, context, currentUser);

        protected override void CreateHandler(OperationType operationType)
            => transaction.RememberAccountCommonParams(cachService, context, currentUser);

        protected override void BeforeCommit(OperationType operationType)
        {
            if (operationType == OperationType.Delete)
            {
                // Перед удалением клиента необходимо запомнить всю команду по клиенту, чтобы после успешного удаления разослать им уведомления
                Account account = (Account)transaction.GetParameterValue("RecordToRemove");
                List<Employee> managers = account.GetManagers(context);
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
                    case OperationType.Create:
                        SendAddToTeamNotifications();
                        break;
                    case OperationType.Delete:
                        SendAccDeleteNotifications(ownerOrg);
                        break;
                    case OperationType.ChangeAccountPrimaryContact:
                    case OperationType.Update:
                        SendAccUpdateNotifications(ownerOrg);
                        break;
                }
            }
        }

        /// <summary>
        /// Метод рассылает уведомления команде созданного клиента
        /// </summary>
        private void SendAddToTeamNotifications()
        {
            Account account = (Account)transaction.GetParameterValue("NewRecord");
            Organization ownerOrg = context.Organizations.AsNoTracking().FirstOrDefault(org => org.Id == account.OrganizationId);
            List<Employee> managers = context.AccountManagers.AsNoTracking()
                .Include(accMan => accMan.Manager)
                .Where(accMan => accMan.AccountId == account.Id)
                .Select(accMan => accMan.Manager).ToList();

            AccTeamManagementParams accTeamManagementParams = new AccTeamManagementParams()
            {
                OwnerOrg = ownerOrg,
                Account = account,
                AccTeamManagementNotType = AccTeamManagementNotType.AddedToNew
            };
            AccTeamManagementNotFactory accTeamManagementNotFactory = new AccTeamManagementNotFactory(serviceProvider, context, accTeamManagementParams);
            accTeamManagementNotFactory.Send(ownerOrg.Id, managers);
        }

        /// <summary>
        /// Метод рассылает уведомления при удалении клиента
        /// </summary>
        /// <param name="ownerOrg"></param>
        private void SendAccDeleteNotifications(Organization ownerOrg)
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

        /// <summary>
        /// Метод рассылает уведомления при изменении данных клиента
        /// </summary>
        /// <param name="ownerOrg"></param>
        private void SendAccUpdateNotifications(Organization ownerOrg)
        {
            Account account = cachService.GetCachedCurrentEntity<Account>(currentUser);
            List<Employee> managers = account.GetManagers(context);
            if (managers.Count > 0)
            {
                BaseUpdateParams baseUpdateParams = new BaseUpdateParams()
                {
                    OwnerOrg = ownerOrg,
                    ChangedAccount = account
                };
                BaseUpdateNotFactory baseUpdateNotFactory = new BaseUpdateNotFactory(serviceProvider, context, baseUpdateParams);
                baseUpdateNotFactory.Send(ownerOrg.Id, managers);
            }
        }
    }
}
