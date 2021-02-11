using System;
using System.Collections.Generic;

namespace GSCrm.Models.ViewModels
{
    public class AccountViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Site { get; set; }
        public string INN { get; set; }
        public string KPP { get; set; }
        public string OKPO { get; set; }
        public string OGRN { get; set; }
        public string Country { get; set; }
        public string LegalAddress { get; set; }
        /// <summary>
        /// Id сотрудника, который выбирается в качестве нового основного менеджера
        /// </summary>
        public string NewPrimaryManagerId { get; set; }
        public string PrimaryManagerInitialName { get; set; }
        public AccountStatus AccountStatus { get; set; }
        public string AccountType { get; set; }
        public string PrimaryContactId { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        /// <summary>
        /// Признак, использующийся при создании организации, указывающий, что необходимо использовать себя как основного менеджера
        /// </summary>
        public bool AppointMe { get; set; }

        /// <summary>
        /// Поиск по контактам клиента
        /// </summary>
        #region Account Contacts Search
        public string SearchContactFullName { get; set; }

        public string SearchContactType { get; set; }

        public string SearchContactEmail { get; set; }

        public string SearchContactPhoneNumber { get; set; }

        public bool SearchContactPrimary { get; set; }
        #endregion

        /// <summary>
        /// Поиск по адресам клиента
        /// </summary>
        #region Account Addresses Search
        public string SearchAddressCountry { get; set; }

        public string SearchAddressRegion { get; set; }

        public string SearchAddressCity { get; set; }

        public string SearchAddressStreet { get; set; }
        
        public string SearchAddressHouse { get; set; }
        
        public string SearchAddressType { get; set; }
        #endregion

        /// <summary>
        /// Поиск по банковским реквизитам клиента
        /// </summary>
        #region Account Invoices Search
        public string SearchInvoiceBankName { get; set; }

        public string SearchInvoiceCity { get; set; }

        public string SearchInvoiceCheckingAccount { get; set; }

        public string SearchInvoiceCorrespondentAccount { get; set; }

        public string SearchInvoiceBIC { get; set; }

        public string SearchInvoiceSWIFT { get; set; }
        #endregion

        /// <summary>
        /// Поиск по всем сотрудникам организации, создавшей клиента в модальном окне управления командой
        /// </summary>
        #region Account All Managers Search
        public string SearchAllManagersName { get; set; }

        public string SearchAllManagersDivision { get; set; }

        public string SearchAllManagersPosition { get; set; }
        #endregion

        /// <summary>
        /// Поиск менеджера из команды по клиенту
        /// </summary>
        #region Account Selected Managers Search
        public string SearchSelectedManagersName { get; set; }

        public string SearchSelectedManagersPosition { get; set; }

        public string SearchSelectedManagersPhone { get; set; }
        #endregion

        #region Linked Lists
        public List<AccountContactViewModel> AccountContacts { get; set; }
        public List<AccountAddressViewModel> AccountAddresses { get; set; }
        public List<AccountAddressViewModel> AllAccountAddresses { get; set; }
        public List<AccountInvoiceViewModel> AccountInvoices { get; set; }
        public List<AccountQuoteViewModel> AccountQuotes { get; set; }
        public List<AccountManagerViewModel> AccountManagers { get; set; }
        /// <summary>
        /// Список всех сотрудников организации, создавшей клиента
        /// </summary>
        public List<Employee> AllAccountOwnerOrgEmployees { get; set; }
        #endregion
    }
}
