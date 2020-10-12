using System;
using System.Collections.Generic;

namespace GSCrm.Models
{
    public class Account : BaseDataModel
    {
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }
        public string INN { get; set; }
        public string KPP { get; set; }
        public string OKPO { get; set; }
        public string OGRN { get; set; }
        public string Site { get; set; }
        public Guid ParentAccountId { get; set; }
        public Guid PrimaryContactId { get; set; }
        public Guid LegalAddressId { get; set; }
        public Guid PrimaryManagerId { get; set; }
        public AccountStatus AccountStatus { get; set; }
        public AccountType AccountType { get; set; }

        public List<AccountContact> AccountContacts { get; set; }
        public List<AccountAddress> AccountAddresses { get; set; }
        public List<AccountQuote> AccountQuotes { get; set; }
        public List<AccountInvoice> AccountInvoices { get; set; }
        public List<AccountManager> AccountManagers { get; set; }

        public Account()
        {
            AccountContacts = new List<AccountContact>();
            AccountAddresses = new List<AccountAddress>();
            AccountQuotes = new List<AccountQuote>();
            AccountInvoices = new List<AccountInvoice>();
            AccountManagers = new List<AccountManager>();
        }
    }

    public enum AccountStatus
    {
        none,
        Lock,
        Active
    }

    public enum AccountType
    {
        None,
        Individual,
        IndividualEntrepreneur,
        LegalEntity
    }
}
