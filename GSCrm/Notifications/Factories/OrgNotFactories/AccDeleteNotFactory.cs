using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Notifications.Params;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using static GSCrm.CommonConsts;

namespace GSCrm.Notifications.Factories.OrgNotFactories
{
    public class AccDeleteNotFactory : OrgNotificationFactory<AccDeleteParams>
    {
        public AccDeleteNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, AccDeleteParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        protected override Notification Create(NotificationTarget notificationTarget)
            => notificationTarget switch
            {
                NotificationTarget.Email => new EmailNotification()
                {
                    Id = Guid.NewGuid(),
                    Subject = resManager.GetString("AccDeleteNotSubject"),
                    Header = resManager.GetString("OrgEmailHeader").Replace("{orgName}", notificationParams.OwnerOrg.Name),
                    Content = GetEmailTemplate(),
                    NotificationSource = NotificationSource.Organization,
                    SourceId = notificationParams.OwnerOrg.Id.ToString(),
                    NotificationType = NotificationType.AccDelete
                },
                NotificationTarget.Inbox => new InboxNotification()
                {
                    Id = Guid.NewGuid(),
                    NotificationSource = NotificationSource.Organization,
                    SourceId = notificationParams.OwnerOrg.Id.ToString(),
                    NotificationType = NotificationType.AccDelete,
                    Attrib1 = notificationParams.RemovedAccount.Name
                },
                _ => default
            };

        protected override string GetEmailTemplate()
            => new StringBuilder()
            .Append($"<div><p>Клиент \"{notificationParams.RemovedAccount.Name}\", с которым Вы работали, был удален.</p>")
            .Append($"<p>Организация: <a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.OwnerOrg.Id }, httpContext.Request.Scheme)}'>" +
                $"{notificationParams.OwnerOrg.Name}</a></p></div>")
            .ToString();

        protected override List<NotificationTarget> GetNotificationTargets(OrgNotificationsSetting orgNotSetting)
            => new List<NotificationTarget> { orgNotSetting.TAccDeleteNot };

        protected override bool NeedNotification(OrgNotificationsSetting orgNotSetting)
            => orgNotSetting.AccDeleteNot;
    }
}
