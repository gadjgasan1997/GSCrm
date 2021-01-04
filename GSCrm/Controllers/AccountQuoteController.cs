using GSCrm.Mapping;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using GSCrm.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using static GSCrm.CommonConsts;
using Microsoft.AspNetCore.Http;
using GSCrm.Data;

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
