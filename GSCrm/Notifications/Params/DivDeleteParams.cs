﻿using GSCrm.Models;

namespace GSCrm.Notifications.Params
{
    public class DivDeleteParams : INotificationParams
    {
        public Organization Organization { get; set; }
        public Division RemovedDivision { get; set; }
    }
}
