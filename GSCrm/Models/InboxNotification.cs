using GSCrm.Models.Enums;

namespace GSCrm.Models
{
    public class InboxNotification : Notification
    {
        public NotificationActionType ActionType { get; set; } = NotificationActionType.Hide;
        public bool HasRead { get; set; }

        public InboxNotification() : base() { }
    }
}
