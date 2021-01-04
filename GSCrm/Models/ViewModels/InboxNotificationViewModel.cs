using GSCrm.Models.Enums;
using GSCrm.Notifications;

namespace GSCrm.Models.ViewModels
{
    public class InboxNotificationViewModel : BaseViewModel
    {
        public NotificationSource NotificationSource { get; set; } = NotificationSource.Organization;
        public NotificationType NotificationType { get; set; } = NotificationType.None;
        public NotificationActionType ActionType { get; set; } = NotificationActionType.Hide;
        public string Content { get; set; }
        public bool HasRead { get; set; }
        public Organization InviteOrg { get; set; }
    }
}
