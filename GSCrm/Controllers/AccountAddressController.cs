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
    [Route(ACC_ADDRESS)]
    public class AccountAddressController
        : MainController<AccountAddress, AccountAddressViewModel>
    {
        public AccountAddressController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }

        [HttpGet("{id}")]
        public IActionResult Address()
            => Json(cachService.GetCachedCurrentEntity<AccountAddressViewModel>(currentUser));
    }
}
