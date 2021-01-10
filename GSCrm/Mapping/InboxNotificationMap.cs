using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;

namespace GSCrm.Mapping
{
    public class InboxNotificationMap : BaseMap<UserNotification, UserNotificationViewModel>
    {
        public InboxNotificationMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }
    }
}
