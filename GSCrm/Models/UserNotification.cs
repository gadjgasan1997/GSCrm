using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSCrm.Models
{
    public class UserNotification : BaseDataModel
    {
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("Notification")]
        public Guid NotificationId { get; set; }
        public Notification Notification { get; set; }
    }
}
