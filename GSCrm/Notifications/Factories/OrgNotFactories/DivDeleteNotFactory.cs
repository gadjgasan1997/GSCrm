using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Notifications.Params;
using System;
using System.Collections.Generic;
using System.Text;

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
                    Subject = resManager.GetString("OrgEmailSubject").Replace("{orgName}", notificationParams.Organization.Name),
                    Header = resManager.GetString("DivDeleteNotHeader"),
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
            .Append($"<p>Подразделение {notificationParams.RemovedDivision.Name}, в котором вы состоите, было удалено.")
            .Append($"<p>Ваш профиль сотрудника был заблокирован. Для разблокировки необходимо, " +
                $"чтобы уполномоченный сотрудник добавил вас в какое-либо подразделение и назначил должность.</p>")
            .Append($"<p>Организация: <a href='{notificationParams.OrganizationUrl}'>{notificationParams.Organization.Name}</a></p>")
            .ToString();

        protected override List<NotificationTarget> GetNotificationTargets(OrgNotificationsSetting notificationsSetting)
            => new List<NotificationTarget> { notificationsSetting.TDivDeleteNot };

        protected override bool NeedNotification(OrgNotificationsSetting notificationsSetting)
            => notificationsSetting.DivDeleteNot;
    }
}
