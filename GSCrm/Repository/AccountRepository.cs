using GSCrm.Data;
using GSCrm.Data.ApplicationInfo;
using GSCrm.DataTransformers;
using GSCrm.Helpers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;

namespace GSCrm.Repository
{
    public class AccountRepository : GenericRepository<Account, AccountViewModel, AccountValidatior, AccountTransformer>
    {
        private readonly User currentUser;
        public static AccountViewModel CurrentAccount { get; set; }
        public static string SelectedAccountsTab { get; set; }
        public static Account NewAccount { get; private set; }
        public AccountRepository(ApplicationDbContext context, ResManager resManager)
            : base(context, resManager, new AccountValidatior(context, resManager), new AccountTransformer(context, resManager))
        { }

        public AccountRepository(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager, HttpContext httpContext = null)
            : base(context, viewsInfo, resManager, new AccountValidatior(context, resManager, httpContext), new AccountTransformer(context, resManager, httpContext))
        {
            if (httpContext != null)
                currentUser = httpContext.GetCurrentUser(context);
        }

        #region Override Methods
        public override void FailureUpdateHandler(AccountViewModel accountViewModel, Action<AccountViewModel> handler = null)
        {
            if (TryGetItemById(accountViewModel.Id, out Account account))
            {
                accountViewModel = transformer.DataToViewModel(account);
                accountViewModel = transformer.UpdateViewModelFromCash(accountViewModel);
                AttachManagers(accountViewModel);
                AttachContacts(accountViewModel);
                AttachAddresses(accountViewModel);
                AttachInvoices(accountViewModel);
                AttachQuotes(accountViewModel);
            }
        }
        #endregion

        #region Searching

        /// <summary>
        /// Метод устанавливает значения для поиска по всем клиентам
        /// </summary>
        /// <param name="accountsViewModel"></param>
        /// <returns></returns>
        public void SearchAllAccounts(AccountsViewModel accountsViewModel)
        {
            //viewsInfo.Reset(ALL_ACCS);
            AccountsViewModel accountsViewModelCash = ModelCash<AccountsViewModel>.GetViewModel(currentUser.Id, ALL_ACCS);
            accountsViewModelCash.AllAccountsSearchNameCash.AddOrReplace(currentUser.Id, accountsViewModel.AllAccountsSearchName?.ToLower().TrimStartAndEnd());
            accountsViewModelCash.AllAccountsSearchTypeCash.AddOrReplace(currentUser.Id, accountsViewModel.AllAccountsSearchType);
            ModelCash<AccountsViewModel>.SetViewModel(currentUser.Id, ALL_ACCS, accountsViewModelCash);
        }

        /// <summary>
        /// Метод очищает поиск по всем клиентам
        /// </summary>
        public void ClearAllAccountsSearch()
        {
            AccountsViewModel accountsViewModelCash = ModelCash<AccountsViewModel>.GetViewModel(currentUser.Id, ALL_ACCS);
            accountsViewModelCash.AllAccountsSearchNameCash.AddOrReplace(currentUser.Id, default);
            accountsViewModelCash.AllAccountsSearchTypeCash.AddOrReplace(currentUser.Id, default);
            ModelCash<AccountsViewModel>.SetViewModel(currentUser.Id, ALL_ACCS, accountsViewModelCash);
        }

        /// <summary>
        /// Метод устанавливает значения для поиска по клиентам основной организации пользователя
        /// </summary>
        /// <param name="accountsViewModel"></param>
        /// <returns></returns>
        public void SearchCurrentAccounts(AccountsViewModel accountsViewModel)
        {
            //viewsInfo.Reset(CURRENT_ACCS);
            AccountsViewModel accountsViewModelCash = ModelCash<AccountsViewModel>.GetViewModel(currentUser.Id, CURRENT_ACCS);
            accountsViewModelCash.CurrentAccountsSearchNameCash.AddOrReplace(currentUser.Id, accountsViewModel.CurrentAccountsSearchName?.ToLower().TrimStartAndEnd());
            accountsViewModelCash.CurrentAccountsSearchTypeCash.AddOrReplace(currentUser.Id, accountsViewModel.CurrentAccountsSearchType);
            ModelCash<AccountsViewModel>.SetViewModel(currentUser.Id, CURRENT_ACCS, accountsViewModelCash);
        }

        /// <summary>
        /// Метод очищает поиск по клиентам основной организации пользователя
        /// </summary>
        public void ClearCurrentAccountsSearch()
        {
            AccountsViewModel accountsViewModelCash = ModelCash<AccountsViewModel>.GetViewModel(currentUser.Id, CURRENT_ACCS);
            accountsViewModelCash.CurrentAccountsSearchNameCash.AddOrReplace(currentUser.Id, default);
            accountsViewModelCash.CurrentAccountsSearchTypeCash.AddOrReplace(currentUser.Id, default);
            ModelCash<AccountsViewModel>.SetViewModel(currentUser.Id, CURRENT_ACCS, accountsViewModelCash);
        }

