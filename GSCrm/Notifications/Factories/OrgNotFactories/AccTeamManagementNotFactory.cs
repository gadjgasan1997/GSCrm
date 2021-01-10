using System;
using System.Text;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Notifications.Params;
using GSCrm.Notifications.Auxiliary;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using static GSCrm.CommonConsts;

namespace GSCrm.Notifications.Factories.OrgNotFactories
{
    public class AccTeamManagementNotFactory : OrgNotificationFactory<AccTeamManagementParams>
    {
        public AccTeamManagementNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, AccTeamManagementParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        protected override Notification Create(NotificationTarget notificationTarget)
            => notificationTarget switch
            {
                NotificationTarget.Email => new EmailNotification()
                {
                    Id = Guid.NewGuid(),
                    Subject = resManager.GetString("AccTeamManagementNotSubject"),
                    Header = resManager.GetString("OrgEmailHeader").Replace("{orgName}", notificationParams.OwnerOrg.Name),
                    Content = GetEmailTemplate(),
                    NotificationSource = NotificationSource.Organization,
                    SourceId = notificationParams.OwnerOrg.Id.ToString(),
                    NotificationType = NotificationType.AccTeamManagement
                },
                NotificationTarget.Inbox => new InboxNotification()
                {
                    Id = Guid.NewGuid(),
                    NotificationSource = NotificationSource.Organization,
                    SourceId = notificationParams.OwnerOrg.Id.ToString(),
                    NotificationType = NotificationType.AccTeamManagement,
                    Attrib1 = notificationParams.Account.Id.ToString(),
                    Attrib2 = notificationParams.AccTeamManagementNotType.ToString()
                },
                _ => default
            };

        protected override string GetEmailTemplate()
            => notificationParams.AccTeamManagementNotType switch
            {
                AccTeamManagementNotType.AddedToNew => new StringBuilder()
                    .Append($"<div><p>От лица организации <a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.OwnerOrg.Id }, httpContext.Request.Scheme)}'>{notificationParams.OwnerOrg.Name}</a>, " +
                            $"в которой Вы состоите, был создан клиент <a href='{urlHelper.Action(ACCOUNT, ACCOUNT, new { id = notificationParams.Account.Id }, httpContext.Request.Scheme)}'>{notificationParams.Account.Name}</a>.</p>")
                    .Append($"<p>Вы были добавлены в команду по клиенту.</p></div>")
                    .ToString(),
                AccTeamManagementNotType.AddedToExists => new StringBuilder()
                    .Append($"<div><p>Вас добавили в команду по клиенту <a href='{urlHelper.Action(ACCOUNT, ACCOUNT, new { id = notificationParams.Account.Id }, httpContext.Request.Scheme)}'>" +
                        $"{notificationParams.Account.Name}</a>.</p>")
                    .Append($"<p>Организация: <a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.OwnerOrg.Id }, httpContext.Request.Scheme)}'>" +
                        $"{notificationParams.OwnerOrg.Name}</a></p></div>")
                    .ToString(),
                AccTeamManagementNotType.Removed => new StringBuilder()
                    .Append($"<div><p>Вас удалили из команды по клиенту <a href='{urlHelper.Action(ACCOUNT, ACCOUNT, new { id = notificationParams.Account.Id }, httpContext.Request.Scheme)}'>" +
                        $"{notificationParams.Account.Name}</a>.</p>")
                    .Append($"<p>Организация: <a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.OwnerOrg.Id }, httpContext.Request.Scheme)}'>" +
                        $"{notificationParams.OwnerOrg.Name}</a></p></div>")
                    .ToString(),
                AccTeamManagementNotType.SetToPrimary => new StringBuilder()
                    .Append($"<div><p>Вас назначили основным менеджером в команде по клиенту <a href='{urlHelper.Action(ACCOUNT, ACCOUNT, new { id = notificationParams.Account.Id }, httpContext.Request.Scheme)}'>" +
                        $"{notificationParams.Account.Name}</a>.</p>")
                    .Append($"<p>Организация: <a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.OwnerOrg.Id }, httpContext.Request.Scheme)}'>" +
                        $"{notificationParams.OwnerOrg.Name}</a></p></div>")
                    .ToString(),
                _ => string.Empty
            };

        protected override List<NotificationTarget> GetNotificationTargets(OrgNotificationsSetting orgNotSetting)
            => new List<NotificationTarget> { orgNotSetting.TAccTeamManagementNot };

        protected override bool NeedNotification(OrgNotificationsSetting orgNotSetting)
            => orgNotSetting.AccTeamManagementNot;
    }
}
