using Microsoft.AspNetCore.Http;
using static GSCrm.CommonConsts;

namespace GSCrm.Data.ApplicationInfo
{
    public interface IViewsInfo
    {
        void Set(string userId, string viewName, ViewInfo viewInfo);
        ViewInfo Get(string userId, string viewName);
        ViewInfo Get(ApplicationDbContext context, HttpContext httpContext, string viewName);
    }
}