        /// <summary>
        /// Поиск по контактам клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        public void SearchContact(AccountViewModel accountViewModel)
        {
            //viewsInfo.Reset(ACC_CONTACTS);
            AccountViewModel accountViewModelCash = ModelCash<AccountViewModel>.GetViewModel(currentUser.Id, ACC_CONTACTS);
            accountViewModelCash.IdCash.AddOrReplace(currentUser.Id, accountViewModel.Id);
            accountViewModelCash.SearchContactFullNameCash.AddOrReplace(currentUser.Id, accountViewModel.SearchContactFullName?.ToLower().TrimStartAndEnd());
            accountViewModelCash.SearchContactTypeCash.AddOrReplace(currentUser.Id, accountViewModel.SearchContactType);
            accountViewModelCash.SearchContactEmailCash.AddOrReplace(currentUser.Id, accountViewModel.SearchContactEmail?.ToLower().TrimStartAndEnd());
            accountViewModelCash.SearchContactPhoneNumberCash.AddOrReplace(currentUser.Id, accountViewModel.SearchContactPhoneNumber?.ToLower().TrimStartAndEnd());
            accountViewModelCash.SearchContactPrimaryCash.AddOrReplace(currentUser.Id, accountViewModel.SearchContactPrimary);
            ModelCash<AccountViewModel>.SetViewModel(currentUser.Id, ACC_CONTACTS, accountViewModelCash);
        }

        /// <summary>
        /// Очистка поиска по контактам клиента
        /// </summary>
        public void ClearContactSearch()
        {
            AccountViewModel accountViewModelCash = ModelCash<AccountViewModel>.GetViewModel(currentUser.Id, ACC_CONTACTS);
            accountViewModelCash.SearchContactFullNameCash.AddOrReplace(currentUser.Id, default);
            accountViewModelCash.SearchContactTypeCash.AddOrReplace(currentUser.Id, default);
            accountViewModelCash.SearchContactEmailCash.AddOrReplace(currentUser.Id, default);
            accountViewModelCash.SearchContactPhoneNumberCash.AddOrReplace(currentUser.Id, default);
            accountViewModelCash.SearchContactPrimaryCash.AddOrReplace(currentUser.Id, default);
            ModelCash<AccountViewModel>.SetViewModel(currentUser.Id, ACC_CONTACTS, accountViewModelCash);
        }

        /// <summary>
        /// Поиск по адресам клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        public void SearchAddress(AccountViewModel accountViewModel)
        {
            //viewsInfo.Reset(ACC_ADDRESSES);
            AccountViewModel accountViewModelCash = ModelCash<AccountViewModel>.GetViewModel(currentUser.Id, ACC_ADDRESSES);
            accountViewModelCash.SearchAddressTypeCash.AddOrReplace(currentUser.Id, accountViewModel.SearchAddressType);
            accountViewModelCash.SearchAddressCountryCash.AddOrReplace(currentUser.Id, accountViewModel.SearchAddressCountry?.ToLower().TrimStartAndEnd());
            accountViewModelCash.SearchAddressRegionCash.AddOrReplace(currentUser.Id, accountViewModel.SearchAddressRegion?.ToLower().TrimStartAndEnd());
            accountViewModelCash.SearchAddressCityCash.AddOrReplace(currentUser.Id, accountViewModel.SearchAddressCity?.ToLower().TrimStartAndEnd());
            accountViewModelCash.SearchAddressStreetCash.AddOrReplace(currentUser.Id, accountViewModel.SearchAddressStreet?.ToLower().TrimStartAndEnd());
            accountViewModelCash.SearchAddressHouseCash.AddOrReplace(currentUser.Id, accountViewModel.SearchAddressHouse?.ToLower().TrimStartAndEnd());
            ModelCash<AccountViewModel>.SetViewModel(currentUser.Id, ACC_ADDRESSES, accountViewModelCash);
        }

        /// <summary>
        /// Очистка поиска по адресам клиента
        /// </summary>
        public void ClearAddressSearch()
        {
            AccountViewModel accountViewModelCash = ModelCash<AccountViewModel>.GetViewModel(currentUser.Id, ACC_ADDRESSES);
            accountViewModelCash.SearchAddressTypeCash.AddOrReplace(currentUser.Id, default);
            accountViewModelCash.SearchAddressCountryCash.AddOrReplace(currentUser.Id, default);
            accountViewModelCash.SearchAddressRegionCash.AddOrReplace(currentUser.Id, default);
            accountViewModelCash.SearchAddressCityCash.AddOrReplace(currentUser.Id, default);
            accountViewModelCash.SearchAddressStreetCash.AddOrReplace(currentUser.Id, default);
            accountViewModelCash.SearchAddressHouseCash.AddOrReplace(currentUser.Id, default);
            ModelCash<AccountViewModel>.SetViewModel(currentUser.Id, ACC_ADDRESSES, accountViewModelCash);
        }

