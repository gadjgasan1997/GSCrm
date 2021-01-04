using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using System.Collections.Generic;
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
    }
}
