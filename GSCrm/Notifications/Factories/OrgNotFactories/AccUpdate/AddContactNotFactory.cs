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
    public class AddContactNotFactory : AccUpdateNotFactory<AddContactParams>
    {
        public AddContactNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, AddContactParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        protected override string GetEmailTemplate()
            => new StringBuilder()
            .Append($"<div><p>У клиента " +
                $"<a href='{urlHelper.Action(ACCOUNT, ACCOUNT, new { id = notificationParams.ChangedAccount.Id }, httpContext.Request.Scheme)}'>" +
                $"{notificationParams.ChangedAccount.Name}</a>, с которым вы работаете, был добавлен новый контакт со следующими данными:</p>" +
                $"<ul><li>Тип: {notificationParams.NewAccountContact.ContactType.ToLocalString()}</li>" +
                $"<li>ФИО: {notificationParams.NewAccountContact.GetFullName()}</li>" +
                $"<li>Почта: {notificationParams.NewAccountContact.Email}</li>" +
                $"<li>Номер телефона: {notificationParams.NewAccountContact.PhoneNumber}</li></ul>" +
                $"<p>Организация: <a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.OwnerOrg.Id }, httpContext.Request.Scheme)}'>" +
                $"{notificationParams.OwnerOrg.Name}</a></p></div>")
            .ToString();

        protected override void InitInboxNotParams(InboxNotification inboxNot)
            => inboxNot.WriteObjectToAttr3(notificationParams.NewAccountContact);
    }
}
