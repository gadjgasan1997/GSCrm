using System;
using System.Text;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using Microsoft.AspNetCore.Mvc;
using GSCrm.Notifications.Params.EmpUpdate;
using static GSCrm.CommonConsts;

namespace GSCrm.Notifications.Factories.OrgNotFactories.EmpUpdate
{
    public class AddContactNotFactory : EmpUpdateNotFactory<AddContactParams>
    {
        public AddContactNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, AddContactParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        protected override string GetEmailTemplate()
            => new StringBuilder()
            .Append($"<div><p><a href='{urlHelper.Action(EMPLOYEE, EMPLOYEE, new { id = notificationParams.ChangedEmployee.Id }, httpContext.Request.Scheme)}'>" +
                $"{notificationParams.ChangedEmployee.GetFullName()}</a>, Вам был добавлен новый контакт в организации " +
                $"<a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.Organization.Id }, httpContext.Request.Scheme)}'>" +
                $"{notificationParams.Organization.Name}</a> со следующими данными:</p>" +
                $"<ul><li>Тип: {notificationParams.NewEmployeeContact.ContactType.ToLocalString()}</li>" +
                $"<li>Почта: {notificationParams.NewEmployeeContact.Email}</li>" +
                $"<li>Номер телефона: {notificationParams.NewEmployeeContact.PhoneNumber}</li></ul></div>")
            .ToString();

        protected override void InitInboxNotParams(InboxNotification inboxNot)
            => inboxNot.WriteObjectToAttr3(notificationParams.NewEmployeeContact);
    }
}
