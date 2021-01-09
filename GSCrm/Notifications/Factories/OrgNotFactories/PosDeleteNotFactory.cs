using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Notifications.Params;
using System;
using System.Collections.Generic;
using System.Text;

namespace GSCrm.Notifications.Factories.OrgNotFactories
{
    public class PosDeleteNotFactory : OrgNotificationFactory<PosDeleteParams>
    {
        public PosDeleteNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, PosDeleteParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        protected override Notification Create(NotificationTarget notificationTarget)
            => notificationTarget switch
            {
                NotificationTarget.Email => new EmailNotification()
                {
                    Id = Guid.NewGuid(),
                    Subject = resManager.GetString("PosDeleteNotSubject"),
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
                    NotificationType = NotificationType.PosDelete,
                    Attrib1 = notificationParams.RemovedPosition.Name,
                    Attrib2 = notificationParams.IsPrimary.ToString()
                },
                _ => default
            };

        protected override string GetEmailTemplate()
        {
            if (notificationParams.IsPrimary)
            {
                return new StringBuilder()
                    .Append($"<div><p>Должность {notificationParams.RemovedPosition.Name}, которую вы занимали, была удалена.</p>")
                    .Append($"<p>Ваш профиль сотрудника был заблокирован. Для разблокировки необходимо, " +
                        $"чтобы уполномоченный сотрудник назначил вам какую-либо должность.</p>")
                    .Append($"<p>Организация: <a href='{notificationParams.OrganizationUrl}'>{notificationParams.Organization.Name}</a></p></div>")
                    .ToString();
            }
            else return new StringBuilder()
                .Append($"<div><p>Должность {notificationParams.RemovedPosition.Name}, которую вы занимали, была удалена.")
                .Append($"<p>Организация: <a href='{notificationParams.OrganizationUrl}'>{notificationParams.Organization.Name}</a></p></div>")
                .ToString();
        }

        protected override List<NotificationTarget> GetNotificationTargets(OrgNotificationsSetting orgNotSetting)
            => new List<NotificationTarget> { orgNotSetting.TPosDeleteNot };

        protected override bool NeedNotification(OrgNotificationsSetting orgNotSetting)
            => orgNotSetting.PosDeleteNot;
    }
}
