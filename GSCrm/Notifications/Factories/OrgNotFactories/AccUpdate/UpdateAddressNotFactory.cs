using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using GSCrm.Notifications.Params.AccUpdate;
using static GSCrm.CommonConsts;

namespace GSCrm.Notifications.Factories.OrgNotFactories.AccUpdate
{
    public class UpdateAddressNotFactory : AccUpdateNotFactory<UpdateAddressParams>
    {
        public UpdateAddressNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, UpdateAddressParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        protected override string GetEmailTemplate()
        {
            if (notificationParams.OldAccountAddress.GetFullAddress() != notificationParams.NewAccountAddress.GetFullAddress())
            {
                return new StringBuilder()
                    .Append($"<div><p>У клиента " +
                        $"<a href='{urlHelper.Action(ACCOUNT, ACCOUNT, new { id = notificationParams.ChangedAccount.Id }, httpContext.Request.Scheme)}'>" +
                        $"{notificationParams.ChangedAccount.Name}</a>, с которым вы работаете, был изменен адрес <strong>с</strong> " +
                        $"{notificationParams.OldAccountAddress.GetFullAddress()} <strong>на</strong> {notificationParams.NewAccountAddress.GetFullAddress()}</p>" +
                        $"<p>Организация: <a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.OwnerOrg.Id }, httpContext.Request.Scheme)}'>" +
                        $"{notificationParams.OwnerOrg.Name}</a></p></div>")
                    .ToString();
            }
            else return new StringBuilder()
                .Append($"<div><p>У клиента " +
                    $"<a href='{urlHelper.Action(ACCOUNT, ACCOUNT, new { id = notificationParams.ChangedAccount.Id }, httpContext.Request.Scheme)}'>" +
                    $"{notificationParams.ChangedAccount.Name}</a>, с которым вы работаете, был изменен адрес {notificationParams.OldAccountAddress.GetFullAddress()}.</p>" +
                    $"<p>Организация: <a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.OwnerOrg.Id }, httpContext.Request.Scheme)}'>" +
                    $"{notificationParams.OwnerOrg.Name}</a></p></div>")
                .ToString();
        }

        protected override void InitInboxNotParams(InboxNotification inboxNot)
        {
            inboxNot.WriteObjectToAttr3(notificationParams.OldAccountAddress);
            inboxNot.WriteObjectToAttr4(notificationParams.NewAccountAddress);
        }
    }
}
