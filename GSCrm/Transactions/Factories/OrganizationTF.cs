using GSCrm.Models.ViewModels;
using System;
using GSCrm.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using GSCrm.Models;
using GSCrm.Repository;

namespace GSCrm.Transactions.Factories
{
    public class OrganizationTF : TransactionFactory<OrganizationViewModel>
    {
        public OrganizationTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void CloseHandler(TransactionStatus transactionStatus, OperationType operationType)
        {
            // TODO Сделать логику удаления организации(чистка данных в бд)
            if (transactionStatus == TransactionStatus.Success)
            {
                switch (operationType)
                {
                    case OperationType.Delete:
                        //RemoveNotificationsFromOrg();
                        break;
                }
            }
        }

        /// <summary>
        /// Метод удаляет все уведомления, высланные пользователям от лица удаленной организации
        /// </summary>
        private void RemoveNotificationsFromOrg()
        {
            Organization removedOrganization = (Organization)transaction.GetParameterValue("RecordToRemove");
            InboxNotificationRepository inboxNotRepository = new InboxNotificationRepository(serviceProvider, context);
            context.InboxNotifications.AsNoTracking()
                .Where(not => not.SourceId == removedOrganization.Id.ToString())
                .ToList().ForEach(organization => inboxNotRepository.TryDelete(organization));
        }
    }
}
