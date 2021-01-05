using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Notifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GSCrm.Mapping
{
    public class UserNotificationMap : BaseMap<UserNotification, UserNotificationViewModel>
    {
        public UserNotificationMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override UserNotificationViewModel DataToViewModel(UserNotification userNot)
        {
            InboxNotification inboxNot = context.InboxNotifications.AsNoTracking().FirstOrDefault(i => i.Id == userNot.NotificationId);
            Organization inviteOrg = null;
            if (inboxNot.NotificationType == NotificationType.OrgInvite)
                inviteOrg = context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == Guid.Parse(inboxNot.SourceId));
            return new UserNotificationViewModel()
            {
                Id = userNot.Id,
                NotificationId = inboxNot.Id,
                NotificationSource = inboxNot.NotificationSource,
                ActionType = inboxNot.ActionType,
                Content = inboxNot.Content,
                HasRead = userNot.HasRead,
                NotificationType = inboxNot.NotificationType,
                InviteOrg = inviteOrg
            };
        }
    }
}
