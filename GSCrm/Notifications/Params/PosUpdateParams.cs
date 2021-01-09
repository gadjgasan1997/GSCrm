﻿using GSCrm.Models;

namespace GSCrm.Notifications.Params
{
    public class PosUpdateParams : INotificationParams
    {
        public Organization Organization { get; set; }
        public Position ChangedPosition { get; set; }
        public bool DivisionChanged { get; set; }
        public bool IsPrimary { get; set; }
    }
}
