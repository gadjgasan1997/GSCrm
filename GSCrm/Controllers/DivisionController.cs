using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using GSCrm.Data;
using static GSCrm.CommonConsts;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(DIVISION)]
    public class DivisionController
        : MainController<Division, DivisionViewModel>
    {
        public DivisionController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }
    }
}
