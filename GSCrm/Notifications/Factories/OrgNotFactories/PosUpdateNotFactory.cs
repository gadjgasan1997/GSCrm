using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Notifications.Params;
using System;
using System.Collections.Generic;
using System.Text;

namespace GSCrm.Notifications.Factories.OrgNotFactories
{
    public class PosUpdateNotFactory : OrgNotificationFactory<PosUpdateParams>
    {
        public PosUpdateNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, PosUpdateParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        protected override Notification Create(NotificationTarget notificationTarget)
            => notificationTarget switch
            {
                NotificationTarget.Email => new EmailNotification()
                {
                    Id = Guid.NewGuid(),
                    Subject = resManager.GetString("PosUpdateNotSubject"),
                    Header = resManager.GetString("OrgEmailHeader").Replace("{orgName}", notificationParams.Organization.Name),
                    Content = GetEmailTemplate(),
                    NotificationSource = NotificationSource.Organization,
                    SourceId = notificationParams.Organization.Id.ToString(),
                    NotificationType = NotificationType.PosUpdate
                },
                NotificationTarget.Inbox => new InboxNotification()
                {
                    Id = Guid.NewGuid(),
                    NotificationSource = NotificationSource.Organization,
                    SourceId = notificationParams.Organization.Id.ToString(),
                    NotificationType = NotificationType.PosUpdate,
                    Attrib1 = notificationParams.ChangedPosition.Id.ToString(),
                    Attrib2 = notificationParams.DivisionChanged.ToString(),
                    Attrib3 = notificationParams.IsPrimary.ToString()
                },
                _ => default
            };

        protected override string GetEmailTemplate()
        {
            if (notificationParams.DivisionChanged)
            {
                if (notificationParams.IsPrimary)
                    return new StringBuilder()
                        .Append($"<div><p>Должность, <a href='{notificationParams.PositionUrl}'>{notificationParams.ChangedPosition.Name}</a>, которую вы занимаете, была перенесена в другое подразделение.")
                        .Append($"<p>Ваш профиль сотрудника был заблокирован. Для разблокировки необходимо, чтобы уполномоченный сотрудник назначил вам какую-либо должность.</p>")
                        .Append($"<p>Организация: <a href='{notificationParams.OrganizationUrl}'>{notificationParams.Organization.Name}</a></p></div>")
                        .ToString();
                else return new StringBuilder()
                    .Append($"<div><p>Должность, <a href='{notificationParams.PositionUrl}'>{notificationParams.ChangedPosition.Name}</a>, которую вы занимаете, была перенесена в другое подразделение.")
                    .Append($"<p>Организация: <a href='{notificationParams.OrganizationUrl}'>{notificationParams.Organization.Name}</a></p></div>")
                    .ToString();
            }
            else return new StringBuilder()
                .Append($"<div><p>Должность, <a href='{notificationParams.PositionUrl}'>{notificationParams.ChangedPosition.Name}</a>, которую вы занимаете, была изменена.")
                .Append($"<p>Организация: <a href='{notificationParams.OrganizationUrl}'>{notificationParams.Organization.Name}</a></p></div>")
                .ToString();
        }

        protected override List<NotificationTarget> GetNotificationTargets(OrgNotificationsSetting orgNotSetting)
            => new List<NotificationTarget> { orgNotSetting.TPosUpdateNot };

        protected override bool NeedNotification(OrgNotificationsSetting orgNotSetting)
            => orgNotSetting.PosUpdateNot;
    }
}
