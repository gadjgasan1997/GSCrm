using GSCrm.Models;

namespace GSCrm.Notifications.Params.AccUpdate
{
    public class UpdateInvoiceParams : AccUpdateParams
    {
        public AccountInvoice OldAccountInvoice { get; set; }
        public AccountInvoice NewAccountInvoice { get; set; }
        public UpdateInvoiceParams()
        {
            AccUpdateType = Auxiliary.AccUpdateType.UpdateInvoice;
        }
    }
}
