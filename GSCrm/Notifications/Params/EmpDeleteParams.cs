﻿using GSCrm.Models;

namespace GSCrm.Notifications.Params
{
    public class EmpDeleteParams : INotificationParams
    {
        public Organization Organization { get; set; }
        public string OrganizationUrl { get; set; }
        public Employee RemovedEmployee { get; set; }
    }
}
