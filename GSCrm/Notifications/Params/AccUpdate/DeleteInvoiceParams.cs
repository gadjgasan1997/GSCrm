using GSCrm.Models;

namespace GSCrm.Notifications.Params.AccUpdate
{
    public class DeleteInvoiceParams : AccUpdateParams
    {
        public AccountInvoice RemovedInvoice { get; set; }
        public DeleteInvoiceParams()
        {
            AccUpdateType = Auxiliary.AccUpdateType.DeleteInvoice;
        }
    }
}
