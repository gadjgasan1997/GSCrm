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
    [Route(POS_EMPLOYEE)]
    public class PositionEmployeeController
        : MainController<Position, PositionViewModel>
    {
        public PositionEmployeeController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }

        [HttpGet("ListOfPositionEmployees/{pageNumber}")]
        public IActionResult PositionEmployees(int pageNumber)
        {
            PositionViewModel positionViewModel = (PositionViewModel)cachService.GetMainEntity(currentUser, MainEntityType.PositionView);
            PositionRepository positionRepository = new PositionRepository(serviceProvider, context);
            positionRepository.SetViewInfo(currentUser.Id, POS_EMPLOYEES, pageNumber);
            positionRepository.AttachEmployees(positionViewModel);
            return View($"{POS_VIEWS_REL_PATH}{POSITION}.cshtml", positionViewModel);
        }
    }
}