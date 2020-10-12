using GSCrm.Data;
using GSCrm.Data.ApplicationInfo;
using GSCrm.DataTransformers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using GSCrm.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using static GSCrm.CommonConsts;
using static GSCrm.Repository.AccountRepository;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(ACC_ADDRESS)]
    public class AccountAddressController
        : MainController<AccountAddress, AccountAddressViewModel, AccountAddressValidatior, AccountAddressTransformer, AccountAddressRepository>
    {
        public AccountAddressController(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager)
            : base(context, viewsInfo, resManager, new AccountAddressTransformer(context, resManager), new AccountAddressRepository(context, viewsInfo, resManager))
        { }

        [HttpGet("ListOfAddresses/{pageNumber}")]
        public IActionResult Addresses(int pageNumber)
        {
            AccountViewModel accountViewModel = CurrentAccount;
            AccountRepository accountRepository = new AccountRepository(context, viewsInfo, resManager, HttpContext);
            User currentUser = context.Users.FirstOrDefault(n => n.UserName == User.Identity.Name);
            accountRepository.SetViewInfo(currentUser.Id, ACC_ADDRESSES, pageNumber);
            accountRepository.AttachAddresses(accountViewModel);
            return View($"{ACC_VIEWS_REL_PATH}{ACCOUNT}.cshtml", accountViewModel);
        }

        [HttpGet(ADDRESS)]
        public IActionResult Address(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid Id))
                return View("Error");

            AccountAddress accountAddress = context.AccountAddresses.FirstOrDefault(i => i.Id == Id);
            if (accountAddress == null)
                return View("Error");

            return Json(transformer.DataToViewModel(accountAddress));
        }

        [HttpPost("Create")]
        public override IActionResult Create(AccountAddressViewModel addressViewModel)
        {
            ModelStateDictionary modelState = ModelState;
            AccountAddressRepository addressRepository = new AccountAddressRepository(context, resManager, HttpContext);
            if (addressRepository.TryCreate(ref addressViewModel, modelState))
                return Json("");
            return BadRequest(modelState);
        }

        [HttpPost("Update")]
        public override IActionResult Update(AccountAddressViewModel addressViewModel)
        {
            ModelStateDictionary modelState = ModelState;
            AccountAddressRepository addressRepository = new AccountAddressRepository(context, resManager, HttpContext);
            if (addressRepository.TryUpdate(ref addressViewModel, modelState))
                return Json("");
            return BadRequest(modelState);
        }
    }
}
