using GSCrm.Data;
using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using GSCrm.Notifications.Params.AccUpdate;
using static GSCrm.CommonConsts;

namespace GSCrm.Notifications.Factories.OrgNotFactories.AccUpdate
{
    public class BaseUpdateNotFactory : AccUpdateNotFactory<BaseUpdateParams>
    {
        public BaseUpdateNotFactory(IServiceProvider serviceProvider, ApplicationDbContext context, BaseUpdateParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        protected override string GetEmailTemplate()
            => new StringBuilder()
            .Append($"<div><p>Данные вашего клиента " +
                $"<a href='{urlHelper.Action(ACCOUNT, ACCOUNT, new { id = notificationParams.ChangedAccount.Id }, httpContext.Request.Scheme)}'>" +
                $"{notificationParams.ChangedAccount.Name}</a> были изменены.</p>" +
                $"<p>Организация: <a href='{urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = notificationParams.OwnerOrg.Id }, httpContext.Request.Scheme)}'>" +
                $"{notificationParams.OwnerOrg.Name}</a></p></div>")
            .ToString();
    }
}
