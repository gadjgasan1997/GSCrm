using System;
using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using static GSCrm.CommonConsts;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(ACCOUNT)]
    public class AccountController : MainController<Account, AccountViewModel>
    {
        public AccountController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }

        [HttpGet("BackToAccounts")]
        public IActionResult BackToAccounts()
        {
            if (cachService.TryGetValue(currentUser, "SelectedAccountsTab", out object itemValue))
            {
                // Возврат назад в зависимости от того, на какой вкладке находился пользователь перед проваливанием в карточку
                return itemValue.ToString() switch
                {
                    // Проваливание со списка всех клиентов
                    ALL_ACCS => RedirectToAction(ALL_ACCS, "Root", new { pageNumber = cachService.GetViewInfo(currentUser.Id, ALL_ACCS)?.CurrentPageNumber }),

                    // Проваливание со списка клиентов основной организации текущего пользователя
                    _ => RedirectToAction(CURRENT_ACCS, "Root", new { pageNumber = cachService.GetViewInfo(currentUser.Id, CURRENT_ACCS)?.CurrentPageNumber }),
                };
            }
            return View("Error");
        }

        [HttpGet("{id}")]
        public IActionResult Account(string id) => GetAccount(id);

        #region Child Entities
        /// <summary>
        /// Получение списка контактов клиента
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("{id}/Contacts/{pageNumber}")]
        public IActionResult Contacts(string id, int pageNumber)
        {
            repository.SetViewInfo(id, ACC_CONTACTS, pageNumber);
            return GetAccount(id);
        }

        /// <summary>
        /// Получение списка адресов клиента
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("{id}/Addresses/{pageNumber}")]
        public IActionResult Addresses(string id, int pageNumber)
        {
            repository.SetViewInfo(id, ACC_ADDRESSES, pageNumber);
            return GetAccount(id);
        }

        /// <summary>
        /// Получение списка банковских реквизитов клиента
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("{id}/Invoices/{pageNumber}")]
        public IActionResult Invoices(string id, int pageNumber)
        {
            repository.SetViewInfo(id, ACC_INVOICES, pageNumber);
            return GetAccount(id);
        }
        #endregion

        #region Actions
        [HttpGet("/GoToAccount/{id}")]
        public IActionResult GoToAccount(string id, string selectedAccountsTab)
        {
            cachService.AddOrUpdate(currentUser, "SelectedAccountsTab", selectedAccountsTab);
            return RedirectToAction(ACCOUNT, ACCOUNT, new { id });
        }

        [HttpGet("{id}/HasAccNotLegalAddress")]
        public IActionResult HasAccNotLegalAddress()
        {
            Account account = cachService.GetCachedCurrentEntity<Account>(currentUser);
            return Json(account.GetAddresses(context).Count > 1);
        }

        [HttpGet("{id}/ChangeSite/{newSite?}")]
        public IActionResult ChangeSite(string id, string newSite = null)
        {
            ModelStateDictionary modelState = ModelState;
            Account account = cachService.GetCachedCurrentEntity<Account>(currentUser);
            if (!new AccountRepository(serviceProvider, context).TryChangeSite(account, out Dictionary<string, string> errors, newSite))
            {
                AddErrorsToModel(modelState, errors);
                return BadRequest(modelState);
            }
            return Json("");
        }

        [HttpPost("ChangePrimaryContact")]
        public IActionResult ChangePrimaryContact(AccountViewModel accountViewModel)
        {
            ModelStateDictionary modelState = ModelState;
            if (!new AccountRepository(serviceProvider, context).TryChangePrimaryContact(accountViewModel, out Dictionary<string, string> errors))
            {
                AddErrorsToModel(modelState, errors);
                return BadRequest(modelState);
            }
            return Json("");
        }

        [HttpPost("ChangeLegalAddress")]
        public IActionResult ChangeLegalAddress(AccountAddressViewModel addressViewModel)
        {
            ModelStateDictionary modelState = ModelState;
            if (!new AccountAddressRepository(serviceProvider, context).TryChangeLegalAddress(addressViewModel, out Dictionary<string, string> errors))
            {
                AddErrorsToModel(modelState, errors);
                return BadRequest(modelState);
            }
            return Json("");
        }

        [HttpPost("UnlockAccount")]
        public IActionResult UnlockAccount(AccountViewModel accountViewModel)
        {
            ModelStateDictionary modelState = ModelState;
            if (!new AccountRepository(serviceProvider, context).TryUnlockAccount(accountViewModel, out Dictionary<string, string> errors))
            {
                AddErrorsToModel(modelState, errors);
                return BadRequest(modelState);
            }
            return Json("");
        }
        #endregion

        #region Search
        [HttpPost("SearchContact")]
        public IActionResult SearchContact(AccountViewModel accountViewModel)
        {
            new AccountRepository(serviceProvider, context).SearchContact(accountViewModel);
            return base.RedirectToAction(ACCOUNT, ACCOUNT, new { id = accountViewModel.Id });
        }

        [HttpGet("{id}/ClearContactSearch")]
        public IActionResult ClearContactSearch(string id)
        {
            new AccountRepository(serviceProvider, context).ClearContactSearch();
            return base.RedirectToAction(ACCOUNT, ACCOUNT, new { id });
        }

        [HttpPost("SearchAddress")]
        public IActionResult SearchAddress(AccountViewModel accountViewModel)
        {
            new AccountRepository(serviceProvider, context).SearchAddress(accountViewModel);
            return base.RedirectToAction(ACCOUNT, ACCOUNT, new { id = accountViewModel.Id });
        }

        [HttpGet("{id}/ClearAddressSearch")]
        public IActionResult ClearAddressSearch(string id)
        {
            new AccountRepository(serviceProvider, context).ClearAddressSearch();
            return base.RedirectToAction(ACCOUNT, ACCOUNT, new { id });
        }

        [HttpPost("SearchInvoice")]
        public IActionResult SearchInvoice(AccountViewModel accountViewModel)
        {
            new AccountRepository(serviceProvider, context).SearchInvoice(accountViewModel);
            return base.RedirectToAction(ACCOUNT, ACCOUNT, new { id = accountViewModel.Id });
        }

        [HttpGet("{id}/ClearInvoiceSearch")]
        public IActionResult ClearInvoiceSearch(string id)
        {
            new AccountRepository(serviceProvider, context).ClearInvoicesSearch();
            return base.RedirectToAction(ACCOUNT, ACCOUNT, new { id });
        }
        #endregion

        #region Addition Methods
        /// <summary>
        /// Метод загружает и возвращает клиента
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        private IActionResult GetAccount(string accountId)
        {
            if (cachService.TryGetCachedEntity(currentUser, accountId, out Account account))
                return View(ACCOUNT, repository.LoadView(account));
            return View("Error");
        }
        #endregion
    }
}