        /// <summary>
        /// Поиск по банкосвким реквизитам клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        public void SearchInvoice(AccountViewModel accountViewModel)
        {
            //viewsInfo.Reset(ACC_INVOICES);
            AccountViewModel accountViewModelCash = ModelCash<AccountViewModel>.GetViewModel(currentUser.Id, ACC_INVOICES);
            accountViewModelCash.SearchInvoiceBankNameCash.AddOrReplace(currentUser.Id, accountViewModel.SearchInvoiceBankName?.ToLower().TrimStartAndEnd());
            accountViewModelCash.SearchInvoiceCityCash.AddOrReplace(currentUser.Id, accountViewModel.SearchInvoiceCity?.ToLower().TrimStartAndEnd());
            accountViewModelCash.SearchInvoiceCheckingAccountCash.AddOrReplace(currentUser.Id, accountViewModel.SearchInvoiceCheckingAccount?.ToLower().TrimStartAndEnd());
            accountViewModelCash.SearchInvoiceCorrespondentAccountCash.AddOrReplace(currentUser.Id, accountViewModel.SearchInvoiceCorrespondentAccount?.ToLower().TrimStartAndEnd());
            accountViewModelCash.SearchInvoiceBICCash.AddOrReplace(currentUser.Id, accountViewModel.SearchInvoiceBIC?.ToLower().TrimStartAndEnd());
            accountViewModelCash.SearchInvoiceSWIFTCash.AddOrReplace(currentUser.Id, accountViewModel.SearchInvoiceSWIFT?.ToLower().TrimStartAndEnd());
            ModelCash<AccountViewModel>.SetViewModel(currentUser.Id, ACC_INVOICES, accountViewModelCash);
        }

        /// <summary>
        /// Очистка поиска по банкосвким реквизитам клиента
        /// </summary>
        public void ClearInvoiceSearch()
        {
            AccountViewModel accountViewModelCash = ModelCash<AccountViewModel>.GetViewModel(currentUser.Id, ACC_INVOICES);
            accountViewModelCash.SearchInvoiceBankNameCash.AddOrReplace(currentUser.Id, default);
            accountViewModelCash.SearchInvoiceCityCash.AddOrReplace(currentUser.Id, default);
            accountViewModelCash.SearchInvoiceCheckingAccountCash.AddOrReplace(currentUser.Id, default);
            accountViewModelCash.SearchInvoiceCorrespondentAccountCash.AddOrReplace(currentUser.Id, default);
            accountViewModelCash.SearchInvoiceBICCash.AddOrReplace(currentUser.Id, default);
            accountViewModelCash.SearchInvoiceSWIFTCash.AddOrReplace(currentUser.Id, default);
            ModelCash<AccountViewModel>.SetViewModel(currentUser.Id, ACC_INVOICES, accountViewModelCash);
        }

        /// <summary>
        /// Поиск по сделкам клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        public void SearchQuote(AccountViewModel accountViewModel)
        {
            //viewsInfo.Reset(ACC_QUOTES);
            AccountViewModel accountViewModelCash = ModelCash<AccountViewModel>.GetViewModel(currentUser.Id, ACC_QUOTES);
            ModelCash<AccountViewModel>.SetViewModel(currentUser.Id, ACC_QUOTES, accountViewModelCash);
        }

        /// <summary>
        /// Очистка поиска по сделкам клиента
        /// </summary>
        public void ClearQuoteSearch()
        {
            AccountViewModel accountViewModelCash = ModelCash<AccountViewModel>.GetViewModel(currentUser.Id, ACC_QUOTES);
            ModelCash<AccountViewModel>.SetViewModel(currentUser.Id, ACC_QUOTES, accountViewModelCash);
        }

        #endregion

        #region Attaching Accounts

        /// <summary>
        /// Метод добавляет список моделей отображения клиента к модели "AccountsViewModel"
        /// </summary>
        /// <param name="accountsViewModel"></param>
        public void AttachAccounts(ref AccountsViewModel accountsViewModel)
        {
            accountsViewModel.AllAccounts = context.GetAllAccounts(currentUser)
                .TransformToViewModels<Account, AccountViewModel, AccountTransformer>(
                    transformer: new AccountTransformer(context, resManager),
                    limitingFunc: GetLimitedAllAccountsList);

            accountsViewModel.CurrentAccounts = context.GetCurrentAccounts(currentUser)
                .TransformToViewModels<Account, AccountViewModel, AccountTransformer>(
                    transformer: new AccountTransformer(context, resManager),
                    limitingFunc: GetLimitedCurrentAccountsList);
        }

        /// <summary>
        /// Метод ограничивает список клиентов всех организаций
        /// </summary>
        /// <param name="accounts"></param>
        /// <returns></returns>
        private List<Account> GetLimitedAllAccountsList(List<Account> accounts)
        {
            List<Account> limitedAccounts = accounts;
            AccountsViewModel accountsViewModelCash = ModelCash<AccountsViewModel>.GetViewModel(currentUser.Id, ALL_ACCS);
            LimitAllAccsBySearchName(ref limitedAccounts, accountsViewModelCash);
            LimitAllAccsBySearchType(ref limitedAccounts, accountsViewModelCash);
            LimitListByPageNumber(currentUser.Id, ALL_ACCS, ref limitedAccounts);
            return limitedAccounts;
        }

