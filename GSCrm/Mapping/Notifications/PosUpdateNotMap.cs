using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels.Notifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GSCrm.Mapping.Notifications
{
    public class PosUpdateNotMap : UserNotificationMap
    {
        public PosUpdateNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public PosUpdateNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot)
        {
            PosUpdateNotViewModel posUpdateNotViewModel = GetNewNotViewModel<PosUpdateNotViewModel>(userNot, inboxNot);
            if (Guid.TryParse(inboxNot.SourceId, out Guid sourceId))
                posUpdateNotViewModel.Organization = context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == sourceId);
            if (Guid.TryParse(inboxNot.Attrib1, out Guid changedPositionId))
                posUpdateNotViewModel.ChangedPosition = context.Positions.AsNoTracking().FirstOrDefault(pos => pos.Id == changedPositionId);
            if (bool.TryParse(inboxNot.Attrib2, out bool divisionChanged))
                posUpdateNotViewModel.DivisionChanged = divisionChanged;
            if (bool.TryParse(inboxNot.Attrib3, out bool isPrimary))
                posUpdateNotViewModel.IsPrimary = isPrimary;
            return posUpdateNotViewModel;
        }
    }
}
