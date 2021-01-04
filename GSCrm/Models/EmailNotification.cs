﻿using GSCrm.Notifications;

namespace GSCrm.Models
{
    public class EmailNotification : Notification
    {
        public string Subject { get; set; }
        public string Header { get; set; }

        public EmailNotification() : base() { }
    }
}
