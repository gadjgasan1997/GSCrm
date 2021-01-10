using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels.Notifications;
using GSCrm.Notifications.Auxiliary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GSCrm.Mapping.Notifications
{
    public abstract class AccUpdateNotMap<TUpdateNotViewModel> : UserNotificationMap
            where TUpdateNotViewModel : AccUpdateNotViewModel, new()
    {
        protected AccUpdateNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public virtual TUpdateNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot, AccUpdateType parsedUpdateType)
        {
            // Простановка базовых параметров
            TUpdateNotViewModel updateNotViewModel = GetNewNotViewModel<TUpdateNotViewModel>(userNot, inboxNot);
            updateNotViewModel.AccUpdateType = parsedUpdateType;
            if (Guid.TryParse(inboxNot.SourceId, out Guid sourceId))
                updateNotViewModel.OwnerOrg = context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == sourceId);
            if (Guid.TryParse(inboxNot.Attrib2, out Guid changedAccountId))
                updateNotViewModel.ChangedAccount = context.Accounts.AsNoTracking().FirstOrDefault(i => i.Id == changedAccountId);
            return updateNotViewModel;
        }
    }
}
