using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Models.ViewTypes;
using System;
using System.Linq;
using static GSCrm.CommonConsts;
using Microsoft.EntityFrameworkCore;
using GSCrm.Transactions;
using GSCrm.Models.Enums;
using System.Collections.Generic;

namespace GSCrm.Mapping
{
    public class AccountMap : BaseMap<Account, AccountViewModel>
    {
        #region Construts
        public AccountMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }
        #endregion

        public override Account OnModelCreate(AccountViewModel accountViewModel)
        {
            // Поля клиента и список контактов
            base.OnModelCreate(accountViewModel);
            string name = string.Empty;
            string kpp = string.Empty;
            string okpo = string.Empty;
            string ogrn = string.Empty;
            Guid newAccountId = Guid.NewGuid();
            Guid primaryContactId = Guid.Empty;

            // В зависимости от типа клиента инициализация переменных разными значениями
            AccountType accountType = (AccountType)transaction.GetParameterValue("AccountType");
            switch (accountType)
            {
                // Физическое лицо
                case AccountType.Individual:
                    name = accountViewModel.GetIndividualFullName();
                    AccountContact accountContact = GetNewAccountContact(accountViewModel, newAccountId);
                    primaryContactId = accountContact.Id;
                    transaction.AddChange(accountContact, EntityState.Added);
                    break;

                // ИП
                case AccountType.IndividualEntrepreneur:
                    name = accountViewModel.Name;
                    kpp = accountViewModel.KPP;
                    break;

                // Юридическое лицо
                case AccountType.LegalEntity:
                    name = accountViewModel.Name;
                    kpp = accountViewModel.KPP;
                    okpo = accountViewModel.OKPO;
                    ogrn = accountViewModel.OGRN;
                    break;
            }

            // Создание адреса - при любом типе клиента
            AccountAddress accountAddress = GetNewAccountAddress(accountViewModel, newAccountId);
            Guid legalAddressId = accountAddress.Id;
            transaction.AddChange(accountAddress, EntityState.Added);

            // Привязка существующего сотрудника организации как основного менеджера по клиенту
            AccountManager accountManager = GetNewAccountManager(newAccountId);
            Guid primaryManagerId = accountManager.Id;
            transaction.AddChange(accountManager, EntityState.Added);

            return new Account()
            {
                Id = newAccountId,
                Name = name,
                INN = accountViewModel.INN,
                KPP = kpp,
                OKPO = okpo,
                OGRN = ogrn,
                AccountStatus = AccountStatus.Active,
                AccountType = accountType,
                OrganizationId = (Guid)transaction.GetParameterValue("OrganizationId"),
                ParentAccountId = Guid.Empty,
                PrimaryContactId = primaryContactId,
                LegalAddressId = legalAddressId,
                PrimaryManagerId = primaryManagerId
            };
        }

        public override Account OnModelUpdate(AccountViewModel accountViewModel)
        {
            // Клиент, которого необходимо обновить
            Account account = base.OnModelUpdate(accountViewModel);

            // В зависимости от типа клиента менять разные поля
            switch (account.AccountType)
            {
                case AccountType.Individual:
                    account.INN = accountViewModel.INN;
                    break;

                case AccountType.IndividualEntrepreneur:
                    account.Name = accountViewModel.Name;
                    account.INN = accountViewModel.INN;
                    break;

                case AccountType.LegalEntity:
                    account.Name = accountViewModel.Name;
                    account.INN = accountViewModel.INN;
                    account.KPP = accountViewModel.KPP;
                    account.OKPO = accountViewModel.OKPO;
                    account.OGRN = accountViewModel.OGRN;
                    break;
            }

            return account;
        }

