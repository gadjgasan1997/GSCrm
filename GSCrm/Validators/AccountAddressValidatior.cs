using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GSCrm.Utils.AppUtils;
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Validators
{
    public class AccountAddressValidatior : BaseValidator<AccountAddressViewModel>
    {
        private readonly int MAX_REGION_LENGTH = 500;
        private readonly int MAX_CITY_LENGTH = 500;
        private readonly int MAX_STREET_LENGTH = 1000;
        private readonly int MAX_HOUSE_LENGTH = 100;
        private readonly User currentUser;
        public AccountAddressValidatior(ApplicationDbContext context, ResManager resManager, User currentUser = null) : base(context, resManager)
        {
            this.currentUser = currentUser;
        }

        public override Dictionary<string, string> CreationCheck(AccountAddressViewModel addressViewModel)
        {
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CommonChecks(addressViewModel),
                () => CheckTypeOnCreate(addressViewModel)
            });
            return errors;
        }

        public override Dictionary<string, string> UpdateCheck(AccountAddressViewModel addressViewModel)
        {
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CommonChecks(addressViewModel),
                () => CheckTypeOnUpdate(addressViewModel)
            });
            return errors;
        }

        public override Dictionary<string, string> DeleteCheck(Guid id)
        {
            AccountAddress accountAddress = context.AccountAddresses.FirstOrDefault(i => i.Id == id);
            if (accountAddress == null)
            {
                errors.Add("AccountAddressNotFound", resManager.GetString("AccountAddressNotFound"));
                return errors;
            }

            if (accountAddress.AddressType == AddressType.Legal)
                errors.Add("PrimaryAddressIsReadonly", resManager.GetString("PrimaryAddressIsReadonly"));
            return errors;
        }

        /// <summary>
        /// Метод выполняет общие проверки для методов "CreationCheck" и "UpdateCheck"
        /// </summary>
        /// <param name="addressViewModel"></param>
        /// <returns></returns>
        private Dictionary<string, string> CommonChecks(AccountAddressViewModel addressViewModel)
        {
            InvokeIntermittingChecks(errors, new List<Action>()
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
        /// Метод проверяет тип адреса при создании
        /// </summary>
        /// <param name="addressViewModel"></param>
        private void CheckTypeOnCreate(AccountAddressViewModel addressViewModel)
        {
            if (!TryCheckType(addressViewModel, out Account account, out AddressType? addressType)) return;

            if (addressType == AddressType.Legal && !TryCheckLegalAddressUnique(account))
                errors.Add("LegalAddressNotUnique", resManager.GetString("LegalAddressNotUnique"));
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
                errors.Add("LegalAddressNotUnique", resManager.GetString("LegalAddressNotUnique"));
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
        /// <param name="addressType"></param>
        private bool TryCheckLegalAddressUnique(Account account)
        {
            if (account.GetAddresses(context).Where(addr => addr.AddressType == AddressType.Legal).Count() > 0)
                return false;
            return true;
        }

        /// <summary>
        /// Метод проверяет, что под клиентом не существует двух адресов с типом "Юридический" при обновлении адреса
        /// Если это не так, возвращает false
        /// </summary>
        /// <param name="account"></param>
        /// <param name="addressType"></param>
        private bool TryCheckLegalAddressUnique(Account account, Guid currentAddressId)
        {
            if (account.GetAddresses(context).Where(addr => addr.AddressType == AddressType.Legal && addr.Id != currentAddressId).Count() > 0)
                return false;
            return true;
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
    }
}
