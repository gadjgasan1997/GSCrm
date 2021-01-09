using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels.Notifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GSCrm.Mapping.Notifications
{
    public class PosDeleteNotMap : UserNotificationMap
    {
        public PosDeleteNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public PosDeleteNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot)
        {
            Organization organization = context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == Guid.Parse(inboxNot.SourceId));
            PosDeleteNotViewModel posDeleteNotViewModel = GetNewNotViewModel<PosDeleteNotViewModel>(userNot, inboxNot);
            posDeleteNotViewModel.Organization = organization;
            posDeleteNotViewModel.RemovedPositionName = inboxNot.Attrib1;
            posDeleteNotViewModel.IsPrimary = bool.Parse(inboxNot.Attrib2);
            return posDeleteNotViewModel;
        }
    }
}
