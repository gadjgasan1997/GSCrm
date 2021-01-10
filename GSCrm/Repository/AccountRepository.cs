using GSCrm.Mapping;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Models.ViewTypes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using GSCrm.Data;
using GSCrm.Transactions;
using static GSCrm.CommonConsts;
using static GSCrm.RegexConsts;
using static GSCrm.Utils.CollectionsUtils;
using GSCrm.Validators;
using Newtonsoft.Json.Linq;
using GSCrm.Utils;
using GSCrm.Models.Enums;

namespace GSCrm.Repository
{
    public class AccountRepository : BaseRepository<Account, AccountViewModel>
    {
        #region Declarations
        /// <summary>
        /// Все типы представлений, связанные с клиентами
        /// </summary>
        public static AccountViewType[] AccAllViewTypes => new AccountViewType[] {
            AccountViewType.ACC_CONTACTS, AccountViewType.ACC_ADDRESSES, AccountViewType.ACC_INVOICES,
            AccountViewType.ACC_QUOTES, AccountViewType.ACC_DOCS, AccountViewType.ACC_MANAGERS };
        // Длины имени, фамилия и отчества
        private const int FIRST_NAME_MIN_LENGTH = 2;
        private const int FIRST_NAME_MAX_LENGTH = 300;
        private const int LAST_NAME_MIN_LENGTH = 2;
        private const int LAST_NAME_MAX_LENGTH = 300;
        private const int MIDDLE_NAME_MAX_LENGTH = 300;
        // Возможная длина ИНН
        private const int INN_TEN_LENGTH = 10;
        private const int INN_TWELVE_LENGTH = 12;
        // Множители цифр для ИНН
        private static readonly int[] INN_10_NUM_FACTORS = { 2, 4, 10, 3, 5, 9, 4, 6, 8 };
        private static readonly int[] INN_12_NUM_FACTORS_FIRST_CHECK = { 7, 2, 4, 10, 3, 5, 9, 4, 6, 8 };
        private static readonly int[] INN_12_NUM_FACTORS_LAST_CHECK = { 3, 7, 2, 4, 10, 3, 5, 9, 4, 6, 8 };
        // Делитель для ИНН
        private const int INN_DIVIDER = 11;
        // Возможная длина КПП
        private const int KPP_LENGTH = 9;
        // Возможная длина ОКПО
        private const int OKPO_LENGTH = 8;
        // Множители цифр для ОКПО
        private static readonly int[] OKPO_NUM_FACTORS_FIRST_CHECK = { 1, 2, 3, 4, 5, 6, 7 };
        private static readonly int[] OKPO_NUM_FACTORS_LAST_CHECK = { 3, 4, 5, 6, 7, 8, 9 };
        // Делитель для ОКПО
        private const int OKPO_DIVIDER = 11;
        // Возможная длина ОГРН
        private const int OGRN_LENGTH = 13;
        // Делитель для ОГРН
        private const int OGRN_DIVIDER = 11;
        #endregion

        #region Constructs
        public AccountRepository(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(serviceProvider, context)
        { }
        #endregion

        #region Override Methods
        public override bool HasPermissionsForSeeItem(Account account)
        {
            if (account == null) return false;
            Func<UserOrganization, bool> predicate = userOrg => userOrg.UserId == currentUser.Id && userOrg.Accepted;
            List<UserOrganization> userOrganizations = context.UserOrganizations.AsNoTracking().Where(predicate).ToList();
            return userOrganizations.Select(userOrg => userOrg.OrganizationId).ToList().Contains(account.OrganizationId);
        }

        /// <summary>
        /// Возвращает true всегда, так как на текущий момент неизвестно, под какой организацией создается клиент
        /// </summary>
        /// <param name="accountViewModel"></param>
        /// <returns></returns>
        protected override bool RespsIsCorrectOnCreate(AccountViewModel accountViewModel) => true;

        protected override bool TryCreatePrepare(AccountViewModel accountViewModel)
        {
            // Проверка, что у пользователя id организации, под которой создается клиент не пустой 
            Guid organizationId = accountViewModel.OrganizationId != Guid.Empty ? accountViewModel.OrganizationId : currentUser.PrimaryOrganizationId;
            if (organizationId == Guid.Empty)
                errors.Add("YotNeedCreateOrgToAddAccounts", resManager.GetString("YotNeedCreateOrgToAddAccounts"));
            else
            {
                // Установка организации, под которой создается клиент и ее Id
                Organization currentOrganization = context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == organizationId);
                transaction.AddParameter("OrganizationId", organizationId);
                transaction.AddParameter("CurrentOrganization", currentOrganization);

                // Проверка полномочий
                if (!new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("AccCreate", transaction))
                    AddHasNoPermissionsError(OperationType.Create);

                // Остальные проверки
                else if (TryCheckAccountType(accountViewModel))
                {
                    List<Action> commonHandlers = new List<Action>()
                    {
                        () => CheckCountry(accountViewModel),
                        () => CheckPrimaryManager(accountViewModel)
                    };

                    switch ((AccountType)transaction.GetParameterValue("AccountType"))
                    {
                        case AccountType.Individual:
                            InvokeIntermittinActions(errors, new List<Action>()
                            {
                                () => {
                                    new PersonValidator(resManager).CheckPersonName(
                                        accountViewModel.FirstName,
                                        accountViewModel.LastName,
                                        accountViewModel.MiddleName,
                                        ref errors);
                                },
                                () => CheckINNOnCreate(accountViewModel)
                            }.Concat(commonHandlers));
                            break;

                        case AccountType.IndividualEntrepreneur:
                            InvokeIntermittinActions(errors, new List<Action>()
                            {
                                () => CheckIENameOnCreate(accountViewModel),
                                () => CheckINNOnCreate(accountViewModel)
                            }.Concat(commonHandlers));
                            break;

                        case AccountType.LegalEntity:
                            InvokeIntermittinActions(errors, new List<Action>()
                            {
                                () => CheckLENameOnCreate(accountViewModel),
                                () => CheckINNOnCreate(accountViewModel),
                                () => CheckKPPOnCreate(accountViewModel),
                                () => CheckOKPOOnCreate(accountViewModel),
                                () => CheckOGRNOnCreate(accountViewModel)
                            }.Concat(commonHandlers));
                            break;
                    }
                }
            }
            
            return !errors.Any();
        }

        protected override bool RespsIsCorrectOnUpdate(AccountViewModel accountViewModel)
            => CheckPermissionForAccountGroup("AccUpdate", transaction);

        protected override bool TryUpdatePrepare(AccountViewModel accountViewModel)
        {
            if (TryCheckAccountType(accountViewModel))
            {
                Account account = context.Accounts.AsNoTracking().FirstOrDefault(i => i.Id == accountViewModel.Id);
                AccountType accountType = (AccountType)transaction.GetParameterValue("AccountType");
                InvokeIntermittinActions(errors, new List<Action>()
                {
                    () => {
                        if (accountType != account.AccountType)
                            errors.Add("AccountTypeIsReadonly", resManager.GetString("AccountTypeIsReadonly"));
                    },
                    () => {
                        if (context.AccountManagers.AsNoTracking().FirstOrDefault(i => i.Id == account.PrimaryManagerId) == null)
                            errors.Add("AccountWithoutKMIsReadonly", resManager.GetString("AccountWithoutKMIsReadonly"));
                    }
                });

                switch (accountType)
                {
                    case AccountType.Individual:
                        InvokeIntermittinActions(errors, new List<Action>()
                        {
                            () => {
                                string fullName = accountViewModel.Name.TrimStartAndEnd();
                                if (account.Name != fullName)
                                    errors.Add("IndividualNameIsReadonly", resManager.GetString("IndividualNameIsReadonly"));
                            },
                            () => CheckINNOnUpdate(accountViewModel)
                        });
                        break;

                    case AccountType.IndividualEntrepreneur:
                        InvokeIntermittinActions(errors, new List<Action>()
                        {
                            () => CheckIENameOnUpdate(accountViewModel),
                            () => CheckINNOnUpdate(accountViewModel)
                        });
                        break;

                    case AccountType.LegalEntity:
                        InvokeAllChecks(new List<Action>()
                        {
                            () => CheckLENameOnUpdate(accountViewModel),
                            () => CheckINNOnUpdate(accountViewModel),
                            () => CheckKPPOnUpdate(accountViewModel),
                            () => CheckOKPOOnUpdate(accountViewModel),
                            () => CheckOGRNOnUpdate(accountViewModel)
                        });
                        break;
                }
            }
            return !errors.Any();
        }

