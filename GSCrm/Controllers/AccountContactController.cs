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
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(ACC_CONTACT)]
    public class AccountContactController
        : MainController<AccountContact, AccountContactViewModel>
    {
        public AccountContactController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }

        [HttpGet("ListOfContacts/{pageNumber}")]
        public IActionResult Contacts(int pageNumber)
        {
            AccountViewModel accountViewModel = (AccountViewModel)cachService.GetMainEntity(currentUser, MainEntityType.AccountView);
            AccountRepository accountRepository = new AccountRepository(serviceProvider, context);
            accountRepository.SetViewInfo(currentUser.Id, ACC_CONTACTS, pageNumber);
            accountRepository.AttachContacts(accountViewModel);
            return View($"{ACC_VIEWS_REL_PATH}{ACCOUNT}.cshtml", accountViewModel);
        }

        [HttpGet(CONTACT)]
        public IActionResult Contact(string id)
        {
            if (!repository.TryGetItemById(id, out AccountContact accountContact))
                return View("Error");
            return Json(map.DataToViewModel(accountContact));
        }
    }
}