        /// <summary>
        /// Метод инициализирует поля модели списка клиентов
        /// </summary>
        /// <param name="accountsViewModel"></param>
        public void InitializeAccountsViewModel(AccountsViewModel accountsViewModel)
        {
            // Проставление списка всех организаций, в которых состоит пользователь
            List<Organization> organizations = context.UserOrganizations
                .AsNoTracking().Include(org => org.Organization)
                .Where(userOrg => userOrg.UserId == currentUser.Id)
                .Select(org => org.Organization).ToList();
            accountsViewModel.UserOrganizations = organizations.GetViewModelsFromData(new OrganizationMap(serviceProvider, context));

            // Проставление основной организации
            Organization primaryOrganization = organizations.FirstOrDefault(i => i.Id == currentUser.PrimaryOrganizationId);
            if (primaryOrganization != null)
                accountsViewModel.PrimaryOrganization = new OrganizationMap(serviceProvider, context).DataToViewModel(primaryOrganization);


            // Проставление значений в поля фильтров
            AccountsViewModel allAccsViewModel = cachService.GetCachedItem<AccountsViewModel>(currentUser.Id, ALL_ACCS);
            AccountsViewModel currentAccsViewModel = cachService.GetCachedItem<AccountsViewModel>(currentUser.Id, CURRENT_ACCS);
            accountsViewModel.AllAccountsSearchName = allAccsViewModel.AllAccountsSearchName;
            accountsViewModel.AllAccountsSearchType = allAccsViewModel.AllAccountsSearchType;
            accountsViewModel.CurrentAccountsSearchName = currentAccsViewModel.CurrentAccountsSearchName;
            accountsViewModel.CurrentAccountsSearchType = currentAccsViewModel.CurrentAccountsSearchType;
        }

