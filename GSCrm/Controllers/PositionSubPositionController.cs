using GSCrm.Data;
using GSCrm.Mapping;
using GSCrm.Models;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using GSCrm.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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

        [HttpGet("ListOfSubPositions/{pageNumber}")]
        public IActionResult PositionSubPositions(int pageNumber)
        {
            PositionViewModel positionViewModel = (PositionViewModel)cachService.GetMainEntity(currentUser, MainEntityType.PositionView);
            PositionRepository positionRepository = new PositionRepository(serviceProvider, context);
            positionRepository.SetViewInfo(currentUser.Id, POS_EMPLOYEES, pageNumber);
            positionRepository.AttachSubPositions(positionViewModel);
            return View($"{POS_VIEWS_REL_PATH}{POSITION}.cshtml", positionViewModel);
        }
    }
}
