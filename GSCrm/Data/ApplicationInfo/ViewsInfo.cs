using System.Collections.Generic;
using static GSCrm.CommonConsts;
using GSCrm.Helpers;
using Microsoft.AspNetCore.Http;

namespace GSCrm.Data.ApplicationInfo
{
    public class ViewsInfo : IViewsInfo
    {
        private Dictionary<string, Dictionary<string, ViewInfo>> viewsData { get; set; } = new Dictionary<string, Dictionary<string, ViewInfo>>();

        public void Set(string userId, string viewName, ViewInfo viewInfo)
        {
            if (new[] { userId, viewName }.IsNullOrEmpty()) return;
            if (!viewsData.ContainsKey(userId))
            {
                viewsData.Add(userId, new Dictionary<string, ViewInfo>()
                {
                    { viewName, viewInfo }
                });
            }
            else if (!viewsData[userId].ContainsKey(viewName))
                viewsData[userId].Add(viewName, viewInfo);
            else viewsData[userId][viewName] = viewInfo;
        }

        public ViewInfo Get(string userId, string viewName)
        {
            if (new[] { userId, viewName }.IsNullOrEmpty()) return new ViewInfo();
            if (!viewsData.ContainsKey(userId)) return new ViewInfo();
            if (!viewsData[userId].ContainsKey(viewName)) return new ViewInfo();
            return viewsData[userId][viewName];
        }

        public ViewInfo Get(ApplicationDbContext context, HttpContext httpContext, string viewName)
            => Get(httpContext.GetCurrentUser(context).Id, viewName);
    }
}
