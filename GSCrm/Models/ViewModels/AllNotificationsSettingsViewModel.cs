using System.Collections.Generic;

namespace GSCrm.Models.ViewModels
{
    public class AllNotificationsSettingsViewModel : BaseViewModel
    {
        public UserNotificationsSettingViewModel UserNotificationsSettingViewModel { get; set; }
        public List<OrgNotificationsSettingViewModel> OrgNotificationsSettingViewModels { get; set; }
    }
}
