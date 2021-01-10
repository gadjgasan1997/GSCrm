using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Models.ViewModels.Notifications.AccUpdate;
using GSCrm.Notifications.Auxiliary;

namespace GSCrm.Mapping.Notifications.AccUpdate
{
    public class UpdateAddressNotMap : AccUpdateNotMap<UpdateAddressNotViewModel>
    {
        public UpdateAddressNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override UpdateAddressNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot, AccUpdateType parsedUpdateType)
        {
            UpdateAddressNotViewModel updateAddressNotViewModel = base.DataToViewModel(userNot, inboxNot, parsedUpdateType);
            if (!string.IsNullOrEmpty(inboxNot.Attrib3))
                updateAddressNotViewModel.OldAccountAddress = inboxNot.ReadObjectFromAttr3<AccountAddress>();
            if (!string.IsNullOrEmpty(inboxNot.Attrib4))
                updateAddressNotViewModel.NewAccountAddress = inboxNot.ReadObjectFromAttr4<AccountAddress>();
            return updateAddressNotViewModel;
        }
    }
}
