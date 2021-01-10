using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Notifications.Params.AccUpdate;
using static GSCrm.CommonConsts;

namespace GSCrm.Notifications.Factories.OrgNotFactories.AccUpdate
{
    public class DeleteContactNotFactory : AccUpdateNotFactory<DeleteContactParams>
    {
        public DeleteContactNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, DeleteContactParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        protected override string GetEmailTemplate()
            => new StringBuilder()
            .Append($"<div><p>У клиента <a href='{urlHelper.Action(ACCOUNT, ACCOUNT, new { id = notificationParams.ChangedAccount.Id }, httpContext.Request.Scheme)}'>" +
                $"{notificationParams.ChangedAccount.Name}</a> был удален контакт \"{notificationParams.RemovedContact.GetFullName()}\".</p>" +
                $"<p>Организация: <a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.OwnerOrg.Id }, httpContext.Request.Scheme)}'>" +
                $"{notificationParams.OwnerOrg.Name}</a></p></div>")
            .ToString();

        protected override void InitInboxNotParams(InboxNotification inboxNot)
            => inboxNot.WriteObjectToAttr3(notificationParams.RemovedContact);
    }
}
