using System;
using System.Linq;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels.Notifications;
using GSCrm.Notifications.Auxiliary;
using Microsoft.EntityFrameworkCore;

namespace GSCrm.Mapping.Notifications
{
    public class AccTeamManagementNotMap : UserNotificationMap
    {
        public AccTeamManagementNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public AccTeamManagementNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot)
        {
            AccTeamManagementNotViewModel accDeleteNotViewModel = GetNewNotViewModel<AccTeamManagementNotViewModel>(userNot, inboxNot);
            if (Guid.TryParse(inboxNot.SourceId, out Guid sourceId))
                accDeleteNotViewModel.OwnerOrg = context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == sourceId);
            if (Guid.TryParse(inboxNot.Attrib1, out Guid accountId))
                accDeleteNotViewModel.Account = context.Accounts.AsNoTracking().FirstOrDefault(i => i.Id == accountId);
            if (Enum.TryParse(inboxNot.Attrib2, out AccTeamManagementNotType accTeamManagementNotType))
                accDeleteNotViewModel.AccTeamManagementNotType = accTeamManagementNotType;
            return accDeleteNotViewModel;
        }
    }
}
