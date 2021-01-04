using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.Enums;
using GSCrm.Notifications.Params;
using System;
using System.Collections.Generic;
using System.Text;

namespace GSCrm.Notifications.Factories.UserNotFactories0
{
    public class OrgInviteNotFactory : UserNotificationFactory<OrgInviteParams>
    {
        public OrgInviteNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, OrgInviteParams orgInvoiceParams)
            : base(serviceProvider, context, orgInvoiceParams)
        { }

        protected override Notification Create(NotificationTarget notificationTarget)
            => notificationTarget switch
            {
                NotificationTarget.Email => new EmailNotification()
                {
                    Id = Guid.NewGuid(),
                    Content = GetEmailTemplate(),
                    Subject = resManager.GetString("OrgInviteInboxSubject"),
                    Header = resManager.GetString("OrgInviteInboxSubject"),
                    SourceId = notificationParams.Organization.Id.ToString(),
                    NotificationType = NotificationType.OrgInvite
                },
                NotificationTarget.Inbox => new InboxNotification()
                {
                    Id = Guid.NewGuid(),
                    ActionType = NotificationActionType.AllowDeniyHide,
                    Content = resManager.GetString("OrgInviteInboxContent").Replace("{orgName}", notificationParams.Organization.Name),
                    SourceId = notificationParams.Organization.Id.ToString(),
                    NotificationType = NotificationType.OrgInvite
                },
                _ => default
            };

        protected override List<NotificationTarget> GetNotificationTargets(UserNotificationsSetting userNotSetting)
            => new List<NotificationTarget>() { userNotSetting.TOrgInvoiceNot };

        protected override bool NeedNotification(UserNotificationsSetting userNotSetting)
            => userNotSetting.OrgInvoiceNot;

        protected override string GetEmailTemplate()
            => new StringBuilder()
            .Append($"<p>Вы получили приглашение вступить в организацию {notificationParams.Organization.Name}</p>")
            .Append($"<p>Чтобы принять или отклонить приглашение, перейдите по ссылке: <a href='{notificationParams.OrgInviteUrl}'>{notificationParams.Organization.Name}</a></p>")
            .ToString();
                
    }
}
