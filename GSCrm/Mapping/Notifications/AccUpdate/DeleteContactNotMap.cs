using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Models.ViewModels.Notifications.AccUpdate;
using GSCrm.Notifications.Auxiliary;
using System;

namespace GSCrm.Mapping.Notifications.AccUpdate
{
    public class DeleteContactNotMap : AccUpdateNotMap<DeleteContactNotViewModel>
    {
        public DeleteContactNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override DeleteContactNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot, AccUpdateType parsedUpdateType)
        {
            DeleteContactNotViewModel deleteContactNotViewModel = base.DataToViewModel(userNot, inboxNot, parsedUpdateType);
            if (!string.IsNullOrEmpty(inboxNot.Attrib3))
                deleteContactNotViewModel.RemovedContact = inboxNot.ReadObjectFromAttr3<AccountContact>();
            return deleteContactNotViewModel;
        }
    }
}
