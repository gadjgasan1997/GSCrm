using System.Collections.Generic;

namespace GSCrm.Models.ViewModels
{
    public class AccountsViewModel : BaseViewModel
    {
        public string CurrentAccountsSearchName { get; set; }
        public string CurrentAccountsSearchType { get; set; }
        public string AllAccountsSearchName { get; set; }
        public string AllAccountsSearchType { get; set; }
        public Dictionary<string, string> CurrentAccountsSearchNameCash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> CurrentAccountsSearchTypeCash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> AllAccountsSearchNameCash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> AllAccountsSearchTypeCash { get; set; } = new Dictionary<string, string>();
        public string PrimaryOrganizationName { get; set; }
        public IEnumerable<AccountViewModel> CurrentAccounts { get; set; }
        public IEnumerable<AccountViewModel> AllAccounts { get; set; }
    }
}
