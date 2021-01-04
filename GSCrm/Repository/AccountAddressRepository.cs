using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using static GSCrm.Utils.AppUtils;
using static GSCrm.Utils.CollectionsUtils;
using Microsoft.EntityFrameworkCore;
using GSCrm.Data;
using GSCrm.Transactions;
using GSCrm.Models.Enums;

namespace GSCrm.Repository
{
    public class AccountAddressRepository : BaseRepository<AccountAddress, AccountAddressViewModel>
    {
        #region Constructs
        public AccountAddressRepository(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base (serviceProvider, context)
        { }
        #endregion

        #region Override Methods
        protected override bool RespsIsCorrectOnCreate(AccountAddressViewModel addressViewModel)
            => new AccountRepository(serviceProvider, context).CheckPermissionForAccountGroup("AccAddressCreate", transaction);

        protected override bool TryCreatePrepare(AccountAddressViewModel addressViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CommonChecks(addressViewModel),
                () => CheckTypeOnCreate(addressViewModel)
            });
            return !errors.Any();
        }

        protected override bool RespsIsCorrectOnUpdate(AccountAddressViewModel addressViewModel)
            => new AccountRepository(serviceProvider, context).CheckPermissionForAccountGroup("AccAddressUpdate", transaction);

        protected override bool TryUpdatePrepare(AccountAddressViewModel addressViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CommonChecks(addressViewModel),
                () => CheckTypeOnUpdate(addressViewModel),
                () => {
                    if (!string.IsNullOrEmpty(addressViewModel.NewLegalAddressId))
                        TryChangeLegalAddressOnUpdate(addressViewModel);
                }
            });
            return !errors.Any();
        }

        protected override bool RespsIsCorrectOnDelete(AccountAddress accountAddress)
            => new AccountRepository(serviceProvider, context).CheckPermissionForAccountGroup("AccAddressDelete", transaction);

        protected override bool TryDeletePrepare(AccountAddress accountAddress)
        {
            if (accountAddress.AddressType == AddressType.Legal)
                errors.Add("PrimaryAddressIsReadonly", resManager.GetString("PrimaryAddressIsReadonly"));
            return !errors.Any();
        }
        #endregion

        #region Validations
        #region Declarations
        private readonly int MAX_REGION_LENGTH = 500;
        private readonly int MAX_CITY_LENGTH = 500;
        private readonly int MAX_STREET_LENGTH = 1000;
        private readonly int MAX_HOUSE_LENGTH = 100;
        #endregion

        /// <summary>
        /// Метод выполняет общие проверки для методов "CreationCheck" и "UpdateCheck"
        /// </summary>
        /// <param name="addressViewModel"></param>
        /// <returns></returns>
        private Dictionary<string, string> CommonChecks(AccountAddressViewModel addressViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CheckCountry(addressViewModel),
                () => CheckRegion(addressViewModel),
                () => CheckCity(addressViewModel),
                () => CheckStreet(addressViewModel),
                () => CheckHouse(addressViewModel),
            });
            return errors;
        }

        /// <summary>
        /// Метод проверяет страну
        /// </summary>
        /// <param name="addressViewModel"></param>
        private void CheckCountry(AccountAddressViewModel addressViewModel)
        {
            if (string.IsNullOrEmpty(addressViewModel.Country))
            {
                errors.Add("CountryIsRequired", resManager.GetString("CountryIsRequired"));
                return;
            }

            Func<JToken, bool> predicate = n => n.ToString().ToLower().TrimStartAndEnd() == addressViewModel.Country.ToLower().TrimStartAndEnd();
            JArray jArray = GetCountries(currentUser?.DefaultLanguage);
            if (jArray.Where(predicate).FirstOrDefault() == null)
                errors.Add("CountryWrong", resManager.GetString("CountryWrong"));
        }

        /// <summary>
        /// Метод проверяет регион
        /// </summary>
        /// <param name="addressViewModel"></param>
        private void CheckRegion(AccountAddressViewModel addressViewModel)
        {
            if (!string.IsNullOrEmpty(addressViewModel.Region) && addressViewModel.Region.Length > MAX_REGION_LENGTH)
                errors.Add("RegionLength", resManager.GetString("RegionLength"));
        }

        /// <summary>
        /// Метод проверяет город
        /// </summary>
        /// <param name="addressViewModel"></param>
        private void CheckCity(AccountAddressViewModel addressViewModel)
        {
            if (!string.IsNullOrEmpty(addressViewModel.City) && addressViewModel.City.Length > MAX_CITY_LENGTH)
                errors.Add("CityLength", resManager.GetString("CityLength"));
        }

        /// <summary>
        /// Метод проверяет улицу
        /// </summary>
        /// <param name="addressViewModel"></param>
        private void CheckStreet(AccountAddressViewModel addressViewModel)
        {
            if (!string.IsNullOrEmpty(addressViewModel.Street) && addressViewModel.Street.Length > MAX_STREET_LENGTH)
                errors.Add("StreetLength", resManager.GetString("StreetLength"));
        }

        /// <summary>
        /// Метод проверяет дом
        /// </summary>
        /// <param name="addressViewModel"></param>
        private void CheckHouse(AccountAddressViewModel addressViewModel)
        {
            if (!string.IsNullOrEmpty(addressViewModel.House) && addressViewModel.House.Length > MAX_HOUSE_LENGTH)
                errors.Add("HouseLength", resManager.GetString("HouseLength"));
        }

        /// <summary>
        /// Метод проверяет тип адреса при создании
        /// </summary>
        /// <param name="addressViewModel"></param>
        private void CheckTypeOnCreate(AccountAddressViewModel addressViewModel)
        {
            if (!TryCheckType(addressViewModel, out Account account, out AddressType? addressType)) return;
            if (addressType == AddressType.Legal && !TryCheckLegalAddressUnique(account))
            {
                errors.Add("LegalAddressNotUnique", resManager.GetString("LegalAddressNotUnique"));
                return;
            }
            transaction.AddParameter("AccountAddress", new AccountAddress()
            {
                Account = account,
                AddressType = (AddressType)addressType
            });
        }

        /// <summary>
        /// Метод проверяет тип адреса при обновлении
        /// </summary>
        /// <param name="addressViewModel"></param>
        private void CheckTypeOnUpdate(AccountAddressViewModel addressViewModel)
        {
            if (!TryCheckType(addressViewModel, out Account account, out AddressType? addressType)) return;

            // В случае, если тип изменяемого адреса является юридическим, и под клиентом уже есть юридический адрес, выводится ошибка уникальности
            if (addressType == AddressType.Legal && !TryCheckLegalAddressUnique(account, addressViewModel.Id))
            {
                errors.Add("LegalAddressNotUnique", resManager.GetString("LegalAddressNotUnique"));
                return;
            }
            transaction.AddParameter("AccountAddress", new AccountAddress()
            {
                Account = account,
                AddressType = (AddressType)addressType
            });
        }

        /// <summary>
        /// Метод проверяет тип адреса, и, в случае успеха, возвращает клиента, к которому относится адрес и сам тип адреса, преобразуя его из типа "string" в тип "AddressType"
        /// </summary>
        /// <param name="addressViewModel"></param>
        /// <param name="account"></param>
        /// <param name="addressType"></param>
        private bool TryCheckType(AccountAddressViewModel addressViewModel, out Account account, out AddressType? addressType)
        {
            account = null;
            addressType = null;
            if (string.IsNullOrEmpty(addressViewModel.AccountId) || !Guid.TryParse(addressViewModel.AccountId, out Guid accountId))
            {
                errors.Add("AccountNotFound", resManager.GetString("AccountNotFound"));
                return false;
            }

            account = context.Accounts.Include(addr => addr.AccountAddresses).FirstOrDefault(i => i.Id == accountId);
            if (account == null)
            {
                errors.Add("AccountNotFound", resManager.GetString("AccountNotFound"));
                return false;
            }

            if (!TryGetAddressType(addressViewModel.AddressType, ref addressType))
            {
                errors.Add("AddressTypeWrong", resManager.GetString("AddressTypeWrong"));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Метод пытается получить тип адреса из его строкого представления
        /// </summary>
        /// <param name="addressTypeString"></param>
        /// <param name="addressTypeEnum"></param>
        /// <returns></returns>
        private bool TryGetAddressType(string addressTypeString, ref AddressType? addressTypeEnum)
        {
            if (!Enum.TryParse(typeof(AddressType), addressTypeString, out object addressType))
                return false;
            addressTypeEnum = (AddressType)addressType;
            return true;
        }

        /// <summary>
        /// Метод проверяет, что под клиентом не существует двух адресов с типом "Юридический" при создании адреса
        /// Если это не так, возвращает false
        /// </summary>
        /// <param name="account"></param>
        private bool TryCheckLegalAddressUnique(Account account)
            => account.GetAddresses(context).Where(addr => addr.AddressType == AddressType.Legal).Count() == 0;

        /// <summary>
        /// Метод проверяет, что под клиентом не существует двух адресов с типом "Юридический" при обновлении адреса
        /// Если это не так, возвращает false
        /// </summary>
        /// <param name="account"></param>
        private bool TryCheckLegalAddressUnique(Account account, Guid currentAddressId)
            => account.GetAddresses(context).Where(addr => addr.AddressType == AddressType.Legal && addr.Id != currentAddressId).Count() == 0;

        /// <summary>
        /// Метод производит проверки при изменении юридического адреса клиента
        /// </summary>
        /// <param name="addressViewModel"></param>
        /// <returns></returns>
        public bool TryChangeLegalAddressValidate(AccountAddressViewModel addressViewModel)
        {
            AddressType newAddressType = default;
            Guid newLegalAddressId = default;
            InvokeIntermittinActions(errors, new List<Action>()
            {
                // Поиск клиента
                () => {
                    if (!new AccountRepository(serviceProvider, context).TryGetItemById(addressViewModel.AccountId, out Account account))
                        errors.Add("RecordNotFound", resManager.GetString("RecordNotFound"));
                    else transaction.AddParameter("Account", account);
                },
                // Попытка распарсить новый тип изменяемого адреса
                () => {
                    if (!Enum.TryParse(typeof(AddressType), addressViewModel.CurrentAddressNewType, out object type))
                        errors.Add("AddressTypeWrong", resManager.GetString("AddressTypeWrong"));
                    else newAddressType = (AddressType)type;
                },
                // Сравнение нового выбранного типа адреса с юридическим. Если совпадает - ошибка
                () => {
                    if (newAddressType == AddressType.Legal)
                        errors.Add("AddressTypeWrong", resManager.GetString("AddressTypeWrong"));
                    else transaction.AddParameter("NewAddressType", newAddressType);
                },
                // Попытка распарсить id нового выбранного юридического адреса
                () => {
                    if (!Guid.TryParse(addressViewModel.NewLegalAddressId, out Guid guid))
                        errors.Add("AddressNotFound", resManager.GetString("AddressNotFound"));
                    else newLegalAddressId = guid;
                },
                // Попытка найти новый юридический адрес
                () => {
                    AccountAddress newLegalAddress = context.AccountAddresses.AsNoTracking().FirstOrDefault(i => i.Id == newLegalAddressId);
                    if (newLegalAddress == null)
                        errors.Add("AddressNotFound", resManager.GetString("AddressNotFound"));
                    else transaction.AddParameter("NewLegalAddress", newLegalAddress);
                },
            });
            return !errors.Any();
        }
        #endregion

        #region Other Methods
        /// <summary>
        /// Метод пытается изменить тип юридического адреса при обновлении записи
        /// </summary>
        /// <param name="addressViewModel"></param>
        /// <returns></returns>
        public bool TryChangeLegalAddressOnUpdate(AccountAddressViewModel addressViewModel)
        {
            if (TryChangeLegalAddressValidate(addressViewModel))
            {
                addressViewModel.CurrentAddressNewType = addressViewModel.AddressType;
                addressViewModel.AddressType = addressViewModel.CurrentAddressNewType;
                AccountAddress newLegalAddress = (AccountAddress)transaction.GetParameterValue("NewLegalAddress");
                newLegalAddress.AddressType = AddressType.Legal;
                transaction.AddChange(newLegalAddress, EntityState.Modified);
                return true;
            }
            transactionFactory.Close(transaction, TransactionStatus.Error);
            return false;
        }

        /// <summary>
        /// Метод пытается изменить тип юридического адреса
        /// </summary>
        /// <param name="addressViewModel"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public bool TryChangeLegalAddress(AccountAddressViewModel addressViewModel, out Dictionary<string, string> errors)
        {
            errors = this.errors;
            transaction = transactionFactory.Create(currentUser.Id, OperationType.ChangeAccountLegalAddress, addressViewModel);

            // Проверка полномочий
            if (!new AccountRepository(serviceProvider, context).CheckPermissionForAccountGroup("AccUpdate", transaction))
                AddHasNoPermissionsError(OperationType.ChangeAccountLegalAddress);

            // Валидация
            else if (TryChangeLegalAddressValidate(addressViewModel))
            {
                // Изменение типа текущего юридического адреса
                Account account = (Account)transaction.GetParameterValue("Account");
                AccountAddress oldLegalAddress = account.GetAddresses(context).FirstOrDefault(add => add.AddressType == AddressType.Legal);
                oldLegalAddress.AddressType = (AddressType)transaction.GetParameterValue("NewAddressType");
                transaction.AddChange(oldLegalAddress, EntityState.Modified);

                // Присвоение типа "Юридический" другому выбранному адресу
                AccountAddress newLegalAddress = (AccountAddress)transaction.GetParameterValue("NewLegalAddress");
                newLegalAddress.AddressType = AddressType.Legal;
                transaction.AddChange(newLegalAddress, EntityState.Modified);

                // Попытка закоммитить
                if (transactionFactory.TryCommit(transaction, this.errors))
                {
                    transactionFactory.Close(transaction);
                    return true;
                }
            }

            // Закрытие транзакции и выход
            transactionFactory.Close(transaction, TransactionStatus.Error);
            errors = this.errors;
            return !errors.Any();
        }
        #endregion
    }
}
