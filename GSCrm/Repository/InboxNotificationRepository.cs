using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;

namespace GSCrm.Repository
{
    public class InboxNotificationRepository : BaseRepository<InboxNotification, InboxNotificationViewModel>
    {
        public InboxNotificationRepository(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }
    }
}
