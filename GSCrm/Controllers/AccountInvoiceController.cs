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
            => Json(cachService.GetCachedCurrentEntity<AccountInvoiceViewModel>(currentUser));
    }
}
