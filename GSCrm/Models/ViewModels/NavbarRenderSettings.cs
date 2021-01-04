using GSCrm.Data.ApplicationInfo;
using static GSCrm.CommonConsts;

namespace GSCrm.Models.ViewModels
{
    public class NavbarRenderSettings
    {
        public int DefaultItemsCount { get; set; } = DEFAULT_ITEMS_COUNT;
        public int DefaultMinPageNumber { get; set; } = DEFAULT_MIN_PAGE_NUMBER;
        public int DefaultPageStep { get; set; } = DEFAULT_PAGE_STEP;
        public int ItemsCount { get; set; }
        public ViewInfo ViewInfo { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}
