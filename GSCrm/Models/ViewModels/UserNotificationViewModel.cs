using GSCrm.Notifications;
using System;

namespace GSCrm.Models.ViewModels
{
    public class UserNotificationViewModel : BaseViewModel
    {
        public Guid NotificationId { get; set; }
        public NotificationSource NotificationSource { get; set; } = NotificationSource.Organization;
        public NotificationType NotificationType { get; set; } = NotificationType.None;
        public bool HasRead { get; set; }
    }
}
