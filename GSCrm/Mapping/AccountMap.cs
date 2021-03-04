using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Transactions;
using GSCrm.Models.Enums;

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
        /// Метод разлочивает клиента, принимая на вход сотрудника, выбранного как основного менеджера по клиенту
        /// </summary>
        public void UnlockAccount(Employee newManager)
        {
            SetTransaction(OperationType.UnlockAccount);
            Account account = cachService.GetCachedCurrentEntity<Account>(currentUser);
            AccountManager accountManager = account.GetAccTeam(context).FirstOrDefault(i => i.ManagerId == newManager.Id);

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
