using System.Collections.Generic;

namespace GSCrm.Models.ViewModels
{
    public class InboxNotificationsViewModel : BaseViewModel
    {
        public string UserNotificationsSettingId { get; set; }
        public IEnumerable<InboxNotificationViewModel> InboxNotificationViewModels { get; set; }
    }
}