        protected override bool RespsIsCorrectOnDelete(Account account)
            => CheckPermissionForAccountGroup("AccDelete", transaction);

        protected override void FailureUpdateHandler(AccountViewModel accountViewModel)
        {
            if (TryGetItemById(accountViewModel.Id, out Account account))
            {
                accountViewModel = map.DataToViewModel(account);
                accountViewModel = new AccountMap(serviceProvider, context).Refresh(accountViewModel, currentUser, AccAllViewTypes);
                AttachContacts(accountViewModel);
                AttachAddresses(accountViewModel);
                AttachInvoices(accountViewModel);
                AttachQuotes(accountViewModel);
                AttachManagers(accountViewModel);
            }
        }
        #endregion

        #region Searching
        /// <summary>
        /// Метод очищает поиск по всем клиентам
        /// </summary>
        public void ClearAllAccountsSearch()
        {
            AccountsViewModel accountsViewModelCash = cachService.GetCachedItem<AccountsViewModel>(currentUser.Id, ALL_ACCS);
            accountsViewModelCash.AllAccountsSearchName = default;
            accountsViewModelCash.AllAccountsSearchType = default;
            cachService.CacheItem(currentUser.Id, ALL_ACCS, accountsViewModelCash);
        }

        /// <summary>
        /// Метод очищает поиск по клиентам основной организации пользователя
        /// </summary>
        public void ClearCurrentAccountsSearch()
        {
            AccountsViewModel accountsViewModelCash = cachService.GetCachedItem<AccountsViewModel>(currentUser.Id, CURRENT_ACCS);
            accountsViewModelCash.CurrentAccountsSearchName = default;
            accountsViewModelCash.CurrentAccountsSearchType = default;
            cachService.CacheItem(currentUser.Id, CURRENT_ACCS, accountsViewModelCash);
        }

        /// <summary>
        /// Очистка поиска по контактам клиента
        /// </summary>
        public void ClearContactSearch()
        {
            AccountViewModel accountViewModelCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_CONTACTS);
            accountViewModelCash.SearchContactFullName = default;
            accountViewModelCash.SearchContactType = default;
            accountViewModelCash.SearchContactEmail = default;
            accountViewModelCash.SearchContactPhoneNumber = default;
            accountViewModelCash.SearchContactPrimary = default;
            cachService.CacheItem(currentUser.Id, ACC_CONTACTS, accountViewModelCash);
        }

