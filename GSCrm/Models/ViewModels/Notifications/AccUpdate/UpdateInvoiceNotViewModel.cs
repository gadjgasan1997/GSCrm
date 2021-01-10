namespace GSCrm.Models.ViewModels.Notifications.AccUpdate
{
    public class UpdateInvoiceNotViewModel : AccUpdateNotViewModel
    {
        public AccountInvoice OldAccountInvoice { get; set; }
        public AccountInvoice NewAccountInvoice { get; set; }
    }
}
