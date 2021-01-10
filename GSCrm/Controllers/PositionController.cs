using GSCrm.Mapping;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using static GSCrm.CommonConsts;
using static GSCrm.Repository.PositionRepository;
using GSCrm.Data;
using GSCrm.Models.Enums;
using GSCrm.Helpers;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(POSITION)]
    public class PositionController
        : MainController<Position, PositionViewModel>
    {
        public PositionController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }

        [HttpGet("ListOfPositions/{pageNumber}")]
        public IActionResult Positions(int pageNumber)
        {
            OrganizationViewModel orgViewModel = (OrganizationViewModel)cachService.GetMainEntity(currentUser, MainEntityType.OrganizationView);
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            organizationRepository.SetViewInfo(POSITIONS, pageNumber);
            organizationRepository.AttachPositions(orgViewModel);
            return View($"{ORG_VIEWS_REL_PATH}{ORGANIZATION}.cshtml", orgViewModel);
        }

        [HttpGet("{id}")]
        public ViewResult Position(string id)
        {
            PositionRepository positionRepository = new PositionRepository(serviceProvider, context);
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            if (!positionRepository.TryGetItemById(id, out Position position))
                return View($"{POS_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new PositionViewModel());
            if (!organizationRepository.HasPermissionsForSeeItem(position.GetOrganization(context)))
                return View($"{ORG_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new OrganizationViewModel());
            if (!positionRepository.HasPermissionsForSeeItem(position))
                return View($"{POS_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new PositionViewModel());

            PositionViewModel posViewModel = map.DataToViewModel(position);
            posViewModel = new PositionMap(serviceProvider, context).Refresh(posViewModel, currentUser, PosAllViewTypes);
            positionRepository.AttachEmployees(posViewModel);
            positionRepository.AttachSubPositions(posViewModel);
            cachService.CacheItem(currentUser.Id, "CurrentPositionData", position);
            cachService.CacheItem(currentUser.Id, "CurrentPositionView", posViewModel);
            return View(POSITION, posViewModel);
        }

        [HttpPost("ChangeDivision")]
        public IActionResult ChangeDivision(PositionViewModel positionViewModel)
        {
            ModelStateDictionary modelState = ModelState;
            if (!new PositionRepository(serviceProvider, context).TryChangeDivision(positionViewModel, out Dictionary<string, string> errors))
            {
                foreach (KeyValuePair<string, string> error in errors)
                    modelState.AddModelError(error.Key, error.Value);
                return BadRequest(modelState);
            }
            return Json("");
        }

        [HttpPost("Unlock")]
        public IActionResult Unlock(PositionViewModel positionViewModel)
        {
            ModelStateDictionary modelState = ModelState;
            if (!new PositionRepository(serviceProvider, context).TryUnlock(ref positionViewModel, out Dictionary<string, string> errors))
            {
                foreach (KeyValuePair<string, string> error in errors)
                    modelState.AddModelError(error.Key, error.Value);
                return BadRequest(modelState);
            }
            return Json("");
        }

        [HttpPost("SearchEmployee")]
        public IActionResult SearchEmployee(PositionViewModel positionViewModel)
        {
            cachService.CacheItem(currentUser.Id, POS_EMPLOYEES, positionViewModel);
            return RedirectToAction(POSITION, POSITION, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.PositionView) });
        }

        [HttpGet("ClearSearchEmployee")]
        public IActionResult ClearSearchEmployee()
        {
            new PositionRepository(serviceProvider, context).ClearSearchEmployee();
            return RedirectToAction(POSITION, POSITION, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.PositionView) });
        }

        [HttpPost("SearchSubPosition")]
        public IActionResult SearchSubPosition(PositionViewModel positionViewModel)
        {
            cachService.CacheItem(currentUser.Id, POS_SUB_POSS, positionViewModel);
            return RedirectToAction(POSITION, POSITION, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.PositionView) });
        }

        [HttpGet("ClearSearchSubPosition")]
        public IActionResult ClearSearchSubPosition()
        {
            PositionRepository positionRepository = new PositionRepository(serviceProvider, context);
            positionRepository.ClearSearchSubPosition();
            return RedirectToAction(POSITION, POSITION, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.PositionView) });
        }
    }
}
