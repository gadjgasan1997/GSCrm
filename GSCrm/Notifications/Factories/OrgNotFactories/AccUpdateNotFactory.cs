using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Notifications.Params;
using System;
using System.Collections.Generic;

namespace GSCrm.Notifications.Factories.OrgNotFactories
{
    public abstract class AccUpdateNotFactory<TAccUpdateParams> : OrgNotificationFactory<TAccUpdateParams>
        where TAccUpdateParams : AccUpdateParams
    {
        protected AccUpdateNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, TAccUpdateParams notificationParams)
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
                        Subject = resManager.GetString("AccUpdateNotSubject"),
                        Header = resManager.GetString("OrgEmailHeader").Replace("{orgName}", notificationParams.OwnerOrg.Name),
                        Content = GetEmailTemplate(),
                        NotificationSource = NotificationSource.Organization,
                        SourceId = notificationParams.OwnerOrg.Id.ToString(),
                        NotificationType = NotificationType.AccUpdate
                    };

                case NotificationTarget.Inbox:
                    {
                        // Простановка базовых параметров
                        InboxNotification inboxNot = new InboxNotification()
                        {
                            Id = Guid.NewGuid(),
                            NotificationSource = NotificationSource.Organization,
                            SourceId = notificationParams.OwnerOrg.Id.ToString(),
                            NotificationType = NotificationType.AccUpdate,
                            Attrib1 = notificationParams.AccUpdateType.ToString(),
                            Attrib2 = notificationParams.ChangedAccount.Id.ToString()
                        };

                        // Проставление дополнительных атрибутов в зависимости от типа изменения клиента
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
            => new List<NotificationTarget> { orgNotSetting.TAccUpdateNot };

        protected override sealed bool NeedNotification(OrgNotificationsSetting orgNotSetting)
            => orgNotSetting.AccUpdateNot;
    }
}
