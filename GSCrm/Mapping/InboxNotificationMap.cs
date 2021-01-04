using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Notifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GSCrm.Mapping
{
    public class InboxNotificationMap : BaseMap<InboxNotification, InboxNotificationViewModel>
    {
        public InboxNotificationMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override InboxNotificationViewModel DataToViewModel(InboxNotification inboxNot)
        {
            Organization inviteOrg = null;
            if (inboxNot.NotificationType == NotificationType.OrgInvite)
                inviteOrg = context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == Guid.Parse(inboxNot.SourceId));
            return new InboxNotificationViewModel()
            {
                Id = inboxNot.Id,
                NotificationSource = inboxNot.NotificationSource,
                ActionType = inboxNot.ActionType,
                Content = inboxNot.Content,
                HasRead = inboxNot.HasRead,
                NotificationType = inboxNot.NotificationType,
                InviteOrg = inviteOrg
            };
        }
    }
}
