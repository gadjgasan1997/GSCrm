using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Notifications.Params;
using System;
using System.Collections.Generic;
using System.Text;

namespace GSCrm.Notifications.Factories.OrgNotFactories
{
    public class EmpDeleteNotFactory : OrgNotificationFactory<EmpDeleteParams>
    {
        public EmpDeleteNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, EmpDeleteParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        protected override Notification Create(NotificationTarget notificationTarget)
           => notificationTarget switch
           {
               NotificationTarget.Email => new EmailNotification()
               {
                   Id = Guid.NewGuid(),
                   Subject = resManager.GetString("EmpDeleteNotSubject"),
                   Header = resManager.GetString("OrgEmailHeader").Replace("{orgName}", notificationParams.Organization.Name),
                   Content = GetEmailTemplate(),
                   NotificationSource = NotificationSource.Organization,
                   SourceId = notificationParams.Organization.Id.ToString(),
                   NotificationType = NotificationType.EmpDelete
               },
               NotificationTarget.Inbox => new InboxNotification()
               {
                   Id = Guid.NewGuid(),
                   NotificationSource = NotificationSource.Organization,
                   SourceId = notificationParams.Organization.Id.ToString(),
                   NotificationType = NotificationType.EmpDelete,
                   Attrib1 = notificationParams.RemovedEmployee.GetFullName()
               },
               _ => default
           };

        protected override string GetEmailTemplate()
            => new StringBuilder()
            .Append($"<div><p>{notificationParams.RemovedEmployee.GetFullName()}, Вы были уволены из организации <a href='{notificationParams.OrganizationUrl}'>{notificationParams.Organization.Name}</a>.")
            .ToString();

        protected override List<NotificationTarget> GetNotificationTargets(OrgNotificationsSetting orgNotSetting)
            => new List<NotificationTarget> { orgNotSetting.TEmpDeleteNot };

        protected override bool NeedNotification(OrgNotificationsSetting orgNotSetting)
            => orgNotSetting.EmpDeleteNot;
    }
}
