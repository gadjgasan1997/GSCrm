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
    [Route(ACC_CONTACT)]
    public class AccountContactController
        : MainController<AccountContact, AccountContactViewModel, AccountContactValidatior, AccountContactTransformer, AccountContactRepository>
    {
        public AccountContactController(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager)
            : base(context, viewsInfo, resManager, new AccountContactTransformer(context, resManager), new AccountContactRepository(context, viewsInfo, resManager))
        { }

        [HttpGet("ListOfContacts/{pageNumber}")]
        public IActionResult Contacts(int pageNumber)
        {
            AccountViewModel accountViewModel = CurrentAccount;
            AccountRepository accountRepository = new AccountRepository(context, viewsInfo, resManager, HttpContext);
            User currentUser = context.Users.FirstOrDefault(n => n.UserName == User.Identity.Name);
            accountRepository.SetViewInfo(currentUser.Id, ACC_CONTACTS, pageNumber);
            accountRepository.AttachContacts(accountViewModel);
            return View($"{ACC_VIEWS_REL_PATH}{ACCOUNT}.cshtml", accountViewModel);
        }

        [HttpGet(CONTACT)]
        public IActionResult Contact(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid Id))
                return View("Error");

            AccountContact accountContact = context.AccountContacts.FirstOrDefault(i => i.Id == Id);
            if (accountContact == null)
                return View("Error");

            return Json(transformer.DataToViewModel(accountContact));
        }
    }
}
