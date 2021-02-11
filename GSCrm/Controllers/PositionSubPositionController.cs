using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static GSCrm.CommonConsts;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(POS_SUB_POS)]
    public class PositionSubPositionController
        : MainController<Position, PositionViewModel>
    {
        public PositionSubPositionController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }
    }
}
