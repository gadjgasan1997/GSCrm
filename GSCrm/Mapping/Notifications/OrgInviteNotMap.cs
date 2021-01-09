using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels.Notifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GSCrm.Mapping.Notifications
{
    public class OrgInviteNotMap : UserNotificationMap
    {
        public OrgInviteNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public OrgInviteNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot)
        {
            Organization inviteOrg = context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == Guid.Parse(inboxNot.SourceId));
            OrgInviteNotViewModel orgInviteNotViewModel = GetNewNotViewModel<OrgInviteNotViewModel>(userNot, inboxNot);
            orgInviteNotViewModel.InviteOrg = inviteOrg;
            return orgInviteNotViewModel;
        }
    }
}
