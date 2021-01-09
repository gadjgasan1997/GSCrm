using GSCrm.Notifications;
using System.Collections.Generic;

namespace GSCrm.Models
{
    public class Notification : BaseDataModel
    {
        public NotificationSource NotificationSource { get; set; } = NotificationSource.Organization;
        public string SourceId { get; set; }
        public NotificationType NotificationType { get; set; } = NotificationType.None;

        public List<UserNotification> UserNotifications { get; set; }

        public Notification()
        {
            UserNotifications = new List<UserNotification>();
            NotificationSource = NotificationSource.Organization;
        }
    }
}
