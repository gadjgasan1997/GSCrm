using GSCrm.Notifications;

namespace GSCrm.Models.ViewModels
{
    public class UserNotificationsSettingViewModel : BaseViewModel
    {
        public bool OrgInvoiceNot { get; set; }
        public NotificationTarget TOrgInvoiceNot { get; set; }
    }
}
