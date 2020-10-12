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
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using static GSCrm.CommonConsts;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(ACC_INVOICE)]
    public class AccountInvoiceController
        : MainController<AccountInvoice, AccountInvoiceViewModel, AccountInvoiceValidatior, AccountInvoiceTransformer, AccountInvoiceRepository>
    {
        public AccountInvoiceController(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager)
            : base(context, viewsInfo, resManager, new AccountInvoiceTransformer(context, resManager), new AccountInvoiceRepository(context, viewsInfo, resManager))
        { }

        [HttpGet(INVOICE)]
        public IActionResult Invoice(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid Id))
                return View("Error");

            AccountInvoice accountInvoice = context.AccountInvoices.FirstOrDefault(i => i.Id == Id);
            if (accountInvoice == null)
                return View("Error");

            return Json(transformer.DataToViewModel(accountInvoice));
        }
    }
}
