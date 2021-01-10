using GSCrm.Data;
using GSCrm.Helpers;
using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using GSCrm.Notifications.Params.EmpUpdate;
using static GSCrm.CommonConsts;

namespace GSCrm.Notifications.Factories.OrgNotFactories.EmpUpdate
{
    public class BaseUpdateNotFactory : EmpUpdateNotFactory<BaseUpdateParams>
    {
        public BaseUpdateNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, BaseUpdateParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        protected override string GetEmailTemplate()
            => new StringBuilder()
            .Append($"<div><p><a href='{urlHelper.Action(EMPLOYEE, EMPLOYEE, new { id = notificationParams.ChangedEmployee.Id }, httpContext.Request.Scheme)}'>" +
                $"{notificationParams.ChangedEmployee.GetFullName()}</a>, Ваши данные в организации " +
                $"<a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.Organization.Id }, httpContext.Request.Scheme)}'>" +
                $"{notificationParams.Organization.Name}</a> были изменены.</p></div>")
            .ToString();
    }
}
