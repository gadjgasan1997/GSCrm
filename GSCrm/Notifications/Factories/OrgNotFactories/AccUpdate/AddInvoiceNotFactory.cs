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
    public class AddInvoiceNotFactory : AccUpdateNotFactory<AddInvoiceParams>
    {
        public AddInvoiceNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, AddInvoiceParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        protected override string GetEmailTemplate()
            => new StringBuilder()
                .Append($"<div><p>У клиента " +
                    $"<a href='{urlHelper.Action(ACCOUNT, ACCOUNT, new { id = notificationParams.ChangedAccount.Id }, httpContext.Request.Scheme)}'>" +
                    $"{notificationParams.ChangedAccount.Name}</a>, с которым вы работаете, были добавлены следующие банковские реквизиты:</p>" +
                    $"<ul><li>Банк: {notificationParams.NewAccountInvoice.BankName}</li>" +
                    $"<li>Город: {notificationParams.NewAccountInvoice.City}</li>" +
                    $"<li>БИК: {notificationParams.NewAccountInvoice.BIC}</li>" +
                    $"<li>Расчетный счет: {notificationParams.NewAccountInvoice.CheckingAccount}</li>" +
                    $"<li>Корреспонденский счет: {notificationParams.NewAccountInvoice.CorrespondentAccount}</li></ul>" +
                    $"<p>Организация: <a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.OwnerOrg.Id }, httpContext.Request.Scheme)}'>" +
                    $"{notificationParams.OwnerOrg.Name}</a></p></div>")
                .ToString();

        protected override void InitInboxNotParams(InboxNotification inboxNot)
            => inboxNot.WriteObjectToAttr3(notificationParams.NewAccountInvoice);            
    }
}
