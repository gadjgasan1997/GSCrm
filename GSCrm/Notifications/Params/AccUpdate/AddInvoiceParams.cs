using GSCrm.Models;

namespace GSCrm.Notifications.Params.AccUpdate
{
    public class AddInvoiceParams : AccUpdateParams
    {
        public AccountInvoice NewAccountInvoice { get; set; }
        public AddInvoiceParams()
        {
            AccUpdateType = Auxiliary.AccUpdateType.AddInvoice;
        }
    }
}
