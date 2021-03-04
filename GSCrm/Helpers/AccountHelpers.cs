using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace GSCrm.Helpers
{
    public static class AccountHelpers
    {
        #region Contacts
        public static List<AccountContact> GetContacts(this AccountViewModel accountViewModel, ApplicationDbContext context)
            => context.AccountContacts.AsNoTracking().Where(acId => acId.AccountId == accountViewModel.Id).ToList();
        public static List<AccountContact> GetContacts(this Account account, ApplicationDbContext context)
            => context.AccountContacts.AsNoTracking().Where(acId => acId.AccountId == account.Id).ToList();
        #endregion

        #region Addresses
        public static List<AccountAddress> GetAddresses(this AccountViewModel accountViewModel, ApplicationDbContext context)
            => context.AccountAddresses.AsNoTracking().Where(acId => acId.AccountId == accountViewModel.Id).ToList();
        public static List<AccountAddress> GetAddresses(this Account account, ApplicationDbContext context)
            => context.AccountAddresses.AsNoTracking().Where(acId => acId.AccountId == account.Id).ToList();
        #endregion

        #region Invoices
        public static List<AccountInvoice> GetInvoices(this AccountViewModel accountViewModel, ApplicationDbContext context)
            => context.AccountInvoices.AsNoTracking().Where(acId => acId.AccountId == accountViewModel.Id).ToList();
        public static List<AccountInvoice> GetInvoices(this Account account, ApplicationDbContext context)
            => context.AccountInvoices.AsNoTracking().Where(acId => acId.AccountId == account.Id).ToList();
        #endregion

        #region Quotes
        public static List<AccountQuote> GetQuotes(this AccountViewModel accountViewModel, ApplicationDbContext context)
            => context.AccountQuotes.AsNoTracking().Where(acId => acId.AccountId == accountViewModel.Id).ToList();
        #endregion

        #region Managers
        public static List<AccountManager> GetAccTeam(this AccountViewModel accountViewModel, ApplicationDbContext context)
            => context.AccountManagers.AsNoTracking().Where(acId => acId.AccountId == accountViewModel.Id).ToList();
        public static List<AccountManager> GetAccTeam(this Account account, ApplicationDbContext context)
            => context.AccountManagers.AsNoTracking().Where(acId => acId.AccountId == account.Id).ToList();
        public static List<Employee> GetManagers(this Account account, ApplicationDbContext context)
        {
            return context.AccountManagers.AsNoTracking()
                .Include(accMan => accMan.Manager)
                .Where(accMan => accMan.AccountId == account.Id)
                .Select(accMan => accMan.Manager).ToList();
        }
        #endregion

        /// <summary>
        /// Метод возвращает полное имя физического лица
        /// </summary>
        /// <param name="accountViewModel"></param>
        /// <returns></returns>
        public static string GetIndividualFullName(this AccountViewModel accountViewModel)
            => $"{accountViewModel.LastName} {accountViewModel.FirstName} {accountViewModel.MiddleName}";

        public static AccountViewModel Refresh(this AccountViewModel accountViewModel, AccountViewModel cachedViewModel)
        {
            accountViewModel.SearchContactFullName = cachedViewModel.SearchContactFullName;
            accountViewModel.SearchContactType = cachedViewModel.SearchContactType;
            accountViewModel.SearchContactEmail = cachedViewModel.SearchContactEmail;
            accountViewModel.SearchContactPhoneNumber = cachedViewModel.SearchContactPhoneNumber;
            accountViewModel.SearchContactPrimary = cachedViewModel.SearchContactPrimary;
            accountViewModel.SearchAddressCountry = cachedViewModel.SearchAddressCountry;
            accountViewModel.SearchAddressRegion = cachedViewModel.SearchAddressRegion;
            accountViewModel.SearchAddressCity = cachedViewModel.SearchAddressCity;
            accountViewModel.SearchAddressStreet = cachedViewModel.SearchAddressStreet;
            accountViewModel.SearchAddressHouse = cachedViewModel.SearchAddressHouse;
            accountViewModel.SearchAddressType = cachedViewModel.SearchAddressType;
            accountViewModel.SearchInvoiceBankName = cachedViewModel.SearchInvoiceBankName;
            accountViewModel.SearchInvoiceCity = cachedViewModel.SearchInvoiceCity;
            accountViewModel.SearchInvoiceCheckingAccount = cachedViewModel.SearchInvoiceCheckingAccount;
            accountViewModel.SearchInvoiceCorrespondentAccount = cachedViewModel.SearchInvoiceCorrespondentAccount;
            accountViewModel.SearchInvoiceBIC = cachedViewModel.SearchInvoiceBIC;
            accountViewModel.SearchInvoiceSWIFT = cachedViewModel.SearchInvoiceSWIFT;
            accountViewModel.SearchAllManagersName = cachedViewModel.SearchAllManagersName;
            accountViewModel.SearchAllManagersDivision = cachedViewModel.SearchAllManagersDivision;
            accountViewModel.SearchAllManagersPosition = cachedViewModel.SearchAllManagersPosition;
            accountViewModel.SearchSelectedManagersName = cachedViewModel.SearchSelectedManagersName;
            accountViewModel.SearchSelectedManagersPosition = cachedViewModel.SearchSelectedManagersPosition;
            accountViewModel.SearchSelectedManagersPhone = cachedViewModel.SearchSelectedManagersPhone;
            return accountViewModel;
        }

        public static AccountsViewModel Refresh(this AccountsViewModel accountsViewModel, AccountsViewModel cachedViewModel)
        {
            accountsViewModel.AllAccountsSearchName = cachedViewModel.AllAccountsSearchName;
            accountsViewModel.CurrentAccountsSearchName = cachedViewModel.CurrentAccountsSearchName;
            accountsViewModel.AllAccountsSearchType = cachedViewModel.AllAccountsSearchType;
            accountsViewModel.CurrentAccountsSearchType = cachedViewModel.CurrentAccountsSearchType;
            return accountsViewModel;
        }

        public static void Normalize(this AccountViewModel accountViewModel)
        {
            accountViewModel.Name = accountViewModel.Name?.TrimStartAndEnd();
            accountViewModel.FirstName = accountViewModel.FirstName?.TrimStartAndEnd();
            accountViewModel.LastName = accountViewModel.LastName?.TrimStartAndEnd();
            accountViewModel.MiddleName = accountViewModel.MiddleName?.TrimStartAndEnd();
            accountViewModel.Site = accountViewModel.Site?.TrimStartAndEnd();
            accountViewModel.INN = accountViewModel.INN?.TrimStartAndEnd();
            accountViewModel.KPP = accountViewModel.KPP?.TrimStartAndEnd();
            accountViewModel.OKPO = accountViewModel.OKPO?.TrimStartAndEnd();
            accountViewModel.OGRN = accountViewModel.OGRN?.TrimStartAndEnd();
            accountViewModel.Country = accountViewModel.Country?.TrimStartAndEnd();
            accountViewModel.LegalAddress = accountViewModel.LegalAddress?.TrimStartAndEnd();
            accountViewModel.PrimaryManagerInitialName = accountViewModel.PrimaryManagerInitialName?.TrimStartAndEnd();
        }

        public static void NormalizeSearch(this AccountsViewModel accountsViewModel)
        {
            accountsViewModel.AllAccountsSearchName = accountsViewModel.AllAccountsSearchName?.ToLower().TrimStartAndEnd();
            accountsViewModel.CurrentAccountsSearchName = accountsViewModel.CurrentAccountsSearchName?.ToLower().TrimStartAndEnd();
        }

        public static void NormalizeSearch(this AccountViewModel accountViewModel)
        {
            accountViewModel.SearchContactFullName = accountViewModel.SearchContactFullName?.ToLower().TrimStartAndEnd();
            accountViewModel.SearchContactEmail = accountViewModel.SearchContactEmail?.ToLower().TrimStartAndEnd();
            accountViewModel.SearchContactPhoneNumber = accountViewModel.SearchContactPhoneNumber?.ToLower().TrimStartAndEnd();
            accountViewModel.SearchAddressCountry = accountViewModel.SearchAddressCountry?.ToLower().TrimStartAndEnd();
            accountViewModel.SearchAddressRegion = accountViewModel.SearchAddressRegion?.ToLower().TrimStartAndEnd();
            accountViewModel.SearchAddressCity = accountViewModel.SearchAddressCity?.ToLower().TrimStartAndEnd();
            accountViewModel.SearchAddressStreet = accountViewModel.SearchAddressStreet?.ToLower().TrimStartAndEnd();
            accountViewModel.SearchAddressHouse = accountViewModel.SearchAddressHouse?.ToLower().TrimStartAndEnd();
            accountViewModel.SearchInvoiceBankName = accountViewModel.SearchInvoiceBankName?.ToLower().TrimStartAndEnd();
            accountViewModel.SearchInvoiceCity = accountViewModel.SearchInvoiceCity?.ToLower().TrimStartAndEnd();
            accountViewModel.SearchInvoiceCheckingAccount = accountViewModel.SearchInvoiceCheckingAccount?.ToLower().TrimStartAndEnd();
            accountViewModel.SearchInvoiceCorrespondentAccount = accountViewModel.SearchInvoiceCorrespondentAccount?.ToLower().TrimStartAndEnd();
            accountViewModel.SearchInvoiceBIC = accountViewModel.SearchInvoiceBIC?.ToLower().TrimStartAndEnd();
            accountViewModel.SearchInvoiceSWIFT = accountViewModel.SearchInvoiceSWIFT?.ToLower().TrimStartAndEnd();
            accountViewModel.SearchAllManagersName = accountViewModel.SearchAllManagersName?.ToLower().TrimStartAndEnd();
            accountViewModel.SearchAllManagersDivision = accountViewModel.SearchAllManagersDivision?.ToLower().TrimStartAndEnd();
            accountViewModel.SearchAllManagersPosition = accountViewModel.SearchAllManagersPosition?.ToLower().TrimStartAndEnd();
            accountViewModel.SearchSelectedManagersName = accountViewModel.SearchSelectedManagersName?.ToLower().TrimStartAndEnd();
            accountViewModel.SearchSelectedManagersPosition = accountViewModel.SearchSelectedManagersPosition?.ToLower().TrimStartAndEnd();
            accountViewModel.SearchSelectedManagersPhone = accountViewModel.SearchSelectedManagersPhone?.ToLower().TrimStartAndEnd();
        }
    }
}
