using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Notifications.Params;
using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using static GSCrm.CommonConsts;
using GSCrm.Notifications.Params.EmpUpdate;

namespace GSCrm.Notifications.Factories.OrgNotFactories.EmpUpdate
{
    public class ChangeDivisionNotFactory : EmpUpdateNotFactory<ChangeDivisionParams>
    {
        public ChangeDivisionNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, ChangeDivisionParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        protected override string GetEmailTemplate()
            => new StringBuilder()
            .Append($"<div><p><a href='{urlHelper.Action(EMPLOYEE, EMPLOYEE, new { id = notificationParams.ChangedEmployee.Id }, httpContext.Request.Scheme)}'>" +
                $"{notificationParams.ChangedEmployee.GetFullName()}</a>, Вас перевели в другое подразделение и выдали новую должность - " +
                $"<a href='{urlHelper.Action(POSITION, POSITION, new { id = notificationParams.NewEmployeePosition.Id }, httpContext.Request.Scheme)}'>{notificationParams.NewEmployeePosition.Name}</a></p>" +
                $"<p>Организация: <a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.Organization.Id }, httpContext.Request.Scheme)}'>" +
                $"{notificationParams.Organization.Name}</a></p></div>")
            .ToString();

        protected override void InitInboxNotParams(InboxNotification inboxNot)
            => inboxNot.Attrib3 = notificationParams.NewEmployeePosition.Id.ToString();
    }
}
