using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels.Notifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GSCrm.Mapping.Notifications
{
    public class AccDeleteNotMap : UserNotificationMap
    {
        public AccDeleteNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public AccDeleteNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot)
        {
            AccDeleteNotViewModel accDeleteNotViewModel = GetNewNotViewModel<AccDeleteNotViewModel>(userNot, inboxNot);
            if (Guid.TryParse(inboxNot.SourceId, out Guid sourceId))
                accDeleteNotViewModel.Organization = context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == sourceId);
            accDeleteNotViewModel.RemovedAccountName = inboxNot.Attrib1;
            return accDeleteNotViewModel;
        }
    }
}
