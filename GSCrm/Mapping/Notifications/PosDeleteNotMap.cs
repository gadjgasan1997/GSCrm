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
            PosDeleteNotViewModel posDeleteNotViewModel = GetNewNotViewModel<PosDeleteNotViewModel>(userNot, inboxNot);
            if (Guid.TryParse(inboxNot.SourceId, out Guid sourceId))
                posDeleteNotViewModel.Organization = context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == sourceId);
            posDeleteNotViewModel.RemovedPositionName = inboxNot.Attrib1;
            if (bool.TryParse(inboxNot.Attrib2, out bool isPrimary))
                posDeleteNotViewModel.IsPrimary = isPrimary;
            return posDeleteNotViewModel;
        }
    }
}
