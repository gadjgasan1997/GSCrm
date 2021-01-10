using System;
using System.Text;
using GSCrm.Data;
using GSCrm.Helpers;
using Microsoft.AspNetCore.Mvc;
using GSCrm.Notifications.Params.AccUpdate;
using static GSCrm.CommonConsts;
using GSCrm.Models;

namespace GSCrm.Notifications.Factories.OrgNotFactories.AccUpdate
{
    public class UpdateContactNotFactory : AccUpdateNotFactory<UpdateContactParams>
    {
        public UpdateContactNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, UpdateContactParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        protected override string GetEmailTemplate()
        {
            if (notificationParams.OldAccountContact.GetFullName() != notificationParams.NewAccountContact.GetFullName())
            {
                return new StringBuilder()
                    .Append($"<div><p>У клиента <a href='{urlHelper.Action(ACCOUNT, ACCOUNT, new { id = notificationParams.ChangedAccount.Id }, httpContext.Request.Scheme)}'>" +
                        $"{notificationParams.ChangedAccount.Name}</a> были изменены данные контакта с именем  {notificationParams.OldAccountContact.GetFullName()}.</p>" +
                        $"<p>Имя было изменено на: {notificationParams.NewAccountContact.GetFullName()}</p>" +
                        $"<p>Организацию: <a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.OwnerOrg.Id }, httpContext.Request.Scheme)}'>" +
                        $"{notificationParams.OwnerOrg.Name}</a></p></div>")
                    .ToString();
            }
            else return new StringBuilder()
                .Append($"<div><p>У клиента <a href='{urlHelper.Action(ACCOUNT, ACCOUNT, new { id = notificationParams.ChangedAccount.Id }, httpContext.Request.Scheme)}'>" +
                    $"{notificationParams.ChangedAccount.Name}</a> были изменены данные контакта с именем  {notificationParams.OldAccountContact.GetFullName()}.</p>" +
                    $"<p>Организацию: <a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.OwnerOrg.Id }, httpContext.Request.Scheme)}'>" +
                    $"{notificationParams.OwnerOrg.Name}</a></p></div>")
                .ToString();
        }

        protected override void InitInboxNotParams(InboxNotification inboxNot)
        {
            inboxNot.WriteObjectToAttr3(notificationParams.OldAccountContact);
            inboxNot.WriteObjectToAttr4(notificationParams.NewAccountContact);
        }
    }
}
