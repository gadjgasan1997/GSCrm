using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Models.ViewModels.Notifications.AccUpdate;
using GSCrm.Notifications.Auxiliary;
using System;

namespace GSCrm.Mapping.Notifications.AccUpdate
{
    public class DeleteAddressNotMap : AccUpdateNotMap<DeleteAddressNotViewModel>
    {
        public DeleteAddressNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override DeleteAddressNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot, AccUpdateType parsedUpdateType)
        {
            DeleteAddressNotViewModel deleteAddressNotViewModel = base.DataToViewModel(userNot, inboxNot, parsedUpdateType);
            if (!string.IsNullOrEmpty(inboxNot.Attrib3))
                deleteAddressNotViewModel.RemovedAddress = inboxNot.ReadObjectFromAttr3<AccountAddress>();
            return deleteAddressNotViewModel;
        }
    }
}
