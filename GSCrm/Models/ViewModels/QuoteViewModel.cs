using System;

namespace GSCrm.Models.ViewModels
{
    public class QuoteViewModel : BaseViewModel
    {
        public string SelectedQuotesTab { get; set; }
        public string OrganizationId { get; set; }
        public string AccountName { get; set; }
        public string ManagerInitialName { get; set; }
        public string Number { get; set; }
        public string Amount { get; set; }
        public QuoteStatus QuoteStatus { get; set; }
    }
}
