using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels.Notifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GSCrm.Mapping.Notifications
{
    public class EmpDeleteNotMap : UserNotificationMap
    {
        public EmpDeleteNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public EmpDeleteNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot)
        {
            EmpDeleteNotViewModel empDeleteNotViewModel = GetNewNotViewModel<EmpDeleteNotViewModel>(userNot, inboxNot);
            if (Guid.TryParse(inboxNot.SourceId, out Guid sourceId))
                empDeleteNotViewModel.Organization = context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == sourceId);
            empDeleteNotViewModel.RemovedEmployeeName = inboxNot.Attrib1;
            return empDeleteNotViewModel;
        }
    }
}
