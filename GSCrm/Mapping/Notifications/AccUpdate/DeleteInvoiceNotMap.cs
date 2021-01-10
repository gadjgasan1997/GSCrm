using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Notifications.Auxiliary;
using GSCrm.Models.ViewModels.Notifications.AccUpdate;

namespace GSCrm.Mapping.Notifications.AccUpdate
{
    public class DeleteInvoiceNotMap : AccUpdateNotMap<DeleteInvoiceNotViewModel>
    {
        public DeleteInvoiceNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override DeleteInvoiceNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot, AccUpdateType parsedUpdateType)
        {
            DeleteInvoiceNotViewModel deleteInvoiceNotViewModel = base.DataToViewModel(userNot, inboxNot, parsedUpdateType);
            if (!string.IsNullOrEmpty(inboxNot.Attrib3))
                deleteInvoiceNotViewModel.RemovedInvoice = inboxNot.ReadObjectFromAttr3<AccountInvoice>();
            return deleteInvoiceNotViewModel;
        }
    }
}
