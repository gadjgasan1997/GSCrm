using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Notifications.Params;
using System;
using System.Collections.Generic;

namespace GSCrm.Notifications.Factories.OrgNotFactories
{
    public abstract class EmpUpdateNotFactory<TEmpUpdateParams> : OrgNotificationFactory<TEmpUpdateParams>
        where TEmpUpdateParams : EmpUpdateParams
    {
        protected EmpUpdateNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, TEmpUpdateParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        protected override sealed Notification Create(NotificationTarget notificationTarget)
        {
            switch (notificationTarget)
            {
                case NotificationTarget.Email:
                    return new EmailNotification()
                    {
                        Id = Guid.NewGuid(),
                        Subject = resManager.GetString("EmpUpdateNotSubject"),
                        Header = resManager.GetString("OrgEmailHeader").Replace("{orgName}", notificationParams.Organization.Name),
                        Content = GetEmailTemplate(),
                        NotificationSource = NotificationSource.Organization,
                        SourceId = notificationParams.Organization.Id.ToString(),
                        NotificationType = NotificationType.EmpUpdate
                    };

                case NotificationTarget.Inbox:
                    {
                        // Простановка базовых параметров
                        InboxNotification inboxNot = new InboxNotification()
                        {
                            Id = Guid.NewGuid(),
                            NotificationSource = NotificationSource.Organization,
                            SourceId = notificationParams.Organization.Id.ToString(),
                            NotificationType = NotificationType.EmpUpdate,
                            Attrib1 = notificationParams.EmpUpdateType.ToString(),
                            Attrib2 = notificationParams.ChangedEmployee.Id.ToString()
                        };

                        // Проставление дополнительных атрибутов в зависимости от типа изменения сотрудника
                        InitInboxNotParams(inboxNot);
                        return inboxNot;
                    }

                default:
                    return default;
            }
        }

        protected override sealed void InitNotification(NotificationTarget notificationTarget)
            => base.InitNotification(notificationTarget);

        protected virtual void InitInboxNotParams(InboxNotification inboxNot) { }

        protected override sealed List<NotificationTarget> GetNotificationTargets(OrgNotificationsSetting orgNotSetting)
            => new List<NotificationTarget> { orgNotSetting.TEmpUpdateNot };

        protected override sealed bool NeedNotification(OrgNotificationsSetting orgNotSetting)
            => orgNotSetting.EmpUpdateNot;
    }
}
