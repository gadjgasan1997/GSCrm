using System.Collections.Generic;

namespace GSCrm.Models.ViewModels
{
    public class UserNotificationsViewModel : BaseViewModel
    {
        public string UserNotificationsSettingId { get; set; }
        public IEnumerable<UserNotificationViewModel> UserNotificationViewModels { get; set; }
    }
}
