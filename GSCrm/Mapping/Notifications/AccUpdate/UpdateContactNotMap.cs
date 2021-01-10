using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Models.ViewModels.Notifications.AccUpdate;
using GSCrm.Notifications.Auxiliary;
using System;

namespace GSCrm.Mapping.Notifications.AccUpdate
{
    public class UpdateContactNotMap : AccUpdateNotMap<UpdateContactNotViewModel>
    {
        public UpdateContactNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override UpdateContactNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot, AccUpdateType parsedUpdateType)
        {
            UpdateContactNotViewModel deleteContactNotViewModel = base.DataToViewModel(userNot, inboxNot, parsedUpdateType);
            if (!string.IsNullOrEmpty(inboxNot.Attrib3))
                deleteContactNotViewModel.OldAccountContact = inboxNot.ReadObjectFromAttr3<AccountContact>();
            if (!string.IsNullOrEmpty(inboxNot.Attrib4))
                deleteContactNotViewModel.NewAccountContact = inboxNot.ReadObjectFromAttr4<AccountContact>();
            return deleteContactNotViewModel;
        }
    }
}
