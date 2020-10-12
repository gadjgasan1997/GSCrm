namespace GSCrm.Models.ViewModels
{
    public class AccountInvoiceViewModel : BaseViewModel
    {
        public string AccountId { get; set; }
        public string BankName { get; set; }
        public string City { get; set; }
        public string CheckingAccount { get; set; }
        public string CorrespondentAccount { get; set; }
        public string BIC { get; set; }
        public string SWIFT { get; set; }
    }
}
