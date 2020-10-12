using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace GSCrm.Helpers
{
    public static class AccountHelpers
    {
        /// <summary>
        /// Метод возвращает список всех контактов клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<AccountContact> GetContacts(this AccountViewModel accountViewModel, ApplicationDbContext context)
            => context.AccountContacts.Where(acId => acId.AccountId == accountViewModel.Id).ToList();

        /// <summary>
        /// Методы возвращают список всех адресов клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<AccountAddress> GetAddresses(this AccountViewModel accountViewModel, ApplicationDbContext context)
            => context.AccountAddresses.Where(acId => acId.AccountId == accountViewModel.Id).ToList();
        public static List<AccountAddress> GetAddresses(this Account account, ApplicationDbContext context)
            => context.AccountAddresses.Where(acId => acId.AccountId == account.Id).ToList();

        /// <summary>
        /// Метод возвращает список всех банковских реквизитов клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<AccountInvoice> GetInvoices(this AccountViewModel accountViewModel, ApplicationDbContext context)
            => context.AccountInvoices.Where(acId => acId.AccountId == accountViewModel.Id).ToList();

        /// <summary>
        /// Метод возвращает список всех сделок клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<AccountQuote> GetQuotes(this AccountViewModel accountViewModel, ApplicationDbContext context)
            => context.AccountQuotes.Where(acId => acId.AccountId == accountViewModel.Id).ToList();

        /// <summary>
        /// Метод возвращает список всех менеджеров клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<AccountManager> GetManagers(this AccountViewModel accountViewModel, ApplicationDbContext context)
            => context.AccountManagers.Where(acId => acId.AccountId == accountViewModel.Id).ToList();

        /// <summary>
        /// Метод возвращает полное имя физического лица
        /// </summary>
        /// <param name="accountViewModel"></param>
        /// <returns></returns>
        public static string GetIndividualFullName(this AccountViewModel accountViewModel)
            => $"{accountViewModel.LastName} {accountViewModel.FirstName} {accountViewModel.MiddleName}";
    }
}
