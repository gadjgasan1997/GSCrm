using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using static GSCrm.CommonConsts;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(ACC_INVOICE)]
    public class AccountInvoiceController
        : MainController<AccountInvoice, AccountInvoiceViewModel>
    {
        public AccountInvoiceController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }

        [HttpGet("{id}")]
        public IActionResult Invoice()
        {
            if (bool.TryParse(cachService.GetCachedItem(currentUser.Id, $"{PC}{ACC_INVOICE}"), out bool isCorrectCheck) && isCorrectCheck)
            {
                AccountInvoice accountInvoice = cachService.GetCachedItem<AccountInvoice>(currentUser.Id, "CurrentAccountInvoiceData");
                return Json(map.DataToViewModel(accountInvoice));
            }
            return Json("");
        }
    }
}
