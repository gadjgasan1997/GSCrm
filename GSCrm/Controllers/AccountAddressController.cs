using GSCrm.Data;
using GSCrm.Mapping;
using GSCrm.Models;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using GSCrm.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using static GSCrm.CommonConsts;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(ACC_ADDRESS)]
    public class AccountAddressController
        : MainController<AccountAddress, AccountAddressViewModel>
    {
        public AccountAddressController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }

        [HttpGet("ListOfAddresses/{pageNumber}")]
        public IActionResult Addresses(int pageNumber)
        {
            AccountViewModel accountViewModel = (AccountViewModel)cachService.GetMainEntity(currentUser, MainEntityType.AccountView);
            AccountRepository accountRepository = new AccountRepository(serviceProvider, context);
            accountRepository.SetViewInfo(currentUser.Id, ACC_ADDRESSES, pageNumber);
            accountRepository.AttachAddresses(accountViewModel);
            return View($"{ACC_VIEWS_REL_PATH}{ACCOUNT}.cshtml", accountViewModel);
        }

        [HttpGet(ADDRESS)]
        public IActionResult Address(string id)
        {
            if (!repository.TryGetItemById(id, out AccountAddress accountAddress))
                return View("Error");
            return Json(map.DataToViewModel(accountAddress));
        }

        [HttpPost("ChangeLegalAddress")]
        public IActionResult ChangeLegalAddress(AccountAddressViewModel addressViewModel)
        {
            ModelStateDictionary modelState = ModelState;
            if (!new AccountAddressRepository(serviceProvider, context).TryChangeLegalAddress(addressViewModel, out Dictionary<string, string> errors))
            {
                foreach (KeyValuePair<string, string> error in errors)
                    modelState.AddModelError(error.Key, error.Value);
                return BadRequest(modelState);
            }
            return Json("");
        }
    }
}