        public override AccountViewModel DataToViewModel(Account account)
        {
            Organization organization = context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == account.OrganizationId);
            AccountAddress legalAddress = account.GetAddresses(context).FirstOrDefault(addr => addr.AddressType == AddressType.Legal);
            return new AccountViewModel()
            {
                Id = account.Id,
                AccountStatus = account.AccountStatus,
                AccountType = account.AccountType.ToString(),
                PrimaryContactId = account.PrimaryContactId == Guid.Empty ? string.Empty : account.PrimaryContactId.ToString(),
                OrganizationId = organization.Id,
                OrganizationName = organization.Name,
                LegalAddress = legalAddress?.GetFullAddress(currentUser),
                Name = account.Name,
                INN = account.INN,
                KPP = account.KPP,
                OKPO = account.OKPO,
                OGRN = account.OGRN,
                Site = account.Site
            };
        }

        /// <summary>
        /// Метод обновляет данные клиента из кеша
        /// </summary>
        /// <param name="accountViewModel"></param>
        /// <param name="currentUser"></param>
        /// <param name="accountViewTypes"></param>
        /// <returns></returns>
        public AccountViewModel Refresh(AccountViewModel accountViewModel, User currentUser, params AccountViewType[] accountViewTypes)
        {
            accountViewTypes.ToList().ForEach(accountViewType =>
            {
                switch (accountViewType)
                {
                    // Восстановление данных поиска по контактам
                    case AccountViewType.ACC_CONTACTS:
                        AccountViewModel accContactsCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_CONTACTS);
                        accountViewModel.SearchContactFullName = accContactsCash.SearchContactFullName;
                        accountViewModel.SearchContactType = accContactsCash.SearchContactType;
                        accountViewModel.SearchContactPhoneNumber = accContactsCash.SearchContactPhoneNumber;
                        accountViewModel.SearchContactEmail = accContactsCash.SearchContactEmail;
                        accountViewModel.SearchContactPrimary = accContactsCash.SearchContactPrimary;
                        break;

                    // Восстановление данных поиска по адресам
                    case AccountViewType.ACC_ADDRESSES:
                        AccountViewModel accAddressesCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_ADDRESSES);
                        accountViewModel.SearchAddressCountry = accAddressesCash.SearchAddressCountry;
                        accountViewModel.SearchAddressRegion = accAddressesCash.SearchAddressRegion;
                        accountViewModel.SearchAddressCity = accAddressesCash.SearchAddressCity;
                        accountViewModel.SearchAddressStreet = accAddressesCash.SearchAddressStreet;
                        accountViewModel.SearchAddressHouse = accAddressesCash.SearchAddressHouse;
                        accountViewModel.SearchAddressType = accAddressesCash.SearchAddressType;
                        break;

                    // Восстановление данных поиска по банковским реквизитам
                    case AccountViewType.ACC_INVOICES:
                        AccountViewModel accInvoicesCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_INVOICES);
                        accountViewModel.SearchInvoiceBankName = accInvoicesCash.SearchInvoiceBankName;
                        accountViewModel.SearchInvoiceCity = accInvoicesCash.SearchInvoiceCity;
                        accountViewModel.SearchInvoiceCheckingAccount = accInvoicesCash.SearchInvoiceCheckingAccount;
                        accountViewModel.SearchInvoiceCorrespondentAccount = accInvoicesCash.SearchInvoiceCorrespondentAccount;
                        accountViewModel.SearchInvoiceBIC = accInvoicesCash.SearchInvoiceBIC;
                        accountViewModel.SearchInvoiceSWIFT = accInvoicesCash.SearchInvoiceSWIFT;
                        break;

                    // Восстановление данных поиска по сделкам
                    case AccountViewType.ACC_QUOTES:
                        AccountViewModel accQuotesCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_QUOTES);
                        break;

                    // Восстановление данных поиска по документам
                    case AccountViewType.ACC_DOCS:
                        AccountViewModel accDocumentsCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_DOCS);
                        break;

                    // Восстановление данных поиска по менеджерам
                    // На данный момент поиск по менеджерам отсутсвует
                    case AccountViewType.ACC_MANAGERS:
                        break;

                    // Восстановление данных поиска по списку всех сотрудников организации, создавшей клиента
                    case AccountViewType.ACC_TEAM_ALL_EMPLOYEES:
                        AccountViewModel accountAllManagersCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES);
                        break;

                    // Восстановление данных поиска для команды по клиенту
                    case AccountViewType.ACC_TEAM_SELECTED_EMPLOYEES:
                        AccountViewModel accountSelectedManagersCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_TEAM_SELECTED_EMPLOYEES);
                        break;

                    default:
                        break;
                }
            });
            return accountViewModel;
        }

        /// <summary>
        /// Метод разлочивает клиента, принимая на вход сотрудника, выбранного как основного менеджера по клиенту
        /// </summary>
        public void UnlockAccount(Employee newManager)
        {
            SetTransaction(OperationType.UnlockAccount);
            Account account = (Account)transaction.GetParameterValue("CurrentAccount");
            AccountManager accountManager = account.GetManagers(context).FirstOrDefault(i => i.ManagerId == newManager.Id);

            // Если произошло так, что у клиента был в списке этот менеджер, но не был основным, то его не надо добавлять.
            // В противном случае он добавляется в команду
            if (accountManager == null)
            {
                accountManager = new AccountManager()
                {
                    Id = Guid.NewGuid(),
                    Account = account,
                    AccountId = account.Id,
                    ManagerId = newManager.Id
                };
                transaction.AddChange(accountManager, EntityState.Added);
            }
            account.PrimaryManagerId = accountManager.Id;
            account.AccountStatus = AccountStatus.Active;
            transaction.AddChange(account, EntityState.Modified);
        }

        /// <summary>
        /// Метод создает новый контакт при создании клиента с типом "Физическое лицо"
        /// </summary>
        /// <param name="accountViewModel"></param>
        /// <param name="newAccountId">Id нового созданного клиента</param>
        /// <returns></returns>
        private AccountContact GetNewAccountContact(AccountViewModel accountViewModel, Guid newAccountId)
        {
            return new AccountContact()
            {
                Id = Guid.NewGuid(),
                AccountId = newAccountId,
                ContactType = ContactType.Work,
                FirstName = accountViewModel.FirstName,
                LastName = accountViewModel.LastName,
                MiddleName = accountViewModel.MiddleName
            };
        }

        /// <summary>
        /// Метод создает новый адрес при создании клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        /// <param name="newAccountId">Id нового созданного клиента</param>
        /// <returns></returns>
        private AccountAddress GetNewAccountAddress(AccountViewModel accountViewModel, Guid newAccountId)
        {
            return new AccountAddress()
            {
                Id = Guid.NewGuid(),
                AccountId = newAccountId,
                AddressType = AddressType.Legal,
                Country = accountViewModel.Country
            };
        }

        /// <summary>
        /// Метод создает нового менеджера у клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        /// <param name="newAccountId">Id нового созданного клиента</param>
        /// <returns></returns>
        private AccountManager GetNewAccountManager(Guid newAccountId)
        {
            Employee employee = (Employee)transaction.GetParameterValue("Manager");
            return new AccountManager()
            {
                Id = Guid.NewGuid(),
                AccountId = newAccountId,
                Manager = employee,
                ManagerId = employee.Id
            };
        }
    }
}
