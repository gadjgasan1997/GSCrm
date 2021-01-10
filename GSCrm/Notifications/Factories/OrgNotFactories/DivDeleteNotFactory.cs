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
    public class DivDeleteNotFactory : OrgNotificationFactory<DivDeleteParams>
    {
        public DivDeleteNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, DivDeleteParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        protected override Notification Create(NotificationTarget notificationTarget)
            => notificationTarget switch
            {
                NotificationTarget.Email => new EmailNotification()
                {
                    Id = Guid.NewGuid(),
                    Subject = resManager.GetString("DivDeleteNotSubject"),
                    Header = resManager.GetString("OrgEmailHeader").Replace("{orgName}", notificationParams.Organization.Name),
                    Content = GetEmailTemplate(),
                    NotificationSource = NotificationSource.Organization,
                    SourceId = notificationParams.Organization.Id.ToString(),
                    NotificationType = NotificationType.DivDelete
                },
                NotificationTarget.Inbox => new InboxNotification()
                {
                    Id = Guid.NewGuid(),
                    NotificationSource = NotificationSource.Organization,
                    SourceId = notificationParams.Organization.Id.ToString(),
                    NotificationType = NotificationType.DivDelete,
                    Attrib1 = notificationParams.RemovedDivision.Name
                },
                _ => default
            };

        protected override string GetEmailTemplate()
            => new StringBuilder()
            .Append($"<div><p>Подразделение {notificationParams.RemovedDivision.Name}, в котором вы состоите, было удалено.")
            .Append($"<p>Ваш профиль сотрудника был заблокирован. Для разблокировки необходимо, " +
                $"чтобы уполномоченный сотрудник добавил вас в какое-либо подразделение и назначил должность.</p>")
            .Append($"<p>Организация: <a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.Organization.Id })}'>" +
                $"{notificationParams.Organization.Name}</a></p></div>")
            .ToString();

        protected override List<NotificationTarget> GetNotificationTargets(OrgNotificationsSetting orgNotSetting)
            => new List<NotificationTarget> { orgNotSetting.TDivDeleteNot };

        protected override bool NeedNotification(OrgNotificationsSetting orgNotSetting)
            => orgNotSetting.DivDeleteNot;
    }
}
