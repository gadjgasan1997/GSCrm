using GSCrm.Data;
using GSCrm.Mapping;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using GSCrm.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
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

        [HttpGet(INVOICE)]
        public IActionResult Invoice(string id)
        {
            if (!repository.TryGetItemById(id, out AccountInvoice accountInvoice))
                return View("Error");
            return Json(map.DataToViewModel(accountInvoice));
        }
    }
}
