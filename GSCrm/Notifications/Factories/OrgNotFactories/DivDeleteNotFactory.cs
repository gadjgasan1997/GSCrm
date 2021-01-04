using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.Enums;
using GSCrm.Notifications.Params;
using System;
using System.Collections.Generic;

namespace GSCrm.Notifications.Factories.OrgNotFactories
{
    public class DivDeleteNotFactory : OrgNotificationFactory<DivDeleteParams>
    {
        public DivDeleteNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, DivDeleteParams divDeleteParams)
            : base(serviceProvider, context, divDeleteParams)
        { }

        protected override Notification Create(NotificationTarget notificationTarget)
            => notificationTarget switch
            {
                NotificationTarget.Email => new EmailNotification()
                {
                    Id = Guid.NewGuid(),
                    Subject = resManager.GetString("DivDeleteNotSubject"),
                    Header = resManager.GetString("DivDeleteNotHeader"),
                    Content = resManager.GetString("DivDeleteNotContent")
                },
                NotificationTarget.Inbox => new InboxNotification()
                {
                    Id = Guid.NewGuid(),
                    ActionType = NotificationActionType.Hide,
                    Content = resManager.GetString("DivDeleteNotContent"),
                    HasRead = false
                },
                _ => default
            };

        protected override string GetEmailTemplate()
        {
            throw new NotImplementedException();
        }

        protected override List<NotificationTarget> GetNotificationTargets(OrgNotificationsSetting notificationsSetting)
            => new List<NotificationTarget> { notificationsSetting.TDivDeleteNot };

        protected override bool NeedNotification(OrgNotificationsSetting notificationsSetting)
            => notificationsSetting.DivDeleteNot;
    }
}