        /// <summary>
        /// Ограничение списка всех клиентов по названию
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="accountsToLimit"></param>
        private void LimitAllAccsBySearchName(ref List<Account> accountsToLimit, AccountsViewModel accountsViewModel)
        {
            string allAccountsSearchName = accountsViewModel.AllAccountsSearchNameCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(allAccountsSearchName))
                accountsToLimit = accountsToLimit.Where(n => n.Name.ToLower().Contains(allAccountsSearchName)).ToList();
        }

        /// <summary>
        /// Ограничение списка всех клиентов по типу
        /// </summary>
        /// <param name="accountsToLimit"></param>
        private void LimitAllAccsBySearchType(ref List<Account> accountsToLimit, AccountsViewModel accountsViewModel)
        {
            string allAccountsSearchType = accountsViewModel.AllAccountsSearchTypeCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(allAccountsSearchType))
                accountsToLimit = accountsToLimit.Where(n => n.AccountType.ToString() == allAccountsSearchType).ToList();
        }

        /// <summary>
        /// Метод ограничивает список клиентов основной организации пользователя
        /// </summary>
        /// <param name="accounts"></param>
        /// <returns></returns>
        private List<Account> GetLimitedCurrentAccountsList(List<Account> accounts)
        {
            List<Account> limitedAccounts = accounts.Where(orgId => orgId.OrganizationId == currentUser.PrimaryOrganizationId).ToList();
            AccountsViewModel accountsViewModelCash = ModelCash<AccountsViewModel>.GetViewModel(currentUser.Id, CURRENT_ACCS);
            LimitCurrentAccsBySearchName(ref limitedAccounts, accountsViewModelCash);
            LimitCurrentAccsBySearchType(ref limitedAccounts, accountsViewModelCash);
            LimitListByPageNumber(currentUser.Id, CURRENT_ACCS, ref limitedAccounts);
            return limitedAccounts;
        }

        /// <summary>
        /// Ограничение списка клиентов основной организации текущего пользователя по названию
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="accountsToLimit"></param>
        private void LimitCurrentAccsBySearchName(ref List<Account> accountsToLimit, AccountsViewModel accountsViewModel)
        {
            string currentAccountsSearchName = accountsViewModel.CurrentAccountsSearchNameCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(currentAccountsSearchName))
                accountsToLimit = accountsToLimit.Where(n => n.Name.ToLower().Contains(currentAccountsSearchName)).ToList();
        }

        /// <summary>
        /// Ограничение списка клиентов основной орагнизации пользователя по типу
        /// </summary>
        /// <param name="accountsToLimit"></param>
        private void LimitCurrentAccsBySearchType(ref List<Account> accountsToLimit, AccountsViewModel accountsViewModel)
        {
            string currentAccountsSearchType = accountsViewModel.CurrentAccountsSearchTypeCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(currentAccountsSearchType))
                accountsToLimit = accountsToLimit.Where(n => n.AccountType.ToString() == currentAccountsSearchType).ToList();
        }

        #endregion

        #region Attaching Contacts

        /// <summary>
        /// Добавляет контакты к клиенту
        /// </summary>
        /// <param name="accountViewModel"></param>
        public void AttachContacts(AccountViewModel accountViewModel)
        {
            accountViewModel.AccountContacts = accountViewModel.GetContacts(context)
                .TransformToViewModels<AccountContact, AccountContactViewModel, AccountContactTransformer>(
                    transformer: new AccountContactTransformer(context, resManager),
                    limitingFunc: GetLimitedAccContactsList);
        }

        private List<AccountContact> GetLimitedAccContactsList(List<AccountContact> accountContacts)
        {
            AccountViewModel accountViewModelCash = ModelCash<AccountViewModel>.GetViewModel(currentUser.Id, ACC_CONTACTS);
            List<AccountContact> limitedAccContacts = accountContacts;
            LimitContactsByFullName(ref limitedAccContacts, accountViewModelCash);
            LimitContactsByType(ref limitedAccContacts, accountViewModelCash);
            LimitContactsByPhoneNumber(ref limitedAccContacts, accountViewModelCash);
            LimitContactsByEmail(ref limitedAccContacts, accountViewModelCash);
            LimitContactsByPrimarySign(ref limitedAccContacts, accountViewModelCash);
            LimitListByPageNumber(currentUser.Id, ACC_CONTACTS, ref limitedAccContacts);
            return limitedAccContacts;
        }

        /// <summary>
        /// Мето ограничивает список контактов клиента по полному имеени
        /// </summary>
        /// <param name="accountContactsToLimit"></param>
        private void LimitContactsByFullName(ref List<AccountContact> accountContactsToLimit, AccountViewModel accountViewModel)
        {
            string searchContactFullName = accountViewModel.SearchContactFullNameCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchContactFullName))
                accountContactsToLimit = accountContactsToLimit.Where(accCon => accCon.GetFullName().ToLower().Contains(searchContactFullName)).ToList();
        }

        /// <summary>
        /// Мето ограничивает список контактов клиента по типу контакта
        /// </summary>
        /// <param name="accountContactsToLimit"></param>
        private void LimitContactsByType(ref List<AccountContact> accountContactsToLimit, AccountViewModel accountViewModel)
        {
            string searchContactType = accountViewModel.SearchContactTypeCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchContactType))
            {
                Func<AccountContact, bool> predicate = accCon => accCon.ContactType == (ContactType)Enum.Parse(typeof(ContactType), searchContactType);
                accountContactsToLimit = accountContactsToLimit.Where(predicate).ToList();
            }
        }

        /// <summary>
        /// Мето ограничивает список контактов клиента по телефону
        /// </summary>
        /// <param name="accountContactsToLimit"></param>
        private void LimitContactsByPhoneNumber(ref List<AccountContact> accountContactsToLimit, AccountViewModel accountViewModel)
        {
            string searchContactPhoneNumber = accountViewModel.SearchContactPhoneNumberCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchContactPhoneNumber))
                accountContactsToLimit = accountContactsToLimit.Where(accCon => accCon.PhoneNumber.ToLower().Contains(searchContactPhoneNumber)).ToList();
        }

        /// <summary>
        /// Мето ограничивает список контактов клиента по почту
        /// </summary>
        /// <param name="accountContactsToLimit"></param>
        private void LimitContactsByEmail(ref List<AccountContact> accountContactsToLimit, AccountViewModel accountViewModel)
        {
            string searchContactEmail = accountViewModel.SearchContactEmailCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchContactEmail))
                accountContactsToLimit = accountContactsToLimit.Where(accCon => accCon.PhoneNumber.ToLower().Contains(searchContactEmail)).ToList();
        }

        /// <summary>
        /// Мето ограничивает список контактов клиента по признаку основного
        /// </summary>
        /// <param name="accountContactsToLimit"></param>
        private void LimitContactsByPrimarySign(ref List<AccountContact> accountContactsToLimit, AccountViewModel accountViewModel)
        {
            Account account = context.Accounts.FirstOrDefault(i => i.Id == accountViewModel.IdCash.GetValueOrDefault(currentUser.Id));
            if (accountViewModel.SearchContactPrimaryCash.GetValueOrDefault(currentUser.Id))
                accountContactsToLimit = accountContactsToLimit.Where(i => i.Id == account.PrimaryContactId).ToList();
        }

        #endregion

        #region Attaching Addresses

        /// <summary>
        /// Добавляет адреса к клиенту
        /// </summary>
        /// <param name="accountViewModel"></param>
        public void AttachAddresses(AccountViewModel accountViewModel)
        {
            List<AccountAddress> accountAddresses = accountViewModel.GetAddresses(context);
            accountViewModel.AccountAddresses = accountAddresses.TransformToViewModels
                <AccountAddress, AccountAddressViewModel, AccountAddressTransformer>(
                    transformer: new AccountAddressTransformer(context, resManager, currentUser),
                    limitingFunc: GetLimitedAccAddressesList);
            accountViewModel.AllAccountAddresses = accountAddresses.GetViewModelsFromData
                <AccountAddress, AccountAddressViewModel, AccountAddressTransformer>(new AccountAddressTransformer(context, resManager, currentUser));
        }

        private List<AccountAddress> GetLimitedAccAddressesList(List<AccountAddress> accountAddresses)
        {
            AccountViewModel accountViewModelCash = ModelCash<AccountViewModel>.GetViewModel(currentUser.Id, ACC_ADDRESSES);
            List<AccountAddress> limitedAccAddresses = accountAddresses;
            LimitAddressesByCountry(ref limitedAccAddresses, accountViewModelCash);
            LimitAddressesByRegion(ref limitedAccAddresses, accountViewModelCash);
            LimitAddressesByCity(ref limitedAccAddresses, accountViewModelCash);
            LimitAddressesByStreet(ref limitedAccAddresses, accountViewModelCash);
            LimitAddressesByHouse(ref limitedAccAddresses, accountViewModelCash);
            LimitAddressesByType(ref limitedAccAddresses, accountViewModelCash);
            LimitListByPageNumber(currentUser.Id, ACC_ADDRESSES, ref limitedAccAddresses);
            return limitedAccAddresses;
        }

        /// <summary>
        /// Ограничение списка адресов клиента по стране
        /// </summary>
        /// <param name="accountAddresses"></param>
        /// <param name="accountViewModel"></param>
        private void LimitAddressesByCountry(ref List<AccountAddress> accountAddresses, AccountViewModel accountViewModel)
        {
            string searchAddressCountry = accountViewModel.SearchAddressCountryCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchAddressCountry))
                accountAddresses = accountAddresses.Where(addr => addr.Country.ToLower().Contains(searchAddressCountry)).ToList();
        }

        /// <summary>
        /// Ограничение списка адресов клиента по региону
        /// </summary>
        /// <param name="accountAddresses"></param>
        /// <param name="accountViewModel"></param>
        private void LimitAddressesByRegion(ref List<AccountAddress> accountAddresses, AccountViewModel accountViewModel)
        {
            string searchAddressRegion = accountViewModel.SearchAddressRegionCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchAddressRegion))
                accountAddresses = accountAddresses.Where(addr => !string.IsNullOrEmpty(addr.Region) && addr.Region.ToLower().Contains(searchAddressRegion)).ToList();
        }

        /// <summary>
        /// Ограничение списка адресов клиента по городу
        /// </summary>
        /// <param name="accountAddresses"></param>
        /// <param name="accountViewModel"></param>
        private void LimitAddressesByCity(ref List<AccountAddress> accountAddresses, AccountViewModel accountViewModel)
        {
            string searchAddressCity = accountViewModel.SearchAddressCityCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchAddressCity))
                accountAddresses = accountAddresses.Where(addr => !string.IsNullOrEmpty(addr.City) && addr.City.ToLower().Contains(searchAddressCity)).ToList();
        }

        /// <summary>
        /// Ограничение списка адресов клиента по улице
        /// </summary>
        /// <param name="accountAddresses"></param>
        /// <param name="accountViewModel"></param>
        private void LimitAddressesByStreet(ref List<AccountAddress> accountAddresses, AccountViewModel accountViewModel)
        {
            string searchAddressStreet = accountViewModel.SearchAddressStreetCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchAddressStreet))
                accountAddresses = accountAddresses.Where(addr => !string.IsNullOrEmpty(addr.Street) && addr.Street.ToLower().Contains(searchAddressStreet)).ToList();
        }

        /// <summary>
        /// Ограничение списка адресов клиента по дому
        /// </summary>
        /// <param name="accountAddresses"></param>
        /// <param name="accountViewModel"></param>
        private void LimitAddressesByHouse(ref List<AccountAddress> accountAddresses, AccountViewModel accountViewModel)
        {
            string searchAddressHouse = accountViewModel.SearchAddressHouseCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchAddressHouse))
                accountAddresses = accountAddresses.Where(addr => !string.IsNullOrEmpty(addr.House) && addr.House.ToLower().Contains(searchAddressHouse)).ToList();
        }

        /// <summary>
        /// Ограничение списка адресов клиента по типу
        /// </summary>
        /// <param name="accountAddresses"></param>
        /// <param name="accountViewModel"></param>
        private void LimitAddressesByType(ref List<AccountAddress> accountAddresses, AccountViewModel accountViewModel)
        {
            string searchAddressType = accountViewModel.SearchAddressTypeCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchAddressType) && Enum.TryParse(typeof(AddressType), searchAddressType, out object addressType))
                accountAddresses = accountAddresses.Where(addr => addr.AddressType == (AddressType)addressType).ToList();
        }

        #endregion

        #region Attaching Invoices

        /// <summary>
        /// Добавляет банковские реквизиты к клиенту
        /// </summary>
        /// <param name="accountViewModel"></param>
        public void AttachInvoices(AccountViewModel accountViewModel)
        {
            accountViewModel.AccountInvoices = accountViewModel.GetInvoices(context)
                .TransformToViewModels<AccountInvoice, AccountInvoiceViewModel, AccountInvoiceTransformer>(
                    transformer: new AccountInvoiceTransformer(context, resManager),
                    limitingFunc: GetLimitedAccInvoicesList);
        }

        private List<AccountInvoice> GetLimitedAccInvoicesList(List<AccountInvoice> accountInvoices)
        {
            AccountViewModel accountViewModelCash = ModelCash<AccountViewModel>.GetViewModel(currentUser.Id, ACC_INVOICES);
            List<AccountInvoice> limitedAccInvoices = accountInvoices;
            LimitInvoicesByBankName(ref limitedAccInvoices, accountViewModelCash);
            LimitInvoicesByCity(ref limitedAccInvoices, accountViewModelCash);
            LimitInvoicesByCheckingAccount(ref limitedAccInvoices, accountViewModelCash);
            LimitInvoicesByCorrespondentAccount(ref limitedAccInvoices, accountViewModelCash);
            LimitInvoicesByBIC(ref limitedAccInvoices, accountViewModelCash);
            LimitInvoicesBySWIFT(ref limitedAccInvoices, accountViewModelCash);
            LimitListByPageNumber(currentUser.Id, ACC_INVOICES, ref limitedAccInvoices);
            return limitedAccInvoices;
        }

        /// <summary>
        /// Ограничение списка банковских реквизитов по названию банка
        /// </summary>
        /// <param name="accountInvoicesToLimit"></param>
        /// <param name="accountViewModel"></param>
        private void LimitInvoicesByBankName(ref List<AccountInvoice> accountInvoicesToLimit, AccountViewModel accountViewModel)
        {
            string searchInvoiceBankName = accountViewModel.SearchInvoiceBankNameCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchInvoiceBankName))
                accountInvoicesToLimit = accountInvoicesToLimit.Where(acc => acc.BankName.ToLower().Contains(searchInvoiceBankName)).ToList();
        }

        /// <summary>
        /// Ограничение списка банковских реквизитов по городу банка
        /// </summary>
        /// <param name="accountInvoicesToLimit"></param>
        /// <param name="accountViewModel"></param>
        private void LimitInvoicesByCity(ref List<AccountInvoice> accountInvoicesToLimit, AccountViewModel accountViewModel)
        {
            string searchInvoiceCity = accountViewModel.SearchInvoiceCityCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchInvoiceCity))
                accountInvoicesToLimit = accountInvoicesToLimit.Where(acc => acc.BankName.ToLower().Contains(searchInvoiceCity)).ToList();
        }

        /// <summary>
        /// Ограничение списка банковских реквизитов по расчетному счету
        /// </summary>
        /// <param name="accountInvoicesToLimit"></param>
        /// <param name="accountViewModel"></param>
        private void LimitInvoicesByCheckingAccount(ref List<AccountInvoice> accountInvoicesToLimit, AccountViewModel accountViewModel)
        {
            string searchInvoiceCheckingAccount= accountViewModel.SearchInvoiceCheckingAccountCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchInvoiceCheckingAccount))
                accountInvoicesToLimit = accountInvoicesToLimit.Where(acc => acc.BankName.ToLower().Contains(searchInvoiceCheckingAccount)).ToList();
        }

        /// <summary>
        /// Ограничение списка банковских реквизитов по корреспонденскому счету
        /// </summary>
        /// <param name="accountInvoicesToLimit"></param>
        /// <param name="accountViewModel"></param>
        private void LimitInvoicesByCorrespondentAccount(ref List<AccountInvoice> accountInvoicesToLimit, AccountViewModel accountViewModel)
        {
            string searchInvoiceCorrespondentAccount= accountViewModel.SearchInvoiceCorrespondentAccountCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchInvoiceCorrespondentAccount))
                accountInvoicesToLimit = accountInvoicesToLimit.Where(acc => acc.BankName.ToLower().Contains(searchInvoiceCorrespondentAccount)).ToList();
        }

        /// <summary>
        /// Ограничение списка банковских реквизитов по БИКу
        /// </summary>
        /// <param name="accountInvoicesToLimit"></param>
        /// <param name="accountViewModel"></param>
        private void LimitInvoicesByBIC(ref List<AccountInvoice> accountInvoicesToLimit, AccountViewModel accountViewModel)
        {
            string searchInvoiceBIC = accountViewModel.SearchInvoiceBICCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchInvoiceBIC))
                accountInvoicesToLimit = accountInvoicesToLimit.Where(acc => acc.BankName.ToLower().Contains(searchInvoiceBIC)).ToList();
        }

        /// <summary>
        /// Ограничение списка банковских реквизитов по коду SWIFT
        /// </summary>
        /// <param name="accountInvoicesToLimit"></param>
        /// <param name="accountViewModel"></param>
        private void LimitInvoicesBySWIFT(ref List<AccountInvoice> accountInvoicesToLimit, AccountViewModel accountViewModel)
        {
            string searchInvoiceSWIFT = accountViewModel.SearchInvoiceSWIFTCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchInvoiceSWIFT))
                accountInvoicesToLimit = accountInvoicesToLimit.Where(acc => acc.BankName.ToLower().Contains(searchInvoiceSWIFT)).ToList();
        }

        #endregion

        #region Attaching Quotes

        /// <summary>
        /// Добавляет сделки к клиенту
        /// </summary>
        /// <param name="accountViewModel"></param>
        public void AttachQuotes(AccountViewModel accountViewModel)
        {

        }

        private List<AccountQuote> GetLimitedAccQuotesList(List<AccountQuote> accountQuotes)
        {
            AccountViewModel accountViewModelCash = ModelCash<AccountViewModel>.GetViewModel(currentUser.Id, ACC_QUOTES);
            List<AccountQuote> limitedAccQuotes = accountQuotes;
            LimitListByPageNumber(currentUser.Id, ACC_QUOTES, ref limitedAccQuotes);
            return limitedAccQuotes;
        }

        #endregion

        #region Attaching Managers

        /// <summary>
        /// Добавляет менеджеров к клиенту
        /// </summary>
        /// <param name="accountViewModel"></param>
        public void AttachManagers(AccountViewModel accountViewModel)
        {
            accountViewModel.AccountManagers = accountViewModel.GetManagers(context).GetViewModelsFromData
                <AccountManager, AccountManagerViewModel, AccountManagerTransformer>(new AccountManagerTransformer(context, resManager));
        }

        #endregion

        #region Other
        /// <summary>
        /// Метод пытается изменить основной контакт на клиенте
        /// </summary>
        /// <param name="accountViewModel"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public bool TryChangePrimaryContact(AccountViewModel accountViewModel, Account account, ModelStateDictionary modelState)
        {
            Dictionary<string, string> errors = validator.ChangePrimaryContactCheck(accountViewModel, account);
            if (errors.Count > 0)
            {
                errors.Keys.ToList().ForEach(error => modelState.AddModelError(error, errors[error]));
                return false;
            }

            // В зависимости от типа клиента
            switch (account.AccountType)
            {
                // Для физических лиц основной контакт является обязательным
                case AccountType.Individual:
                    Guid primaryContactId = Guid.Parse(accountViewModel.PrimaryContactId);
                    AccountContact primaryContact = context.AccountContacts.FirstOrDefault(i => i.Id == primaryContactId);
                    account.PrimaryContactId = primaryContactId;
                    account.Name = primaryContact.GetFullName();
                    break;

                // Для остальных типов клиентов основной контакт может отсутствовать
                case AccountType.IndividualEntrepreneur:
                    account.PrimaryContactId = string.IsNullOrEmpty(accountViewModel.PrimaryContactId) ? Guid.Empty : Guid.Parse(accountViewModel.PrimaryContactId);
                    break;
            }

            context.Accounts.Update(account);
            context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Метод пытается изменить юридический адрес у клиента
        /// </summary>
        /// <param name="addressViewModel"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public bool TryChangeLegalAddress(AccountAddressViewModel addressViewModel, out Dictionary<string, string> errors)
        {
            errors = new Dictionary<string, string>();
            if (!TryGetItemById(addressViewModel.AccountId, out Account account))
            {
                if (!errors.ContainsKey("RecordNotFound"))
                    errors.Add("RecordNotFound", resManager.GetString("RecordNotFound"));
                return false;
            }

            errors = validator.ChangeLegalAddressCheck(addressViewModel, account);
            if (errors.Any()) return false;

            AccountAddress oldLegalAddress = account.GetAddresses(context).FirstOrDefault(addr => addr.AddressType == AddressType.Legal);
            oldLegalAddress.AddressType = (AddressType)Enum.Parse(typeof(AddressType), addressViewModel.CurrentAddressNewType);
            AccountAddress newLegalAddress = context.AccountAddresses.FirstOrDefault(i => i.Id == Guid.Parse(addressViewModel.NewLegalAddressId));
            newLegalAddress.AddressType = AddressType.Legal;
            context.AccountAddresses.UpdateRange(oldLegalAddress, newLegalAddress);
            context.SaveChanges();
            return true;
        }

        public bool TryChangeSite(string accountId, out Dictionary<string, string> errors, string newSite = null)
        {
            errors = new Dictionary<string, string>();
            if (!TryGetItemById(accountId, out Account account))
                errors.Add("AccountNotFound", resManager.GetString("AccountNotFound"));
            if (context.AccountManagers.FirstOrDefault(i => i.Id == account.PrimaryManagerId) == null)
                errors.Add("AccountWithoutKMIsReadonly", resManager.GetString("AccountWithoutKMIsReadonly"));
            else
            {
                try
                {
                    account.Site = newSite;
                    context.Accounts.Update(account);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    errors.Add(resManager.GetString("UnhandledException"), ex.Message);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Метод пытается добавить в команду по клиенту нового менеджера
        /// </summary>
        /// <param name="accountViewModel"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public bool TryAddAccountManager(AccountViewModel accountViewModel, out Dictionary<string, string> errors)
        {
            errors = new Dictionary<string, string>();
            if (!TryGetItemById(accountViewModel.Id, out Account account))
                errors.Add("AccountNotFound", resManager.GetString("AccountNotFound"));
            if (!new EmployeeRepository(context, resManager).TryGetItemById(accountViewModel.NewPrimaryManagerId, out Employee employee))
                errors.Add("EmployeeNotExists", resManager.GetString("EmployeeNotExists"));
            else
            {
                try
                {
                    AccountManager accountManager = new AccountManager()
                    {
                        Id = Guid.NewGuid(),
                        Account = account,
                        AccountId = account.Id,
                        ManagerId = employee.Id
                    };
                    account.PrimaryManagerId = accountManager.Id;
                    account.AccountStatus = AccountStatus.Active;
                    context.Entry(account).State = EntityState.Modified;

                    // Если произошло так, что у клиента был в списке этот менеджер, но не был основным, то его не надо добавлять
                    if (!account.AccountManagers.Select(i => i.ManagerId).Contains(accountManager.ManagerId))
                    {
                        account.AccountManagers.Add(accountManager);
                        context.Entry(accountManager).State = EntityState.Added;
                    }
                    context.Accounts.Update(account);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    errors.Add(resManager.GetString("UnhandledException"), ex.Message);
                }
            }
            return false;
        }

        /// <summary>
        /// Метод проверяет клиентов, у которых поданный сотрудник является основным клиентским менеджером
        /// Если в команде найденного клиента присутствуют другие менеджеры, метод делает основным случайного
        /// Иначе блокирует клиента
        /// </summary>
        /// <param name="employee"></param>
        public void CheckAccountsForLock(Employee employee)
        {
            // Список всех основных клиентских менеджеров, где сотрудником является поданный "employee"
            Func<AccountManager, bool> predicate = accNam => accNam.ManagerId == employee.Id && accNam.Account.PrimaryManagerId == accNam.Id;
            List<AccountManager> accountManagers = context.AccountManagers.Include(acc => acc.Account).Where(predicate).ToList();
            accountManagers.Select(acc => acc.Account).ToList().ForEach(account =>
            {
                List<AccountManager> allAccManagers = context.AccountManagers.Where(acc => acc.AccountId == account.Id).ToList();
                if (allAccManagers.Count <= 1)
                {
                    account.AccountStatus = AccountStatus.Lock;
                    account.PrimaryManagerId = Guid.Empty;
                }
                else account.PrimaryManagerId = allAccManagers.FirstOrDefault(i => i.ManagerId != employee.Id).Id;
                context.Accounts.Update(account);
            });
        }
        #endregion
    }
}
