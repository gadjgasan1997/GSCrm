using System.Collections.Generic;

namespace GSCrm.Models.ViewModels
{
    public class QuotesViewModel : BaseViewModel
    {
        public string PrimaryOrganizationName { get; set; }
        public string AllQuotesSearchName { get; set; }
        public string CurrentQuotesSearchName { get; set; }
        public IEnumerable<QuoteViewModel> AllQuotes { get; set; }
        public IEnumerable<QuoteViewModel> CurrentQuotes { get; set; }
    }
}
