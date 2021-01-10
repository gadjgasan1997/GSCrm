using System;
using System.Text;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Notifications.Params.AccUpdate;
using Microsoft.AspNetCore.Mvc;
using static GSCrm.CommonConsts;

namespace GSCrm.Notifications.Factories.OrgNotFactories.AccUpdate
{
    public class DeleteInvoiceNotFactory : AccUpdateNotFactory<DeleteInvoiceParams>
    {
        public DeleteInvoiceNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, DeleteInvoiceParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        protected override string GetEmailTemplate()
            => new StringBuilder()
                .Append($"<div><p>У клиента " +
                    $"<a href='{urlHelper.Action(ACCOUNT, ACCOUNT, new { id = notificationParams.ChangedAccount.Id }, httpContext.Request.Scheme)}'>" +
                    $"{notificationParams.ChangedAccount.Name}</a>, с которым вы работаете, были удалены банковские реквизиты со счетом {notificationParams.RemovedInvoice.CheckingAccount}</p>" +
                    $"<p>Организация: <a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.OwnerOrg.Id }, httpContext.Request.Scheme)}'>" +
                    $"{notificationParams.OwnerOrg.Name}</a></p></div>")
                .ToString();

        protected override void InitInboxNotParams(InboxNotification inboxNot)
            => inboxNot.WriteObjectToAttr3(notificationParams.RemovedInvoice);
    }
}
