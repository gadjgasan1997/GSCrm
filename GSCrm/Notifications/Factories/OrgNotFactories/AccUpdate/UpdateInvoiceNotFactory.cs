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
    public class UpdateInvoiceNotFactory : AccUpdateNotFactory<UpdateInvoiceParams>
    {
        public UpdateInvoiceNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, UpdateInvoiceParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        protected override string GetEmailTemplate()
        {
            if (notificationParams.OldAccountInvoice.CheckingAccount != notificationParams.NewAccountInvoice.CheckingAccount)
            {
                return new StringBuilder()
                    .Append($"<div><p>У клиента " +
                        $"<a href='{urlHelper.Action(ACCOUNT, ACCOUNT, new { id = notificationParams.ChangedAccount.Id }, httpContext.Request.Scheme)}'>" +
                        $"{notificationParams.ChangedAccount.Name}</a>, с которым вы работаете, были изменены банковские реквизиты.</p>" +
                        $"<p>Счет {notificationParams.OldAccountInvoice.CheckingAccount} изменился на {notificationParams.NewAccountInvoice.CheckingAccount}.</p>" +
                        $"<p>Организация: <a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.OwnerOrg.Id }, httpContext.Request.Scheme)}'>" +
                        $"{notificationParams.OwnerOrg.Name}</a></p></div>")
                    .ToString();
            }
            else return new StringBuilder()
                .Append($"<div><p>У клиента " +
                    $"<a href='{urlHelper.Action(ACCOUNT, ACCOUNT, new { id = notificationParams.ChangedAccount.Id }, httpContext.Request.Scheme)}'>" +
                    $"{notificationParams.ChangedAccount.Name}</a>, с которым вы работаете, были изменены банковские реквизиты по счету {notificationParams.OldAccountInvoice.CheckingAccount}</p>" +
                    $"<p>Организация: <a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.OwnerOrg.Id }, httpContext.Request.Scheme)}'>" +
                    $"{notificationParams.OwnerOrg.Name}</a></p></div>")
                .ToString();
        }

        protected override void InitInboxNotParams(InboxNotification inboxNot)
        {
            inboxNot.WriteObjectToAttr3(notificationParams.OldAccountInvoice);
            inboxNot.WriteObjectToAttr4(notificationParams.NewAccountInvoice);
        }
    }
}
