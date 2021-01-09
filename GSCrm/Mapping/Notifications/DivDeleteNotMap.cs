using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels.Notifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GSCrm.Mapping.Notifications
{
    public class DivDeleteNotMap : UserNotificationMap
    {
        public DivDeleteNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public DivDeleteNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot)
        {
            Organization organization = context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == Guid.Parse(inboxNot.SourceId));
            DivDeleteNotViewModel divDeleteNotViewModel = GetNewNotViewModel<DivDeleteNotViewModel>(userNot, inboxNot);
            divDeleteNotViewModel.Organization = organization;
            divDeleteNotViewModel.RemovedDivisionName = inboxNot.Attrib1;
            return divDeleteNotViewModel;
        }
    }
}
