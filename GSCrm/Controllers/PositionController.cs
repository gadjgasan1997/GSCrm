using GSCrm.Data;
using GSCrm.Data.ApplicationInfo;
using GSCrm.DataTransformers;
using GSCrm.Helpers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using GSCrm.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;
using static GSCrm.Repository.OrganizationRepository;
using static GSCrm.Repository.PositionRepository;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(POSITION)]
    public class PositionController
        : MainController<Position, PositionViewModel, PositionValidator, PositionTransformer, PositionRepository>
    {
        public PositionController(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager)
            : base(context, viewsInfo, resManager, new PositionTransformer(context, resManager), new PositionRepository(context, viewsInfo, resManager))
        { }

        [HttpGet("ListOfPositions/{pageNumber}")]
        public IActionResult Positions(int pageNumber)
        {
            User currentUser = context.Users.FirstOrDefault(n => n.UserName == User.Identity.Name);
            OrganizationViewModel orgViewModel = CurrentOrganization;
            OrganizationRepository organizationRepository = new OrganizationRepository(context, viewsInfo, resManager, HttpContext);
            organizationRepository.SetViewInfo(currentUser.Id, POSITIONS, pageNumber);
            organizationRepository.AttachPositions(orgViewModel);
            return View($"{ORG_VIEWS_REL_PATH}{ORGANIZATION}.cshtml", orgViewModel);
        }

        [HttpGet("{id}")]
        public ViewResult Position(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid Id))
                return View("Error");
            Position position = context.Positions.FirstOrDefault(i => i.Id == Id);
            if (position == null)
                return View("Error");

            PositionViewModel posViewModel = transformer.DataToViewModel(position);
            posViewModel = new PositionTransformer(context, resManager, HttpContext).UpdateViewModelFromCash(posViewModel);
            repository = new PositionRepository(context, viewsInfo, resManager, HttpContext);
            repository.AttachEmployees(posViewModel);
            repository.AttachSubPositions(posViewModel);
            CurrentPosition = posViewModel;
            return View(POSITION, posViewModel);
        }

        [HttpGet("GetPositions/{orgId}/{divNamePart}/{posNamePart}")]
        public IActionResult GetPositions(string orgId, string divNamePart, string posNamePart)
        {
            if (!string.IsNullOrEmpty(divNamePart) && !string.IsNullOrEmpty(posNamePart) && Guid.TryParse(orgId, out Guid guid))
            {
                List<Division> divisions = context.GetOrgDivisions(guid);
                if (divisions?.Count > 0)
                {
                    Division selectedDivision = divisions.FirstOrDefault(n => n.Name == divNamePart);
                    if (selectedDivision == null) return Json("");

                    List<PositionViewModel> positionViewModels = selectedDivision.Positions
                        .TransformToViewModels<Position, PositionViewModel, PositionTransformer>(
                            transformer: transformer,
                            limitingFunc: pos => pos.Name.Contains(posNamePart, StringComparison.OrdinalIgnoreCase));
                    return Json(positionViewModels);
                }
            }
            return Json("");
        }

        public override IActionResult Update(PositionViewModel positionViewModel)
        {
            ModelStateDictionary modelState = ModelState;
            if (new PositionRepository(context, viewsInfo, resManager, HttpContext).TryUpdate(ref positionViewModel, modelState))
                return Json(Url.Action(POSITION, POSITION, new { id = CurrentPosition.Id }));
            return BadRequest(modelState);
        }

        [HttpPost("ChangeDivision")]
        public IActionResult ChangeDivision(PositionViewModel positionViewModel)
        {
            ModelStateDictionary modelState = ModelState;
            if (repository.TryChangeDivision(positionViewModel, modelState))
                return Json("");
            return BadRequest(modelState);
        }

        [HttpPost("SearchEmployee")]
        public IActionResult SearchEmployee(PositionViewModel positionViewModel)
        {
            repository = new PositionRepository(context, viewsInfo, resManager, HttpContext);
            repository.SearchEmployee(positionViewModel);
            return RedirectToAction(POSITION, POSITION, new { id = CurrentPosition.Id });
        }

        [HttpGet("ClearSearchEmployee")]
        public IActionResult ClearSearchEmployee()
        {
            repository = new PositionRepository(context, viewsInfo, resManager, HttpContext);
            repository.ClearSearchEmployee();
            return RedirectToAction(POSITION, POSITION, new { id = CurrentPosition.Id });
        }

        [HttpPost("SearchSubPosition")]
        public IActionResult SearchSubPosition(PositionViewModel positionViewModel)
        {
            repository = new PositionRepository(context, viewsInfo, resManager, HttpContext);
            repository.SearchSubPosition(positionViewModel);
            return RedirectToAction(POSITION, POSITION, new { id = CurrentPosition.Id });
        }

        [HttpGet("ClearSearchSubPosition")]
        public IActionResult ClearSearchSubPosition()
        {
            repository = new PositionRepository(context, viewsInfo, resManager, HttpContext);
            repository.ClearSearchSubPosition();
            return RedirectToAction(POSITION, POSITION, new { id = CurrentPosition.Id });
        }
    }
}
