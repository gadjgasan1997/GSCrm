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
    }
}
