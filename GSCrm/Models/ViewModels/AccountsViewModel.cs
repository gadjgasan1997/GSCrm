using Newtonsoft.Json;
using System.Collections.Generic;

namespace GSCrm.Models.ViewModels
{
    public class AccountsViewModel : BaseViewModel
    {
        /// <summary>
        /// Название организации, выбранной пользователем в качестве основной
        /// </summary>
        public OrganizationViewModel PrimaryOrganization { get; set; }

        /// <summary>
        /// Поиск по клиентам основной организации пользователя
        /// </summary>
        #region Current Accounts Search
        public string CurrentAccountsSearchName { get; set; }

        public string CurrentAccountsSearchType { get; set; }
        #endregion

        /// <summary>
        /// Поиск по клиентам всех организаций, в которых состоит пользователь
        /// </summary>
        #region All Accounts Search
        public string AllAccountsSearchName { get; set; }

        public string AllAccountsSearchType { get; set; }
        #endregion

        #region Linked Lists
        public IEnumerable<AccountViewModel> CurrentAccounts { get; set; }
        public IEnumerable<AccountViewModel> AllAccounts { get; set; }
        public IEnumerable<OrganizationViewModel> UserOrganizations { get; set; }
        #endregion
    }
}
