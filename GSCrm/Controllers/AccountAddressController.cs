using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
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

        [HttpGet("{id}")]
        public IActionResult Address()
        {
            if (bool.TryParse(cachService.GetCachedItem(currentUser.Id, $"{PC}{ACC_ADDRESS}"), out bool isCorrectCheck) && isCorrectCheck)
            {
                AccountAddress accountAddress = cachService.GetCachedItem<AccountAddress>(currentUser.Id, "CurrentAccountAddressData");
                return Json(map.DataToViewModel(accountAddress));
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
    }
}