        /// <summary>
        /// Очистка поиска по адресам клиента
        /// </summary>
        public void ClearAddressSearch()
        {
            AccountViewModel accountViewModelCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_ADDRESSES);
            accountViewModelCash.SearchAddressType = default;
            accountViewModelCash.SearchAddressCountry = default;
            accountViewModelCash.SearchAddressRegion = default;
            accountViewModelCash.SearchAddressCity = default;
            accountViewModelCash.SearchAddressStreet = default;
            accountViewModelCash.SearchAddressHouse = default;
            cachService.CacheItem(currentUser.Id, ACC_ADDRESSES, accountViewModelCash);
        }

        /// <summary>
        /// Очистка поиска по банкосвким реквизитам клиента
        /// </summary>
        public void ClearInvoiceSearch()
        {
            AccountViewModel accountViewModelCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_INVOICES);
            accountViewModelCash.SearchInvoiceBankName = default;
            accountViewModelCash.SearchInvoiceCity = default;
            accountViewModelCash.SearchInvoiceCheckingAccount = default;
            accountViewModelCash.SearchInvoiceCorrespondentAccount = default;
            accountViewModelCash.SearchInvoiceBIC = default;
            accountViewModelCash.SearchInvoiceSWIFT = default;
            cachService.CacheItem(currentUser.Id, ACC_INVOICES, accountViewModelCash);
        }

        /// <summary>
        /// Очистка поиска по сделкам клиента
        /// </summary>
        public void ClearQuoteSearch()
        {
            AccountViewModel accountViewModelCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_QUOTES);
            cachService.CacheItem(currentUser.Id, ACC_QUOTES, accountViewModelCash);
        }

        #endregion

        #region Attaching Accounts

        /// <summary>
        /// Метод добавляет список моделей отображения клиента к модели "AccountsViewModel"
        /// </summary>
        /// <param name="accountsViewModel"></param>
        public void AttachAccounts(ref AccountsViewModel accountsViewModel)
        {
            List<Account> allAccounts = context.GetAllAccounts(currentUser);
            List<Account> currentAccounts = new List<Account>(allAccounts);
            accountsViewModel.AllAccounts = allAccounts.MapToViewModels(map, GetLimitedAllAccountsList);
            accountsViewModel.CurrentAccounts = currentAccounts.MapToViewModels(map, GetLimitedCurrentAccountsList);
        }

        /// <summary>
        /// Метод ограничивает список клиентов всех организаций
        /// </summary>
        /// <param name="accounts"></param>
        /// <returns></returns>
        private List<Account> GetLimitedAllAccountsList(List<Account> accounts)
        {
            List<Account> limitedAccounts = accounts;
            AccountsViewModel accountsViewModelCash = cachService.GetCachedItem<AccountsViewModel>(currentUser.Id, ALL_ACCS);
            LimitAllAccsBySearchName(ref limitedAccounts, accountsViewModelCash);
            LimitAllAccsBySearchType(ref limitedAccounts, accountsViewModelCash);
            LimitListByPageNumber(ALL_ACCS, ref limitedAccounts);
            return limitedAccounts;
        }

        /// <summary>
        /// Ограничение списка всех клиентов по названию
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="accountsToLimit"></param>
        private void LimitAllAccsBySearchName(ref List<Account> accountsToLimit, AccountsViewModel accountsViewModel)
        {
            if (!string.IsNullOrEmpty(accountsViewModel.AllAccountsSearchName))
                accountsToLimit = accountsToLimit.Where(n => n.Name.ToLower().Contains(accountsViewModel.AllAccountsSearchName)).ToList();
        }

        /// <summary>
        /// Ограничение списка всех клиентов по типу
        /// </summary>
        /// <param name="accountsToLimit"></param>
        private void LimitAllAccsBySearchType(ref List<Account> accountsToLimit, AccountsViewModel accountsViewModel)
        {
            if (!string.IsNullOrEmpty(accountsViewModel.AllAccountsSearchType))
                accountsToLimit = accountsToLimit.Where(n => n.AccountType.ToString() == accountsViewModel.AllAccountsSearchType).ToList();
        }

        /// <summary>
        /// Метод ограничивает список клиентов основной организации пользователя
        /// </summary>
        /// <param name="accounts"></param>
        /// <returns></returns>
        private List<Account> GetLimitedCurrentAccountsList(List<Account> accounts)
        {
            List<Account> limitedAccounts = accounts.Where(orgId => orgId.OrganizationId == currentUser.PrimaryOrganizationId).ToList();
            AccountsViewModel accountsViewModelCash = cachService.GetCachedItem<AccountsViewModel>(currentUser.Id, CURRENT_ACCS);
            LimitCurrentAccsBySearchName(ref limitedAccounts, accountsViewModelCash);
            LimitCurrentAccsBySearchType(ref limitedAccounts, accountsViewModelCash);
            LimitListByPageNumber(CURRENT_ACCS, ref limitedAccounts);
            return limitedAccounts;
        }

        /// <summary>
        /// Ограничение списка клиентов основной организации текущего пользователя по названию
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="accountsToLimit"></param>
        private void LimitCurrentAccsBySearchName(ref List<Account> accountsToLimit, AccountsViewModel accountsViewModel)
        {
            if (!string.IsNullOrEmpty(accountsViewModel.CurrentAccountsSearchName))
                accountsToLimit = accountsToLimit.Where(n => n.Name.ToLower().Contains(accountsViewModel.CurrentAccountsSearchName)).ToList();
        }

        /// <summary>
        /// Ограничение списка клиентов основной орагнизации пользователя по типу
        /// </summary>
        /// <param name="accountsToLimit"></param>
        private void LimitCurrentAccsBySearchType(ref List<Account> accountsToLimit, AccountsViewModel accountsViewModel)
        {
            if (!string.IsNullOrEmpty(accountsViewModel.CurrentAccountsSearchType))
                accountsToLimit = accountsToLimit.Where(n => n.AccountType.ToString() == accountsViewModel.CurrentAccountsSearchType).ToList();
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
                .MapToViewModels(new AccountContactMap(serviceProvider, context), GetLimitedAccContactsList);
        }

        private List<AccountContact> GetLimitedAccContactsList(List<AccountContact> accountContacts)
        {
            AccountViewModel accountViewModelCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_CONTACTS);
            List<AccountContact> limitedAccContacts = accountContacts;
            LimitContactsByFullName(ref limitedAccContacts, accountViewModelCash);
            LimitContactsByType(ref limitedAccContacts, accountViewModelCash);
            LimitContactsByPhoneNumber(ref limitedAccContacts, accountViewModelCash);
            LimitContactsByEmail(ref limitedAccContacts, accountViewModelCash);
            LimitContactsByPrimarySign(ref limitedAccContacts, accountViewModelCash);
            LimitListByPageNumber(ACC_CONTACTS, ref limitedAccContacts);
            return limitedAccContacts;
        }

        /// <summary>
        /// Мето ограничивает список контактов клиента по полному имеени
        /// </summary>
        /// <param name="accountContactsToLimit"></param>
        private void LimitContactsByFullName(ref List<AccountContact> accountContactsToLimit, AccountViewModel accountViewModel)
        {
            if (!string.IsNullOrEmpty(accountViewModel.SearchContactFullName))
                accountContactsToLimit = accountContactsToLimit.Where(accCon => accCon.GetFullName().ToLower().Contains(accountViewModel.SearchContactFullName)).ToList();
        }

        /// <summary>
        /// Мето ограничивает список контактов клиента по типу контакта
        /// </summary>
        /// <param name="accountContactsToLimit"></param>
        private void LimitContactsByType(ref List<AccountContact> accountContactsToLimit, AccountViewModel accountViewModel)
        {
            if (!string.IsNullOrEmpty(accountViewModel.SearchContactType))
            {
                Func<AccountContact, bool> predicate = accCon => accCon.ContactType == (ContactType)Enum.Parse(typeof(ContactType), accountViewModel.SearchContactType);
                accountContactsToLimit = accountContactsToLimit.Where(predicate).ToList();
            }
        }

        /// <summary>
        /// Мето ограничивает список контактов клиента по телефону
        /// </summary>
        /// <param name="accountContactsToLimit"></param>
        private void LimitContactsByPhoneNumber(ref List<AccountContact> accountContactsToLimit, AccountViewModel accountViewModel)
        {
            if (!string.IsNullOrEmpty(accountViewModel.SearchContactPhoneNumber))
                accountContactsToLimit = accountContactsToLimit.Where(accCon => accCon.PhoneNumber.ToLower().Contains(accountViewModel.SearchContactPhoneNumber)).ToList();
        }

        /// <summary>
        /// Мето ограничивает список контактов клиента по почту
        /// </summary>
        /// <param name="accountContactsToLimit"></param>
        private void LimitContactsByEmail(ref List<AccountContact> accountContactsToLimit, AccountViewModel accountViewModel)
        {
            if (!string.IsNullOrEmpty(accountViewModel.SearchContactEmail))
                accountContactsToLimit = accountContactsToLimit.Where(accCon => accCon.PhoneNumber.ToLower().Contains(accountViewModel.SearchContactEmail)).ToList();
        }

        /// <summary>
        /// Мето ограничивает список контактов клиента по признаку основного
        /// </summary>
        /// <param name="accountContactsToLimit"></param>
        private void LimitContactsByPrimarySign(ref List<AccountContact> accountContactsToLimit, AccountViewModel accountViewModel)
        {
            Account account = context.Accounts.FirstOrDefault(i => i.Id == accountViewModel.Id);
            if (accountViewModel.SearchContactPrimary)
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
            accountViewModel.AccountAddresses = accountAddresses.MapToViewModels(
                    map: new AccountAddressMap(serviceProvider, context),
                    limitingFunc: GetLimitedAccAddressesList);
            accountViewModel.AllAccountAddresses = accountAddresses.GetViewModelsFromData(new AccountAddressMap(serviceProvider, context));
        }

        private List<AccountAddress> GetLimitedAccAddressesList(List<AccountAddress> accountAddresses)
        {
            AccountViewModel accountViewModelCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_ADDRESSES);
            List<AccountAddress> limitedAccAddresses = accountAddresses;
            LimitAddressesByCountry(ref limitedAccAddresses, accountViewModelCash);
            LimitAddressesByRegion(ref limitedAccAddresses, accountViewModelCash);
            LimitAddressesByCity(ref limitedAccAddresses, accountViewModelCash);
            LimitAddressesByStreet(ref limitedAccAddresses, accountViewModelCash);
            LimitAddressesByHouse(ref limitedAccAddresses, accountViewModelCash);
            LimitAddressesByType(ref limitedAccAddresses, accountViewModelCash);
            LimitListByPageNumber(ACC_ADDRESSES, ref limitedAccAddresses);
            return limitedAccAddresses;
        }

        /// <summary>
        /// Ограничение списка адресов клиента по стране
        /// </summary>
        /// <param name="accountAddresses"></param>
        /// <param name="accountViewModel"></param>
        private void LimitAddressesByCountry(ref List<AccountAddress> accountAddresses, AccountViewModel accountViewModel)
        {
            if (!string.IsNullOrEmpty(accountViewModel.SearchAddressCountry))
                accountAddresses = accountAddresses.Where(addr => addr.Country.ToLower().Contains(accountViewModel.SearchAddressCountry)).ToList();
        }

        /// <summary>
        /// Ограничение списка адресов клиента по региону
        /// </summary>
        /// <param name="accountAddresses"></param>
        /// <param name="accountViewModel"></param>
        private void LimitAddressesByRegion(ref List<AccountAddress> accountAddresses, AccountViewModel accountViewModel)
        {
            if (!string.IsNullOrEmpty(accountViewModel.SearchAddressRegion))
                accountAddresses = accountAddresses.Where(addr => !string.IsNullOrEmpty(addr.Region) &&
                    addr.Region.ToLower().Contains(accountViewModel.SearchAddressRegion)).ToList();
        }

        /// <summary>
        /// Ограничение списка адресов клиента по городу
        /// </summary>
        /// <param name="accountAddresses"></param>
        /// <param name="accountViewModel"></param>
        private void LimitAddressesByCity(ref List<AccountAddress> accountAddresses, AccountViewModel accountViewModel)
        {
            if (!string.IsNullOrEmpty(accountViewModel.SearchAddressCity))
                accountAddresses = accountAddresses.Where(addr => !string.IsNullOrEmpty(addr.City) &&
                    addr.City.ToLower().Contains(accountViewModel.SearchAddressCity)).ToList();
        }

        /// <summary>
        /// Ограничение списка адресов клиента по улице
        /// </summary>
        /// <param name="accountAddresses"></param>
        /// <param name="accountViewModel"></param>
        private void LimitAddressesByStreet(ref List<AccountAddress> accountAddresses, AccountViewModel accountViewModel)
        {
            if (!string.IsNullOrEmpty(accountViewModel.SearchAddressStreet))
                accountAddresses = accountAddresses.Where(addr => !string.IsNullOrEmpty(addr.Street) &&
                    addr.Street.ToLower().Contains(accountViewModel.SearchAddressStreet)).ToList();
        }

        /// <summary>
        /// Ограничение списка адресов клиента по дому
        /// </summary>
        /// <param name="accountAddresses"></param>
        /// <param name="accountViewModel"></param>
        private void LimitAddressesByHouse(ref List<AccountAddress> accountAddresses, AccountViewModel accountViewModel)
        {
            if (!string.IsNullOrEmpty(accountViewModel.SearchAddressHouse))
                accountAddresses = accountAddresses.Where(addr => !string.IsNullOrEmpty(addr.House) &&
                    addr.House.ToLower().Contains(accountViewModel.SearchAddressHouse)).ToList();
        }

        /// <summary>
        /// Ограничение списка адресов клиента по типу
        /// </summary>
        /// <param name="accountAddresses"></param>
        /// <param name="accountViewModel"></param>
        private void LimitAddressesByType(ref List<AccountAddress> accountAddresses, AccountViewModel accountViewModel)
        {
            if (!string.IsNullOrEmpty(accountViewModel.SearchAddressType) && Enum.TryParse(typeof(AddressType), accountViewModel.SearchAddressType, out object addressType))
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
                .MapToViewModels(new AccountInvoiceMap(serviceProvider, context), GetLimitedAccInvoicesList);
        }

        private List<AccountInvoice> GetLimitedAccInvoicesList(List<AccountInvoice> accountInvoices)
        {
            AccountViewModel accountViewModelCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_INVOICES);
            List<AccountInvoice> limitedAccInvoices = accountInvoices;
            LimitInvoicesByBankName(ref limitedAccInvoices, accountViewModelCash);
            LimitInvoicesByCity(ref limitedAccInvoices, accountViewModelCash);
            LimitInvoicesByCheckingAccount(ref limitedAccInvoices, accountViewModelCash);
            LimitInvoicesByCorrespondentAccount(ref limitedAccInvoices, accountViewModelCash);
            LimitInvoicesByBIC(ref limitedAccInvoices, accountViewModelCash);
            LimitInvoicesBySWIFT(ref limitedAccInvoices, accountViewModelCash);
            LimitListByPageNumber(ACC_INVOICES, ref limitedAccInvoices);
            return limitedAccInvoices;
        }

        /// <summary>
        /// Ограничение списка банковских реквизитов по названию банка
        /// </summary>
        /// <param name="accountInvoicesToLimit"></param>
        /// <param name="accountViewModel"></param>
        private void LimitInvoicesByBankName(ref List<AccountInvoice> accountInvoicesToLimit, AccountViewModel accountViewModel)
        {
            if (!string.IsNullOrEmpty(accountViewModel.SearchInvoiceBankName))
                accountInvoicesToLimit = accountInvoicesToLimit.Where(acc => acc.BankName.ToLower().Contains(accountViewModel.SearchInvoiceBankName)).ToList();
        }

        /// <summary>
        /// Ограничение списка банковских реквизитов по городу банка
        /// </summary>
        /// <param name="accountInvoicesToLimit"></param>
        /// <param name="accountViewModel"></param>
        private void LimitInvoicesByCity(ref List<AccountInvoice> accountInvoicesToLimit, AccountViewModel accountViewModel)
        {
            if (!string.IsNullOrEmpty(accountViewModel.SearchInvoiceCity))
                accountInvoicesToLimit = accountInvoicesToLimit.Where(acc => acc.BankName.ToLower().Contains(accountViewModel.SearchInvoiceCity)).ToList();
        }

        /// <summary>
        /// Ограничение списка банковских реквизитов по расчетному счету
        /// </summary>
        /// <param name="accountInvoicesToLimit"></param>
        /// <param name="accountViewModel"></param>
        private void LimitInvoicesByCheckingAccount(ref List<AccountInvoice> accountInvoicesToLimit, AccountViewModel accountViewModel)
        {
            if (!string.IsNullOrEmpty(accountViewModel.SearchInvoiceCheckingAccount))
                accountInvoicesToLimit = accountInvoicesToLimit.Where(acc => acc.BankName.ToLower().Contains(accountViewModel.SearchInvoiceCheckingAccount)).ToList();
        }

        /// <summary>
        /// Ограничение списка банковских реквизитов по корреспонденскому счету
        /// </summary>
        /// <param name="accountInvoicesToLimit"></param>
        /// <param name="accountViewModel"></param>
        private void LimitInvoicesByCorrespondentAccount(ref List<AccountInvoice> accountInvoicesToLimit, AccountViewModel accountViewModel)
        {
            if (!string.IsNullOrEmpty(accountViewModel.SearchInvoiceCorrespondentAccount))
                accountInvoicesToLimit = accountInvoicesToLimit.Where(acc => acc.BankName.ToLower().Contains(accountViewModel.SearchInvoiceCorrespondentAccount)).ToList();
        }

        /// <summary>
        /// Ограничение списка банковских реквизитов по БИКу
        /// </summary>
        /// <param name="accountInvoicesToLimit"></param>
        /// <param name="accountViewModel"></param>
        private void LimitInvoicesByBIC(ref List<AccountInvoice> accountInvoicesToLimit, AccountViewModel accountViewModel)
        {
            if (!string.IsNullOrEmpty(accountViewModel.SearchInvoiceBIC))
                accountInvoicesToLimit = accountInvoicesToLimit.Where(acc => acc.BankName.ToLower().Contains(accountViewModel.SearchInvoiceBIC)).ToList();
        }

        /// <summary>
        /// Ограничение списка банковских реквизитов по коду SWIFT
        /// </summary>
        /// <param name="accountInvoicesToLimit"></param>
        /// <param name="accountViewModel"></param>
        private void LimitInvoicesBySWIFT(ref List<AccountInvoice> accountInvoicesToLimit, AccountViewModel accountViewModel)
        {
            if (!string.IsNullOrEmpty(accountViewModel.SearchInvoiceSWIFT))
                accountInvoicesToLimit = accountInvoicesToLimit.Where(acc => acc.BankName.ToLower().Contains(accountViewModel.SearchInvoiceSWIFT)).ToList();
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
            AccountViewModel accountViewModelCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_QUOTES);
            List<AccountQuote> limitedAccQuotes = accountQuotes;
            LimitListByPageNumber(ACC_QUOTES, ref limitedAccQuotes);
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
            accountViewModel.AccountManagers = accountViewModel.GetAccTeam(context).GetViewModelsFromData
                <AccountManager, AccountManagerViewModel>(new AccountManagerMap(serviceProvider, context));
        }

        #endregion

        #region Validations
        #region INN Checks
        /// <summary>
        /// Проверка ИНН при создании клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckINNOnCreate(AccountViewModel accountViewModel)
        {
            string inn = accountViewModel.INN.TrimStartAndEnd();
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CheckINN(inn),
                () => CheckINNUnique(inn)
            });
        }

        /// <summary>
        /// Проверка ИНН при обновлении клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckINNOnUpdate(AccountViewModel accountViewModel)
        {
            string inn = accountViewModel.INN.TrimStartAndEnd();
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CheckINN(inn),
                () => CheckINNUnique(inn, accountViewModel)
            });
        }

        /// <summary>
        /// Проверка ИНН на корректность
        /// </summary>
        /// <param name="inn"></param>
        private void CheckINN(string inn)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (string.IsNullOrEmpty(inn) || (inn.Length != INN_TEN_LENGTH && inn.Length != INN_TWELVE_LENGTH))
                        errors.Add("INNLength", resManager.GetString("INNLength"));
                },
                () => {
                    if (ONLY_DIGITS.IsMatch(inn))
                        errors.Add("INNWrong", resManager.GetString("INNWrong"));
                },
                () =>
                {
                    if (inn.Length == INN_TEN_LENGTH)
                        CheckTenCharactersINN(inn);
                    else CheckTwelveCharactersINN(inn);
                }
            });
        }

        /// <summary>
        /// Проверка ИНН при длине в 10 символов 
        /// </summary>
        /// <param name="inn"></param>
        private void CheckTenCharactersINN(string inn)
        {
            // Нахождение суммы произведений цифр ИНН на соответствующий множитель
            int numeralSumm = 0;
            for (int numeralIndex = 0; numeralIndex < INN_TEN_LENGTH - 1; numeralIndex++)
            {
                string numeral = inn[numeralIndex].ToString();
                numeralSumm += Convert.ToInt32(numeral) * INN_10_NUM_FACTORS[numeralIndex];
            }

            // Контрольная сумма
            int checkNumber = GetCheckNumber(numeralSumm);

            // Сравнение контрольной суммы с 10 цифрой в ИНН
            string lastNumeral = inn[INN_TEN_LENGTH - 1].ToString();
            if (checkNumber != Convert.ToInt32(lastNumeral))
                errors.Add("INNWrong", resManager.GetString("INNWrong"));
        }

        /// <summary>
        /// Проверка ИНН при длине в 12 символов 
        /// </summary>
        /// <param name="inn"></param>
        private void CheckTwelveCharactersINN(string inn)
        {
            // Нахождение суммы произведений цифр ИНН на соответствующий множитель
            int firstNumeralSumm = 0;
            for (int numeralIndex = 0; numeralIndex < INN_TEN_LENGTH; numeralIndex++)
            {
                string numeral = inn[numeralIndex].ToString();
                firstNumeralSumm += Convert.ToInt32(numeral) * INN_12_NUM_FACTORS_FIRST_CHECK[numeralIndex];
            }

            int firstCheckNumber = GetCheckNumber(firstNumeralSumm);

            // Проверка на равенство первого контрольного числа с 11-й цифрой ИНН
            string elevenINNNumeral = inn[INN_TEN_LENGTH].ToString();
            if (firstCheckNumber != Convert.ToInt32(elevenINNNumeral))
            {
                errors.Add("INNWrong", resManager.GetString("INNWrong"));
                return;
            }

            // Если проверка пройдена, вычисляется следующее контрольное число
            // Нахождение суммы произведений цифр ИНН на соответствующий множитель
            int lastNumeralSumm = 0;
            for (int numeralIndex = 0; numeralIndex < INN_TEN_LENGTH + 1; numeralIndex++)
            {
                string numeral = inn[numeralIndex].ToString();
                lastNumeralSumm += Convert.ToInt32(numeral) * INN_12_NUM_FACTORS_LAST_CHECK[numeralIndex];
            }

            // Второе контрольное число
            int lastCheckNumber = GetCheckNumber(lastNumeralSumm);

            // Проверка на равенство второго контрольного числа с 12-й цифрой ИНН
            string twelveINNNumeral = inn[INN_TEN_LENGTH + 1].ToString();
            if (lastCheckNumber != Convert.ToInt32(twelveINNNumeral))
                errors.Add("INNWrong", resManager.GetString("INNWrong"));
        }

        /// <summary>
        /// Метод вычисляет контрольное число
        /// </summary>
        /// <param name="numeralSumm">Сумма произведений цифр ИНН на соответствующие множители</param>
        /// <returns></returns>
        private int GetCheckNumber(int numeralSumm)
        {
            // Число "numeralSumm" делится на константу INN_DIVIDER для получения целой части, затем умножается на нее
            int numeralQuotient = numeralSumm / INN_DIVIDER;
            int numeralComposition = numeralQuotient * INN_DIVIDER;

            // Получение и возврат контрольной суммы
            int checkNumber = numeralSumm - numeralComposition;
            if (checkNumber < 0)
                checkNumber *= (-1);
            if (checkNumber == 10)
                checkNumber = 0;
            return checkNumber;
        }

        /// <summary>
        /// Проверка, что не существует клиента с таким ИНН (используется при создании)
        /// </summary>
        /// <param name="inn"></param>
        private void CheckINNUnique(string inn)
        {
            Account accountWithSameINN = context.GetOrgAccounts(currentUser.PrimaryOrganizationId).FirstOrDefault(i => i.INN == inn);
            if (accountWithSameINN != null)
                errors.Add("INNNotUnique", resManager.GetString("INNNotUnique"));
        }
        
        /// <summary>
         /// Проверка, что не существует клиента с таким ИНН (используется при обновлении, так как необходимо исключить инн обновляемого клиента из поиска)
         /// </summary>
         /// <param name="inn"></param>
         /// <param name="updatedAccount">Клиент, инн которого обновляется</param>
        private void CheckINNUnique(string inn, AccountViewModel updatedAccount)
        {
            // Список всех клиентов той организации, к которой относится обновляемый клиент
            List<Account> orgAccounts = context.GetOrgAccounts(updatedAccount.OrganizationId);
            Account accountWithSameINN = orgAccounts.FirstOrDefault(i => i.INN == inn && i.Id != updatedAccount.Id);
            if (accountWithSameINN != null)
                errors.Add("INNNotUnique", resManager.GetString("INNNotUnique"));
        }
        #endregion

        #region KPP Chekcs
        /// <summary>
        /// Проверка КПП при создании клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckKPPOnCreate(AccountViewModel accountViewModel)
        {
            string kpp = accountViewModel.KPP.TrimStartAndEnd();
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CheckKPP(kpp),
                () => CheckKPPUnique(kpp)
            });
        }

        /// <summary>
        /// Проверка КПП при обновлении клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckKPPOnUpdate(AccountViewModel accountViewModel)
        {
            string kpp = accountViewModel.KPP.TrimStartAndEnd();
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CheckKPP(kpp),
                () => CheckKPPUnique(kpp, accountViewModel)
            });
        }

        /// <summary>
        /// Проверка КПП на корректность
        /// </summary>
        /// <param name="kpp"></param>
        private void CheckKPP(string kpp)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (string.IsNullOrEmpty(kpp) || kpp.Length != KPP_LENGTH)
                        errors.Add("KPPLength", resManager.GetString("KPPLength"));
                },
                () => {
                    if (ONLY_DIGITS.IsMatch(kpp))
                        errors.Add("KPPWrong", resManager.GetString("KPPWrong"));
                },
                () => {
                }
            });
        }

        /// <summary>
        /// Проверка, что не существует клиента с таким КПП (используется при создании)
        /// </summary>
        /// <param name="kpp"></param>
        private void CheckKPPUnique(string kpp)
        {
            Account accountWithSameKPP = context.GetAccountsByType(currentUser.PrimaryOrganizationId, AccountType.LegalEntity).FirstOrDefault(k => k.KPP == kpp);
            if (accountWithSameKPP != null)
                errors.Add("KPPNotUnique", resManager.GetString("KPPNotUnique"));
        }

        /// <summary>
        /// Проверка, что не существует клиента с таким КПП (используется при обновлении, так как необходимо исключить КПП обновляемого клиента из поиска)
        /// </summary>
        /// <param name="kpp"></param>
        /// <param name="updatedAccount"></param>
        private void CheckKPPUnique(string kpp, AccountViewModel updatedAccount)
        {
            // Список всех клиентов той организации, к которой относится обновляемый клиент
            List<Account> orgAccounts = context.GetOrgAccounts(updatedAccount.OrganizationId);
            Account accountWithSameKPP = orgAccounts.FirstOrDefault(k => k.KPP == kpp && k.Id != updatedAccount.Id);
            if (accountWithSameKPP != null)
                errors.Add("KPPNotUnique", resManager.GetString("KPPNotUnique"));
        }
        #endregion

        #region OKPO Checks
        /// <summary>
        /// Проверка ОКПО при создании клиента
        /// </summary>
        /// <param name="okpo"></param>
        private void CheckOKPOOnCreate(AccountViewModel accountViewModel)
        {
            string okpo = accountViewModel.OKPO?.TrimStartAndEnd();
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CheckOKPO(okpo),
                () => CheckOKPOUnique(okpo)
            });
        }

        /// <summary>
        /// Проверка ОКПО при обновлении клиента
        /// </summary>
        /// <param name="okpo"></param>
        private void CheckOKPOOnUpdate(AccountViewModel accountViewModel)
        {
            string okpo = accountViewModel.OKPO?.TrimStartAndEnd();
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CheckOKPO(okpo),
                () => CheckOKPOUnique(okpo, accountViewModel)
            });
        }

        /// <summary>
        /// Проверка ОКПО на корректность
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckOKPO(string okpo)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (string.IsNullOrEmpty(okpo) || okpo.Length != OKPO_LENGTH)
                        errors.Add("OKPOLength", resManager.GetString("OKPOLength"));
                },
                () => {
                    if (ONLY_DIGITS.IsMatch(okpo))
                        errors.Add("OKPOWrong", resManager.GetString("OKPOWrong"));
                },
                () => CheckOKPORight(okpo)
            });
        }

        /// <summary>
        /// Проверка ОКПО по алгоритму
        /// </summary>
        /// <param name="okpo"></param>
        private void CheckOKPORight(string okpo)
        {
            // Нахождение суммы произведений цифр ОКПО на соответствующий множитель
            int firstNumeralSumm = 0;
            for (int numeralIndex = 0; numeralIndex < OKPO_LENGTH - 1; numeralIndex++)
            {
                string numeral = okpo[numeralIndex].ToString();
                firstNumeralSumm += Convert.ToInt32(numeral) * OKPO_NUM_FACTORS_FIRST_CHECK[numeralIndex];
            }

            // Число "firstNumeralSumm" делится на константу OKPO_DIVIDER для получения остатка от деления
            int firstNumeralQuotient = firstNumeralSumm % OKPO_DIVIDER;

            // Получение контрольного числа
            int checkNumeral;

            // Если остаток от деления не равен 10, то контрольному числу присваивается это значение
            if (firstNumeralQuotient != 10)
                checkNumeral = firstNumeralQuotient;

            // Иначе необходим пересчет
            else
            {
                // Повторное нахождение суммы произведений цифр ОКПО на соответствующий множитель
                int lastNumeralSumm = 0;
                for (int numeralIndex = 0; numeralIndex < OKPO_LENGTH - 1; numeralIndex++)
                {
                    string numeral = okpo[numeralIndex].ToString();
                    lastNumeralSumm += Convert.ToInt32(numeral) * OKPO_NUM_FACTORS_LAST_CHECK[numeralIndex];
                }

                // Число "lastNumeralSumm" делится на константу OKPO_DIVIDER для получения остатка от деления
                int lastNumeralQuotient = lastNumeralSumm % OKPO_DIVIDER;

                // Если после повторной проверки с другими множителями значение остатка от деления остается равным 10, контрольное число становится равным 0, иначе - остаток от деления
                if (lastNumeralQuotient != 10)
                    checkNumeral = lastNumeralQuotient;
                else checkNumeral = 0;
            }

            // Проверка на равенство контрольного числа с 8-й цифрой ОКПО
            string lastOKPONumeral = okpo[OKPO_LENGTH - 1].ToString();
            if (checkNumeral != Convert.ToInt32(lastOKPONumeral))
                errors.Add("OKPOWrong", resManager.GetString("OKPOWrong"));
        }

        /// <summary>
        /// Проверка, что не существует клиента с таким ОКПО (используется при создании)
        /// </summary>
        /// <param name="okpo"></param>
        private void CheckOKPOUnique(string okpo)
        {
            Account accountWithSameOKPO = context.GetAccountsByType(currentUser.PrimaryOrganizationId, AccountType.LegalEntity).FirstOrDefault(o => o.OKPO == okpo);
            if (accountWithSameOKPO != null)
                errors.Add("OKPONotUnique", resManager.GetString("OKPONotUnique"));
        }

        /// <summary>
        /// Проверка, что не существует клиента с таким ОКПО (используется при обновлении, так как необходимо исключить ОКПО обновляемого клиента из поиска)
        /// </summary>
        /// <param name="okpo"></param>
        /// <param name="updatedAccount"></param>
        private void CheckOKPOUnique(string okpo, AccountViewModel updatedAccount)
        {
            // Список всех клиентов той организации, к которой относится обновляемый клиент
            List<Account> orgAccounts = context.GetOrgAccounts(updatedAccount.OrganizationId);
            Account accountWithSameOKPO = orgAccounts.FirstOrDefault(o => o.OKPO == okpo && o.Id != updatedAccount.Id);
            if (accountWithSameOKPO != null)
                errors.Add("OKPONotUnique", resManager.GetString("OKPONotUnique"));
        }
        #endregion

        #region OGRN Checks
        /// <summary>
        /// Проверка ОГРН при создании клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckOGRNOnCreate(AccountViewModel accountViewModel)
        {
            string ogrn = accountViewModel.OGRN?.TrimStartAndEnd();
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CheckOGRN(ogrn),
                () => CheckOGRNUnique(ogrn)
            });
        }

        /// <summary>
        /// Проверка ОГРН при обновлении клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckOGRNOnUpdate(AccountViewModel accountViewModel)
        {
            string ogrn = accountViewModel.OGRN?.TrimStartAndEnd();
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CheckOGRN(ogrn),
                () => CheckOGRNUnique(ogrn, accountViewModel)
            });
        }

        /// <summary>
        /// Проверка ОГРН на корректность
        /// </summary>
        /// <param name="ogrn"></param>
        private void CheckOGRN(string ogrn)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (string.IsNullOrEmpty(ogrn) || ogrn.Length != OGRN_LENGTH)
                        errors.Add("OGRNLength", resManager.GetString("OGRNLength"));
                },
                () => {
                    if (ONLY_DIGITS.IsMatch(ogrn))
                        errors.Add("OGRNWrong", resManager.GetString("OGRNWrong"));
                },
                () => CheckOGRNRight(ogrn)
            });
        }

        /// <summary>
        /// Проверка ОГРН по алгоритму
        /// </summary>
        /// <param name="ogrn"></param>
        private void CheckOGRNRight(string ogrn)
        {
            // Вычисление остатка от деления первых 12 цифр ОГРН на константу "OGRN_DIVIDER"
            string ogrnNumerals = ogrn.TakeToString(OGRN_LENGTH - 1);
            string numeralQuotient = (Convert.ToInt64(ogrnNumerals) % OGRN_DIVIDER).ToString();

            // Получение контрольного числа - младшего разряда числа, полученного на предыдущем шаге
            int leastSignificantDigit = Convert.ToInt32(numeralQuotient[^1].ToString());

            // Проверка на равенство контрольного числа с 13-й цифрой ОГРН
            string lastOGRNNumeral = ogrn[OGRN_LENGTH - 1].ToString();
            if (leastSignificantDigit != Convert.ToInt32(lastOGRNNumeral))
                errors.Add("OGRNWrong", resManager.GetString("OGRNWrong"));
        }

        /// <summary>
        /// Проверка, что не существует клиента с таким ОГРН (используется при создании)
        /// </summary>
        /// <param name="ogrn"></param>
        private void CheckOGRNUnique(string ogrn)
        {
            Account accountWithSameOGRN = context.GetAccountsByType(currentUser.PrimaryOrganizationId, AccountType.LegalEntity).FirstOrDefault(o => o.OGRN == ogrn);
            if (accountWithSameOGRN != null)
                errors.Add("OGRNNotUnique", resManager.GetString("OGRNNotUnique"));
        }

        /// <summary>
        /// Проверка, что не существует клиента с таким ОГРН (используется при обновлении, так как необходимо исключить ОГРН обновляемого клиента из поиска)
        /// </summary>
        /// <param name="ogrn"></param>
        /// <param name="updatedAccount"></param>
        private void CheckOGRNUnique(string ogrn, AccountViewModel updatedAccount)
        {
            // Список всех клиентов той организации, к которой относится обновляемый клиент
            List<Account> orgAccounts = context.GetOrgAccounts(updatedAccount.OrganizationId);
            Account accountWithSameOGRN = orgAccounts.FirstOrDefault(o => o.OGRN == ogrn && o.Id != updatedAccount.Id);
            if (accountWithSameOGRN != null)
                errors.Add("OGRNNotUnique", resManager.GetString("OGRNNotUnique"));
        }
        #endregion

        #region Other Checks
        /// <summary>
        /// Проверка на то, что тип клиента является допустимым
        /// </summary>
        /// <param name="accountViewModel"></param>
        private bool TryCheckAccountType(AccountViewModel accountViewModel)
        {
            if (!Enum.TryParse(typeof(AccountType), accountViewModel.AccountType, out object type))
            {
                errors.Add("WrongAccountType", resManager.GetString("WrongAccountType"));
                return false;
            }
            transaction.AddParameter("AccountType", (AccountType)type);
            return true;
        }

        /// <summary>
        /// Проверка названия ИП при создании клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckIENameOnCreate(AccountViewModel accountViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (string.IsNullOrEmpty(accountViewModel.Name.TrimStartAndEnd()))
                        errors.Add("IENameLength", resManager.GetString("IENameLength"));
                },
                () => CheckIENameNotExistsOnCreate(accountViewModel)
            });
        }

        /// <summary>
        /// Проверка названия ИП при обновлении клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckIENameOnUpdate(AccountViewModel accountViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (string.IsNullOrEmpty(accountViewModel.Name.TrimStartAndEnd()))
                        errors.Add("IENameLength", resManager.GetString("IENameLength"));
                },
                () => CheckIENameNotExistsOnUpdate(accountViewModel)
            });
        }

        /// <summary>
        /// Проверка на отсутствие ИП с таким же именем в бд
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckIENameNotExistsOnCreate(AccountViewModel accountViewModel)
        {
            List<Account> orgAccounts = context.GetOrgAccounts(currentUser.PrimaryOrganizationId);
            Account ieWithSameName = orgAccounts.FirstOrDefault(n => n.Name == accountViewModel.Name.TrimStartAndEnd());
            if (ieWithSameName != null)
                errors.Add("AccountAlreadyExists", resManager.GetString("AccountAlreadyExists"));
        }

        /// <summary>
        /// Проверка на отсутствие ИП с таким же именем в бд (используется при обновлении, так как необходимо исключить обновляемого клиента из поиска)
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckIENameNotExistsOnUpdate(AccountViewModel accountViewModel)
        {
            // Список всех клиентов, созданных организацией, к которой относится обновляемый клиент
            List<Account> orgAccounts = context.GetOrgAccounts(accountViewModel.OrganizationId);
            Account ieWithSameName = orgAccounts.FirstOrDefault(acc => acc.Name == accountViewModel.Name.TrimStartAndEnd() && acc.Id != accountViewModel.Id);
            if (ieWithSameName != null)
                errors.Add("AccountAlreadyExists", resManager.GetString("AccountAlreadyExists"));
        }

        /// <summary>
        /// Проверка названия юридического лица при создании клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckLENameOnCreate(AccountViewModel accountViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (string.IsNullOrEmpty(accountViewModel.Name.TrimStartAndEnd()))
                        errors.Add("LENameLength", resManager.GetString("LENameLength"));
                },
                () => CheckLENameNotExistsOnCreate(accountViewModel)
            });
        }

        /// <summary>
        /// Проверка названия юридического лица при обновлении клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckLENameOnUpdate(AccountViewModel accountViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (string.IsNullOrEmpty(accountViewModel.Name.TrimStartAndEnd()))
                        errors.Add("LENameLength", resManager.GetString("LENameLength"));
                },
                () => CheckLENameNotExistsOnUpdate(accountViewModel)
            });
        }

        /// <summary>
        /// Проверка на отсутствие юридического лица с таким же именем в бд
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckLENameNotExistsOnCreate(AccountViewModel accountViewModel)
        {
            List<Account> orgAccounts = context.GetOrgAccounts(currentUser.PrimaryOrganizationId);
            Account leWithSameName = orgAccounts.FirstOrDefault(n => n.Name == accountViewModel.Name.TrimStartAndEnd());
            if (leWithSameName != null)
                errors.Add("AccountAlreadyExists", resManager.GetString("AccountAlreadyExists"));
        }

        /// <summary>
        /// Проверка на отсутствие юридического лица с таким же именем в бд (используется при обновлении, так как необходимо исключить обновляемого клиента из поиска)
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckLENameNotExistsOnUpdate(AccountViewModel accountViewModel)
        {
            // Список всех клиентов, созданных организацией, к которой относится обновляемый клиент
            List<Account> orgAccounts = context.GetOrgAccounts(accountViewModel.OrganizationId);
            Account leWithSameName = orgAccounts.FirstOrDefault(acc => acc.Name == accountViewModel.Name.TrimStartAndEnd() && acc.Id != accountViewModel.Id);
            if (leWithSameName != null)
                errors.Add("AccountAlreadyExists", resManager.GetString("AccountAlreadyExists"));
        }

        /// <summary>
        /// Проверка страны на существование
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckCountry(AccountViewModel accountViewModel)
        {
            string country = accountViewModel.Country.TrimStartAndEnd();
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (string.IsNullOrEmpty(country))
                        errors.Add("CoutnryLength", resManager.GetString("CoutnryLength"));
                },
                () => CheckCountryExists(country)
            });
        }

        /// <summary>
        /// Проверка страны на существование
        /// </summary>
        /// <param name="country"></param>
        private void CheckCountryExists(string country)
        {
            JArray countries = AppUtils.GetCountries(currentUser.DefaultLanguage);
            Func<JToken, bool> predicate = n => n.ToString().ToLower() == country.ToLower().TrimStartAndEnd();
            JToken findCountry = countries.FirstOrDefault(predicate);
            if (findCountry == null)
                errors.Add("CountryNotExists", resManager.GetString("CountryNotExists"));
        }

        /// <summary>
        /// Проверка выбранного менеджера
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckPrimaryManager(AccountViewModel accountViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (!accountViewModel.AppointMe && string.IsNullOrEmpty(accountViewModel.PrimaryManagerInitialName.TrimStartAndEnd()))
                        errors.Add("PrimaryManagerNameLength", resManager.GetString("PrimaryManagerNameLength"));
                },
                () => CheckManagerExists(accountViewModel)
            });
        }

        /// <summary>
        /// Проверка менеджера с таким именем на существование
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckManagerExists(AccountViewModel accountViewModel)
        {
            Employee manager = default;
            Guid organizationId = (Guid)transaction.GetParameterValue("OrganizationId");
            if (accountViewModel.AppointMe)
                manager = context.GetCurrentEmployee(organizationId, Guid.Parse(currentUser.Id));
            else
            {
                List<Employee> orgEmployees = context.GetOrgEmployees(organizationId);
                string initialManagerName = accountViewModel.PrimaryManagerInitialName.TrimStartAndEnd();
                manager = orgEmployees.FirstOrDefault(n => n.GetIntialsFullName().ToLower() == initialManagerName.ToLower());
            }
            if (manager == null)
            {
                errors.Add("ManagerNotExists", resManager.GetString("ManagerNotExists"));
                return;
            }
            transaction.AddParameter("Manager", manager);
        }

        /// <summary>
        /// Метод выполняет проверки, необходимые при изменении сайта
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        private bool TryChangeSiteValidate(string accountId)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (!CheckPermissionForAccountGroup("AccUpdate", transaction))
                        AddHasNoPermissionsError(OperationType.Update);
                },
                () => {
                    if (!TryGetItemById(accountId, out Account account))
                        errors.Add("AccountNotFound", resManager.GetString("AccountNotFound"));
                    else transaction.AddParameter("Account", account);
                },
                () => {
                    Account account = (Account)transaction.GetParameterValue("Account");
                    if (context.AccountManagers.AsNoTracking().FirstOrDefault(i => i.Id == account.PrimaryManagerId) == null)
                        errors.Add("AccountWithoutKMIsReadonly", resManager.GetString("AccountWithoutKMIsReadonly"));
                }
            });
            return !errors.Any();
        }

        /// <summary>
        /// Метод выполняет проверки, необходимые при изенении основного контакта на клиенте
        /// </summary>
        /// <param name="accountViewModel"></param>
        /// <returns></returns>
        public bool TryChangePrimaryContactValidate(AccountViewModel accountViewModel)
        {
            bool needStop = default;
            Account account = (Account)transaction.GetParameterValue("CurrentAccount");
            Guid primaryContactId = default;
            InvokeIntermittinActions(errors, needStop, new List<Action>()
            {
                // Проверка полномочий
                () => {
                    if (!CheckPermissionForAccountGroup("AccUpdate", transaction))
                        AddHasNoPermissionsError(OperationType.ChangeAccountPrimaryContact);
                },
                // Для физических лиц основной контакт является обязательным
                () => {
                    if (account.AccountType == AccountType.Individual && string.IsNullOrEmpty(accountViewModel.PrimaryContactId))
                        errors.Add("PrimaryIndividualContactIsRequired", resManager.GetString("PrimaryIndividualContactIsRequired"));
                },
                // Для остальных типов клиентов основной контакт не является обязательным, поэтому осуществляется возврат из метода
                () => {
                    if (account.AccountType != AccountType.Individual && string.IsNullOrEmpty(accountViewModel.PrimaryContactId))
                        needStop = true;
                },
                // Если не получается распарсить строку с контактом, возвращается ошибка
                () => {
                    if (!Guid.TryParse(accountViewModel.PrimaryContactId, out Guid guid))
                        errors.Add("AccountContactNotFound", resManager.GetString("AccountContactNotFound"));
                    else
                    {
                        primaryContactId = guid;
                        transaction.AddParameter("PrimaryContactId", primaryContactId);
                    }
                },
                // Если контакт не найден, также возвращается ошибка
                () => {
                    AccountContact accountContact = account.GetContacts(context).FirstOrDefault(i => i.Id == primaryContactId);
                    if (accountContact == null)
                        errors.Add("AccountContactNotFound", resManager.GetString("AccountContactNotFound"));
                    else transaction.AddParameter("AccountContact", accountContact);
                }
            });
            return !errors.Any();
        }
        #endregion
        #endregion

        #region Other
        /// <summary>
        /// Метод пытается изменить основной контакт на клиенте
        /// </summary>
        /// <param name="accountViewModel"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public bool TryChangePrimaryContact(AccountViewModel accountViewModel, out Dictionary<string, string> errors)
        {
            errors = this.errors;
            transaction = viewModelsTransactionFactory.Create(currentUser.Id, OperationType.ChangeAccountPrimaryContact, accountViewModel);
            if (TryChangePrimaryContactValidate(accountViewModel))
            {
                // В зависимости от типа клиента
                Account account = (Account)transaction.GetParameterValue("CurrentAccount");
                switch (account.AccountType)
                {
                    // Для физических лиц основной контакт является обязательным
                    case AccountType.Individual:
                        Guid primaryContactId = (Guid)transaction.GetParameterValue("PrimaryContactId");
                        AccountContact primaryContact = (AccountContact)transaction.GetParameterValue("AccountContact");
                        account.PrimaryContactId = primaryContactId;
                        account.Name = primaryContact.GetFullName();
                        break;

                    // Для остальных типов клиентов основной контакт может отсутствовать
                    case AccountType.IndividualEntrepreneur:
                        account.PrimaryContactId = (Guid)transaction.GetParameterValue("PrimaryContactId");
                        break;
                }
                transaction.AddChange(account, EntityState.Modified);
                
                // Попытка закоммитить
                if (viewModelsTransactionFactory.TryCommit(transaction, this.errors))
                {
                    viewModelsTransactionFactory.Close(transaction);
                    return true;
                }
            }

            // Добавление ошибок, закрытие транзакции и выход
            errors = this.errors;
            viewModelsTransactionFactory.Close(transaction, TransactionStatus.Error);
            return false;
        }

        /// <summary>
        /// Метод пытается изменить сайт
        /// </summary>
        /// <param name="accountId">Id клиента, для которого надо изменить сайт</param>
        /// <param name="errors">Список ошибок, возникших в процессе</param>
        /// <param name="newSite">Новое название сайта</param>
        /// <returns></returns>
        public bool TryChangeSite(string accountId, out Dictionary<string, string> errors, string newSite = null)
        {
            transaction = viewModelsTransactionFactory.Create(currentUser.Id, OperationType.Update, accountId);
            if (TryChangeSiteValidate(accountId))
            {
                Account account = (Account)transaction.GetParameterValue("Account");
                account.Site = newSite;
                transaction.AddParameter("ChangedRecord", account);
                transaction.AddChange(account, EntityState.Modified);
                if (viewModelsTransactionFactory.TryCommit(transaction, this.errors))
                {
                    viewModelsTransactionFactory.Close(transaction);
                    errors = this.errors;
                    return true;
                }
            }
            viewModelsTransactionFactory.Close(transaction, TransactionStatus.Error);
            errors = this.errors;
            return false;
        }

        /// <summary>
        /// Метод пытается добавить в команду по клиенту нового менеджера
        /// </summary>
        /// <param name="accountViewModel"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public bool TryUnlockAccount(AccountViewModel accountViewModel, out Dictionary<string, string> errors)
        {
            errors = this.errors;
            transaction = viewModelsTransactionFactory.Create(currentUser.Id, OperationType.UnlockAccount, accountViewModel);
            Employee employee = default;
            InvokeIntermittinActions(errors, new List<Action>()
            {
                // Проверка полномочий
                () => {
                    if (!CheckPermissionForAccountGroup("AccUnlock", transaction))
                        AddHasNoPermissionsError(OperationType.UnlockAccount);
                },
                // Проверка, что существует такой сотрудник
                () => {
                    if (!new EmployeeRepository(serviceProvider, context).TryGetRefItemById(accountViewModel.NewPrimaryManagerId, ref employee))
                        this.errors.Add("EmployeeNotExists", resManager.GetString("EmployeeNotExists"));
                }
            });

            // Если нет ошибок происходит маппинг
            if (!this.errors.Any())
            {
                new AccountMap(serviceProvider, context).UnlockAccount(employee);
                if (viewModelsTransactionFactory.TryCommit(transaction, this.errors))
                {
                    viewModelsTransactionFactory.Close(transaction);
                    return true;
                }
            }

            // Запись ошибок, закрытие транзакции и выход
            errors = this.errors;
            viewModelsTransactionFactory.Close(transaction, TransactionStatus.Error);
            return false;
        }

        /// <summary>
        /// Метод проверяет клиентов, у которых поданный сотрудник является основным клиентским менеджером
        /// Если в команде найденного клиента присутствуют другие менеджеры, метод делает основным случайного
        /// Иначе блокирует клиента
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="transaction"></param>
        public void CheckAccountsForLock(Employee employee, ITransaction transaction)
        {
            // Список всех основных клиентских менеджеров, где сотрудником является поданный "employee"
            Func<AccountManager, bool> predicate = accNam => accNam.ManagerId == employee.Id && accNam.Account.PrimaryManagerId == accNam.Id;
            List<AccountManager> accountManagers = context.AccountManagers.AsNoTracking().Include(acc => acc.Account).Where(predicate).ToList();
            accountManagers.Select(acc => acc.Account).ToList().ForEach(account =>
            {
                List<AccountManager> allAccManagers = context.AccountManagers.AsNoTracking().Where(acc => acc.AccountId == account.Id).ToList();
                if (allAccManagers.Count <= 1)
                {
                    account.AccountStatus = AccountStatus.Lock;
                    account.PrimaryManagerId = Guid.Empty;
                }
                else account.PrimaryManagerId = allAccManagers.FirstOrDefault(i => i.ManagerId != employee.Id).Id;
                transaction.AddChange(account, EntityState.Modified);
            });
        }

        /// <summary>
        /// Метод проверяет, имеет ли сотрудник разрешение на выполнение поданной на вход операции для всех сущностей, относящихся к клиенту
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public bool CheckPermissionForAccountGroup(string actionName, ITransaction transaction)
        {
            Account currentAccount = (Account)transaction.GetParameterValue("CurrentAccount");
            Organization accountOwnerOrg = context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == currentAccount.OrganizationId);
            if (accountOwnerOrg == null) return false;
            if (!currentUser.NeedCheckResps(accountOwnerOrg)) return true;
            Employee currentEmployee = context.GetCurrentEmployee(accountOwnerOrg, Guid.Parse(currentUser.Id));
            return currentEmployee != null && currentEmployee.HasPermissionFor(actionName, context);
        }
        #endregion
    }
}
