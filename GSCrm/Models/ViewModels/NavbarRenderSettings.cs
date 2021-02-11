using GSCrm.Data.ApplicationInfo;

namespace GSCrm.Models.ViewModels
{
    public class NavbarRenderSettings
    {
        public int ItemsCount { get; set; }
        public ViewInfo ViewInfo { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}
