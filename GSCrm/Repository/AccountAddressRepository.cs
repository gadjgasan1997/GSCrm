﻿using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;
using GSCrm.Data;
using GSCrm.Transactions;
using GSCrm.Models.Enums;
using static GSCrm.Utils.AppUtils;
using static GSCrm.Utils.CollectionsUtils;

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
            => new AccountRepository(serviceProvider, context).CheckPermissionForAccountGroup("AccAddressCreate");

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
            => new AccountRepository(serviceProvider, context).CheckPermissionForAccountGroup("AccAddressUpdate");

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
            => new AccountRepository(serviceProvider, context).CheckPermissionForAccountGroup("AccAddressDelete");

        protected override void UpdateCacheOnDelete(AccountAddress accountAddress)
        {
            if (cachService.TryGetCachedEntity(currentUser, accountAddress.AccountId, out Account account) &&
                cachService.TryGetCachedEntity(currentUser, accountAddress.AccountId, out AccountViewModel accountViewModel))
            {
                cachService.CacheCurrentEntity(currentUser, account);
                cachService.CacheCurrentEntity(currentUser, accountViewModel);
            }
        }

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
                () => {
                    if (!string.IsNullOrEmpty(addressViewModel.Region) && addressViewModel.Region.Length > MAX_REGION_LENGTH)
                        errors.Add("RegionLength", resManager.GetString("RegionLength"));
                },
                () => {
                    if (!string.IsNullOrEmpty(addressViewModel.City) && addressViewModel.City.Length > MAX_CITY_LENGTH)
                        errors.Add("CityLength", resManager.GetString("CityLength"));
                },
                () => {
                    if (!string.IsNullOrEmpty(addressViewModel.Street) && addressViewModel.Street.Length > MAX_STREET_LENGTH)
                        errors.Add("StreetLength", resManager.GetString("StreetLength"));
                },
                () => {
                    if (!string.IsNullOrEmpty(addressViewModel.House) && addressViewModel.House.Length > MAX_HOUSE_LENGTH)
                        errors.Add("HouseLength", resManager.GetString("HouseLength"));
                }
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
        /// Метод проверяет тип адреса при создании
        /// </summary>
        /// <param name="addressViewModel"></param>
        private void CheckTypeOnCreate(AccountAddressViewModel addressViewModel)
        {
            if (TryCheckType(addressViewModel, out AddressType addressType))
            {
                // В случае, если тип создаваемого адреса является юридическим, и под клиентом уже есть юридический адрес, выводится ошибка уникальности
                Account account = cachService.GetCachedCurrentEntity<Account>(currentUser);
                if (addressType == AddressType.Legal && !TryCheckLegalAddressUnique(account))
                {
                    errors.Add("LegalAddressNotUnique", resManager.GetString("LegalAddressNotUnique"));
                    return;
                }
                transaction.AddParameter("AddressType", addressType);
            }
        }

        /// <summary>
        /// Метод проверяет тип адреса при обновлении
        /// </summary>
        /// <param name="addressViewModel"></param>
        private void CheckTypeOnUpdate(AccountAddressViewModel addressViewModel)
        {
            if (TryCheckType(addressViewModel, out AddressType addressType))
            {
                // В случае, если тип изменяемого адреса является юридическим, и под клиентом уже есть юридический адрес, выводится ошибка уникальности
                Account account = cachService.GetCachedCurrentEntity<Account>(currentUser);
                if (addressType == AddressType.Legal && !TryCheckLegalAddressUnique(account, addressViewModel.Id))
                {
                    errors.Add("LegalAddressNotUnique", resManager.GetString("LegalAddressNotUnique"));
                    return;
                }
                transaction.AddParameter("NewAddressType", addressType);
            }
        }

        /// <summary>
        /// Метод проверяет тип адреса
        /// </summary>
        /// <param name="addressViewModel"></param>
        private bool TryCheckType(AccountAddressViewModel addressViewModel, out AddressType addressType)
        {
            addressType = AddressType.None;
            if (!Enum.TryParse(typeof(AddressType), addressViewModel.AddressType, out object type))
            {
                errors.Add("AddressTypeWrong", resManager.GetString("AddressTypeWrong"));
                return false;
            }
            addressType = (AddressType)type;
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
                }
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
            viewModelsTF.Close(transaction, TransactionStatus.Error);
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
            transaction = viewModelsTF.Create(currentUser.Id, OperationType.ChangeAccountLegalAddress, addressViewModel);

            // Проверка полномочий
            if (!new AccountRepository(serviceProvider, context).CheckPermissionForAccountGroup("AccUpdate"))
                AddHasNoPermissionsError(OperationType.ChangeAccountLegalAddress);

            // Валидация
            else if (TryChangeLegalAddressValidate(addressViewModel))
            {
                // Изменение типа текущего юридического адреса
                Account account = cachService.GetCachedCurrentEntity<Account>(currentUser);
                AccountAddress oldLegalAddress = account.GetAddresses(context).FirstOrDefault(add => add.AddressType == AddressType.Legal);
                oldLegalAddress.AddressType = (AddressType)transaction.GetParameterValue("NewAddressType");
                transaction.AddChange(oldLegalAddress, EntityState.Modified);

                // Присвоение типа "Юридический" другому выбранному адресу
                AccountAddress newLegalAddress = (AccountAddress)transaction.GetParameterValue("NewLegalAddress");
                newLegalAddress.AddressType = AddressType.Legal;
                transaction.AddChange(newLegalAddress, EntityState.Modified);

                // Попытка закоммитить
                if (viewModelsTF.TryCommit(transaction, this.errors))
                {
                    viewModelsTF.Close(transaction);
                    return true;
                }
            }

            // Закрытие транзакции и выход
            viewModelsTF.Close(transaction, TransactionStatus.Error);
            errors = this.errors;
            return !errors.Any();
        }
        #endregion
    }
}
