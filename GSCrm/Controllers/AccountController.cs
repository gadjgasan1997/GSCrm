using GSCrm.Mapping;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using GSCrm.Data;
using GSCrm.Models.Enums;
using static GSCrm.CommonConsts;
using static GSCrm.Repository.AccountRepository;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(ACCOUNT)]
    public class AccountController
        : MainController<Account, AccountViewModel>
    {
        public AccountController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }

        [HttpGet("ListOfAllAccounts/{pageNumber}")]
        public ViewResult AllAccounts(int pageNumber)
        {
            AccountRepository accountRepository = new AccountRepository(serviceProvider, context);
            AccountsViewModel accountsViewModel = new AccountsViewModel();
            new AccountMap(serviceProvider, context).InitializeAccountsViewModel(accountsViewModel);
            accountRepository.SetViewInfo(ALL_ACCS, pageNumber);
            accountRepository.AttachAccounts(ref accountsViewModel);
            return View(ACCOUNTS, accountsViewModel);
        }

        [HttpGet("ListOfCurrentAccounts/{pageNumber}")]
        public ViewResult CurrentAccounts(int pageNumber)
        {
            AccountRepository accountRepository = new AccountRepository(serviceProvider, context);
            AccountsViewModel accountsViewModel = new AccountsViewModel();
            new AccountMap(serviceProvider, context).InitializeAccountsViewModel(accountsViewModel);
            accountRepository.SetViewInfo(CURRENT_ACCS, pageNumber);
            accountRepository.AttachAccounts(ref accountsViewModel);
            return View(ACCOUNTS, accountsViewModel);
        }

        [HttpGet("BackToAccounts")]
        public IActionResult BackToAccounts()
            // Возврат назад в зависимости от того, на какой вкладке находился пользователь перед проваливанием в карточку
            => cachService.GetCachedItem(currentUser.Id, "SelectedAccountsTab") switch
                {
                    // Проваливание со списка всех клиентов
                    ALL_ACCS => RedirectToAction(ALL_ACCS, ACCOUNT, new { pageNumber = viewsInfo.Get(currentUser.Id, ALL_ACCS)?.CurrentPageNumber }),

                    // Проваливание со списка клиентов основной организации текущего пользователя
                    _ => RedirectToAction(CURRENT_ACCS, ACCOUNT, new { pageNumber = viewsInfo.Get(currentUser.Id, CURRENT_ACCS)?.CurrentPageNumber }),
                };
        

        [HttpGet("{id}")]
        public ViewResult Account(string id)
        {
            AccountRepository accountRepository = new AccountRepository(serviceProvider, context);
            if (!accountRepository.TryGetItemById(id, out Account account))
                return View($"{ACC_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new AccountViewModel());
            if (!accountRepository.HasPermissionsForSeeItem(account))
                return View($"{ACC_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new AccountViewModel());

            AccountMap map = new AccountMap(serviceProvider, context);
            AccountViewModel accountViewModel = map.DataToViewModel(account);
            accountViewModel = map.Refresh(accountViewModel, currentUser, AccAllViewTypes);
            accountRepository.AttachContacts(accountViewModel);
            accountRepository.AttachAddresses(accountViewModel);
            accountRepository.AttachInvoices(accountViewModel);
            accountRepository.AttachQuotes(accountViewModel);
            accountRepository.AttachManagers(accountViewModel);
            cachService.CacheItem(currentUser.Id, "CurrentAccountData", account);
            cachService.CacheItem(currentUser.Id, "CurrentAccountView", accountViewModel);
            return View(ACCOUNT, accountViewModel);
        }

        [HttpGet("/GetAccount/{id}")]
        public IActionResult GetAccount(string id, string selectedAccountsTab)
        {
            cachService.CacheItem(currentUser.Id, "SelectedAccountsTab", selectedAccountsTab);
            return RedirectToAction(ACCOUNT, ACCOUNT, new { id });
        }

        [HttpGet("HasAccNotLegalAddress/{id}")]
        public IActionResult HasAccNotLegalAddress(string id)
        {
            if (!repository.TryGetItemById(id, out Account account))
                return Json(false);
            return Json(account.GetAddresses(context).Count > 1);
        }

        [HttpGet("ChangeSite/{accountId}/{newSite?}")]
        public IActionResult ChangeSite(string accountId, string newSite = null)
        {
            ModelStateDictionary modelState = ModelState;
            if (!new AccountRepository(serviceProvider, context).TryChangeSite(accountId, out Dictionary<string, string> errors, newSite))
            {
                foreach (KeyValuePair<string, string> error in errors)
                    modelState.AddModelError(error.Key, error.Value);
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
                foreach (KeyValuePair<string, string> error in errors)
                    modelState.AddModelError(error.Key, error.Value);
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
                foreach (KeyValuePair<string, string> error in errors)
                    modelState.AddModelError(error.Key, error.Value);
                return BadRequest(modelState);
            }
            return Json("");
        }

        [HttpPost("SearchAllAccounts")]
        public IActionResult SearchAllAccounts(AccountsViewModel accountsViewModel)
        {
            cachService.CacheItem(currentUser.Id, ALL_ACCS, accountsViewModel);
            return RedirectToAction(ALL_ACCS, ACCOUNT, new { pageNumber = DEFAULT_MIN_PAGE_NUMBER });
        }

        [HttpGet("ClearAllAccountsSearch")]
        public IActionResult ClearAllAccountsSearch()
        {
            new AccountRepository(serviceProvider, context).ClearAllAccountsSearch();
            return RedirectToAction(ALL_ACCS, ACCOUNT, new { pageNumber = DEFAULT_MIN_PAGE_NUMBER });
        }

        [HttpPost("SearchCurrentAccounts")]
        public IActionResult SearchCurrentAccounts(AccountsViewModel accountsViewModel)
        {
            cachService.CacheItem(currentUser.Id, CURRENT_ACCS, accountsViewModel);
            return RedirectToAction(CURRENT_ACCS, ACCOUNT, new { pageNumber = DEFAULT_MIN_PAGE_NUMBER });
        }

        [HttpGet("ClearCurrentAccountsSearch")]
        public IActionResult ClearCurrentAccountsSearch()
        {
            new AccountRepository(serviceProvider, context).ClearCurrentAccountsSearch();
            return RedirectToAction(CURRENT_ACCS, ACCOUNT, new { pageNumber = DEFAULT_MIN_PAGE_NUMBER });
        }

        [HttpPost("SearchContact")]
        public IActionResult SearchContact(AccountViewModel accountViewModel)
        {
            cachService.CacheItem(currentUser.Id, ACC_CONTACTS, accountViewModel);
            return base.RedirectToAction(ACCOUNT, ACCOUNT, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.AccountView) });
        }

        [HttpGet("ClearContactSearch")]
        public IActionResult ClearContactSearch()
        {
            new AccountRepository(serviceProvider, context).ClearContactSearch();
            return base.RedirectToAction(ACCOUNT, ACCOUNT, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.AccountView) });
        }

        [HttpPost("SearchAddress")]
        public IActionResult SearchAddress(AccountViewModel accountViewModel)
        {
            cachService.CacheItem(currentUser.Id, ACC_ADDRESSES, accountViewModel);
            return base.RedirectToAction(ACCOUNT, ACCOUNT, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.AccountView) });
        }

        [HttpGet("ClearAddressSearch")]
        public IActionResult ClearAddressSearch()
        {
            new AccountRepository(serviceProvider, context).ClearAddressSearch();
            return base.RedirectToAction(ACCOUNT, ACCOUNT, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.AccountView) });
        }

        [HttpPost("SearchInvoice")]
        public IActionResult SearchInvoice(AccountViewModel accountViewModel)
        {
            cachService.CacheItem(currentUser.Id, ACC_INVOICES, accountViewModel);
            return base.RedirectToAction(ACCOUNT, ACCOUNT, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.AccountView) });
        }

        [HttpGet("ClearInvoiceSearch")]
        public IActionResult ClearInvoiceSearch()
        {
            new AccountRepository(serviceProvider, context).ClearInvoiceSearch();
            return base.RedirectToAction(ACCOUNT, ACCOUNT, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.AccountView) });
        }

        [HttpPost("SearchQuote")]
        public IActionResult SearchQuote(AccountViewModel accountViewModel)
        {
            cachService.CacheItem(currentUser.Id, ACC_QUOTES, accountViewModel);
            return base.RedirectToAction(ACCOUNT, ACCOUNT, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.AccountView) });
        }

        [HttpGet("ClearQuoteSearch")]
        public IActionResult ClearQuoteSearch()
        {
            new AccountRepository(serviceProvider, context).ClearQuoteSearch();
            return base.RedirectToAction(ACCOUNT, ACCOUNT, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.AccountView) });
        }
    }
}
