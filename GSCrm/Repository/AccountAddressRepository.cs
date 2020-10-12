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

namespace GSCrm.Repository
{
    public class AccountAddressRepository : GenericRepository<AccountAddress, AccountAddressViewModel, AccountAddressValidatior, AccountAddressTransformer>
    {
        public AccountAddressRepository(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager)
            : base (context, viewsInfo, resManager, new AccountAddressValidatior(context, resManager), new AccountAddressTransformer(context, resManager))
        { }

        public AccountAddressRepository(ApplicationDbContext context, ResManager resManager, HttpContext httpContext = null)
            : base(context, resManager, new AccountAddressValidatior(context, resManager, httpContext?.GetCurrentUser(context)), new AccountAddressTransformer(context, resManager))
        { }

        #region Override Methods
        public override bool TryUpdatePrepare(AccountAddressViewModel addressViewModel, ModelStateDictionary modelState)
        {
            if (!base.TryUpdatePrepare(addressViewModel, modelState)) return false;
            if (!string.IsNullOrEmpty(addressViewModel.NewLegalAddressId) && !TryChangeLegalAddress(addressViewModel, modelState))
                return false;
            return true;
        }
        #endregion

        #region Other Methods
        /// <summary>
        /// Метод пытается изменить тип юридического адреса
        /// </summary>
        /// <param name="addressViewModel"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        private bool TryChangeLegalAddress(AccountAddressViewModel addressViewModel, ModelStateDictionary modelState)
        {
            addressViewModel.CurrentAddressNewType = addressViewModel.AddressType;
            AccountRepository accountRepository = new AccountRepository(context, resManager);
            if (accountRepository.TryGetItemById(addressViewModel.AccountId, modelState, out Account account))
            {
                // Валидация данных при изменении типа юридического адреса
                AccountValidatior accountValidatior = new AccountValidatior(context, resManager);
                Dictionary<string, string> errors = accountValidatior.ChangeLegalAddressCheck(addressViewModel, account);
                if (errors.Any())
                {
                    foreach (KeyValuePair<string, string> error in errors)
                        modelState.AddModelError(error.Key, error.Value);
                    return false;
                }
                
                // Изменение самого адреса
                if (TryGetItemById(addressViewModel.NewLegalAddressId, modelState, out AccountAddress newLegalAddress))
                {
                    addressViewModel.AddressType = addressViewModel.CurrentAddressNewType;
                    newLegalAddress.AddressType = AddressType.Legal;
                    context.AccountAddresses.Update(newLegalAddress);
                    return true;
                }
                return false;
            }
            return false;
        }
        #endregion
    }
}
