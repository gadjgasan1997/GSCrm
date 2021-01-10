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
    public class AddAddressNotFactory : AccUpdateNotFactory<AddAddressParams>
    {
        public AddAddressNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, AddAddressParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        protected override string GetEmailTemplate()
            => new StringBuilder()
            .Append($"<div><p>У клиента " +
                $"<a href='{urlHelper.Action(ACCOUNT, ACCOUNT, new { id = notificationParams.ChangedAccount.Id }, httpContext.Request.Scheme)}'>" +
                $"{notificationParams.ChangedAccount.Name}</a>, с которым вы работаете, был добавлен новый адрес со следующими данными:</p>" +
                $"<ul><li>Тип: {notificationParams.NewAccountAddress.AddressType.ToLocalString()}</li>" +
                $"<li>Полный адрес: {notificationParams.NewAccountAddress.GetFullAddress()}</li></ul>" +
                $"<p>Организация: <a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.OwnerOrg.Id }, httpContext.Request.Scheme)}'>" +
                $"{notificationParams.OwnerOrg.Name}</a></p></div>")
            .ToString();

        protected override void InitInboxNotParams(InboxNotification inboxNot)
            => inboxNot.WriteObjectToAttr3(notificationParams.NewAccountAddress);
    }
}
