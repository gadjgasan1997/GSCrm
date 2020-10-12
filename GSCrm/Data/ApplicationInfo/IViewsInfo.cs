using static GSCrm.CommonConsts;

namespace GSCrm.Data.ApplicationInfo
{
    public interface IViewsInfo
    {
        void Set(string userId, string viewName, ViewInfo viewInfo);
        ViewInfo Get(string userId, string viewName);
        void Reset(string userId, string viewName, int currentPageNumber = DEFAULT_MIN_PAGE_NUMBER);
    }
}
