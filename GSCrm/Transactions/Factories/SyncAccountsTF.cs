using System;
using System.Collections.Generic;
using System.Linq;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Notifications.Params;
using GSCrm.Notifications.Auxiliary;
using GSCrm.Notifications.Factories.OrgNotFactories;
using Microsoft.EntityFrameworkCore;
using GSCrm.Models.Enums;

namespace GSCrm.Transactions.Factories
{
    public class SyncAccountsTF : TransactionFactory<SyncAccountViewModel>
    {
        public SyncAccountsTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void CreateHandler(OperationType operationType, SyncAccountViewModel entity)
        {
            if (operationType == OperationType.AccountTeamManagement)
            {
                Account currentAccount = cachService.GetMainEntity(currentUser, MainEntityType.AccountData) as Account;
                transaction.AddParameter("CurrentAccount", currentAccount);
            }
        }

        protected override void CloseHandler(TransactionStatus transactionStatus, OperationType operationType)
        {
            if (transactionStatus == TransactionStatus.Success)
            {
                if (operationType == OperationType.AccountTeamManagement)
                    SendNotifications();
            }
        }

        /// <summary>
        /// Метод рассылает уведомления пользователям при изменении команды по клиенту
        /// </summary>
        private void SendNotifications()
        {
            Account account = (Account)transaction.GetParameterValue("Account");
            Organization ownerOrg = context.Organizations.AsNoTracking().FirstOrDefault(org => org.Id == account.OrganizationId);
            Dictionary<Guid, AccTeamManagementNotType> managersNotTypes = (Dictionary<Guid, AccTeamManagementNotType>)transaction.GetParameterValue("ManagersNotTypes");

            // Для каждого менеджера необходимо формировать новое уведомление, так как его тип разный для пользователей
            managersNotTypes.Keys.ToList().ForEach(managerId =>
            {
                AccTeamManagementParams accTeamManagementParams = new AccTeamManagementParams()
                {
                    OwnerOrg = ownerOrg,
                    Account = account,
                    AccTeamManagementNotType = managersNotTypes[managerId]
                };
                AccTeamManagementNotFactory accTeamManagementNotFactory = new AccTeamManagementNotFactory(serviceProvider, context, accTeamManagementParams);
                accTeamManagementNotFactory.Send(ownerOrg.Id, new List<Guid>() { managerId });
            });
        }
    }
}
