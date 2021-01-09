using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels.Notifications;
using GSCrm.Notifications.Auxiliary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GSCrm.Mapping.Notifications
{
    public abstract class EmpUpdateNotMap<TUpdateNotViewModel> : UserNotificationMap
            where TUpdateNotViewModel : EmpUpdateNotViewModel, new()
    {
        public EmpUpdateNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public virtual TUpdateNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot, EmpUpdateType parsedUpdateType)
        {
            // Простановка базовых параметров
            TUpdateNotViewModel updateNotViewModel = GetNewNotViewModel<TUpdateNotViewModel>(userNot, inboxNot);
            updateNotViewModel.EmpUpdateType = parsedUpdateType;
            if (Guid.TryParse(inboxNot.SourceId, out Guid sourceId))
                updateNotViewModel.Organization = context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == sourceId);
            if (Guid.TryParse(inboxNot.Attrib2, out Guid changedEmployeeId))
                updateNotViewModel.ChangedEmployee = context.Employees.AsNoTracking().FirstOrDefault(i => i.Id == changedEmployeeId);
            return updateNotViewModel;
        }
    }
}
