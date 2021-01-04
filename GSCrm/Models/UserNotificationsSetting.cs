using GSCrm.Notifications;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSCrm.Models
{
    public class UserNotificationsSetting : BaseDataModel
    {
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }
        public bool OrgInvoiceNot { get; set; }
        public NotificationTarget TOrgInvoiceNot { get; set; }
    }
}
