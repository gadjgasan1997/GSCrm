using System;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using GSCrm.Data;
using static GSCrm.CommonConsts;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(ACC_QUOTE)]
    public class AccountQuoteController
        : MainController<AccountQuote, AccountQuoteViewModel>
    {
        public AccountQuoteController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }
    }
}
