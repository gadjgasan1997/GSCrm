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
    public class UpdateContactNotFactory : EmpUpdateNotFactory<UpdateContactParams>
    {
        public UpdateContactNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, UpdateContactParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        protected override string GetEmailTemplate()
            => new StringBuilder()
            .Append($"<div><p><a href='{urlHelper.Action(EMPLOYEE, EMPLOYEE, new { id = notificationParams.ChangedEmployee.Id }, httpContext.Request.Scheme)}'>" +
                $"{notificationParams.ChangedEmployee.GetFullName()}</a>, данные одного из Ваших контактов в организации " +
                $"<a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.Organization.Id }, httpContext.Request.Scheme)}'>" +
                $"{notificationParams.Organization.Name}</a> дыли изменены следующими образом:</p>" +
                $"<ul><li>Тип: c {notificationParams.OldEmployeeContact.ContactType.ToLocalString()} на {notificationParams.NewEmployeeContact.ContactType.ToLocalString()}</li>" +
                $"<li>Почта: с {notificationParams.OldEmployeeContact.Email} на {notificationParams.NewEmployeeContact.Email}</li>" +
                $"<li>Номер телефона: с {notificationParams.OldEmployeeContact.PhoneNumber} на {notificationParams.NewEmployeeContact.PhoneNumber}</li></ul></div>")
            .ToString();

        protected override void InitInboxNotParams(InboxNotification inboxNot)
        {
            inboxNot.WriteObjectToAttr3(notificationParams.OldEmployeeContact);
            inboxNot.WriteObjectToAttr4(notificationParams.NewEmployeeContact);
        }
    }
}
