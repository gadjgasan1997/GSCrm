using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Transactions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;

namespace GSCrm.Repository
{
    public class InboxNotificationRepository : BaseRepository<InboxNotification, InboxNotificationViewModel>
    {
        public InboxNotificationRepository(IServiceProvider serviceProvider, ApplicationDbContext context) : base (serviceProvider, context)
        { }

        #region Attach Notifications
        public void AttachNotifications(ref InboxNotificationsViewModel inboxNotsViewModel)
            => inboxNotsViewModel.InboxNotificationViewModels = context.GetInboxNotifications(currentUser).MapToViewModels(map, GetLimitedList);

        private List<InboxNotification> GetLimitedList(List<InboxNotification> inboxNots)
        {
            List<InboxNotification> limitesInboxNots = inboxNots;
            LimitListByPageNumber(INBOX_NOTS, ref limitesInboxNots);
            return limitesInboxNots;
        }
        #endregion

        #region Other Methods
        public void SetHasReedFlag(string id, bool hasReed)
        {
            List<InboxNotification> inboxNotifications = context.GetInboxNotifications(currentUser);
            InboxNotification inboxNotification = inboxNotifications.FirstOrDefault(i => i.Id == Guid.Parse(id));
            if (inboxNotification != null)
            {
                transaction = dataModelsTransactionFactory.Create(currentUser.Id, OperationType.Update, inboxNotification);
                inboxNotification.HasRead = hasReed;
                transaction.AddChange(inboxNotification, EntityState.Modified);
                if (dataModelsTransactionFactory.TryCommit(transaction, errors))
                    dataModelsTransactionFactory.Close(transaction);
                else dataModelsTransactionFactory.Close(transaction, TransactionStatus.Error);
            }    
        }
        #endregion
    }
}
