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
    [Route(ACC_CONTACT)]
    public class AccountContactController
        : MainController<AccountContact, AccountContactViewModel>
    {
        public AccountContactController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }

        [HttpGet("{id}")]
        public IActionResult Contact()
        {
            if (cachService.TryGetValue(currentUser, $"{PC}{ACC_CONTACT}", out bool isCorrectCheck) && isCorrectCheck)
            {
                cachService.TryGetEntityCache(currentUser, out AccountContact accountContact);
                return Json(map.DataToViewModel(accountContact));
            }
            return Json("");
        }
    }
}
