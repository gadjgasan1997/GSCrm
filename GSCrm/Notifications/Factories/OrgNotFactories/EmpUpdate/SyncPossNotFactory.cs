using GSCrm.Data;
using GSCrm.Helpers;
using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using GSCrm.Notifications.Params.EmpUpdate;
using static GSCrm.CommonConsts;

namespace GSCrm.Notifications.Factories.OrgNotFactories.EmpUpdate
{
    public class SyncPossNotFactory : EmpUpdateNotFactory<SyncPossParams>
    {
        public SyncPossNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, SyncPossParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        protected override string GetEmailTemplate()
            => new StringBuilder()
            .Append($"<div><p><a href='{urlHelper.Action(EMPLOYEE, EMPLOYEE, new { id = notificationParams.ChangedEmployee.Id }, httpContext.Request.Scheme)}'>" +
                $"{notificationParams.ChangedEmployee.GetFullName()}</a>, список занимаемых Вами должностей в организации " +
                $"<a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.Organization.Id }, httpContext.Request.Scheme)}'>" +
                $"{notificationParams.Organization.Name}</a> был изменен.</p></div>")
            .ToString();
    }
}
