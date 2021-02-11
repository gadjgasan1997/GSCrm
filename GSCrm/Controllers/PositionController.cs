using GSCrm.Mapping;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using GSCrm.Data;
using static GSCrm.CommonConsts;
using static GSCrm.Repository.PositionRepository;

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

        [HttpGet("HasNoPermissionsForSee")]
        public IActionResult HasNoPermissionsForSee()
            => View($"{POS_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new PositionViewModel());

        [HttpGet("{id}")]
        public ViewResult Position()
        {
            if (cachService.TryGetEntityCache(currentUser, out Position position))
            {
                PositionMap positionMap = new PositionMap(serviceProvider, context);
                PositionViewModel posViewModel = positionMap.DataToViewModelExt(position);
                posViewModel = positionMap.Refresh(posViewModel, currentUser, PosAllViewTypes);
                PositionRepository positionRepository = new PositionRepository(serviceProvider, context);
                positionRepository.AttachEmployees(posViewModel);
                positionRepository.AttachSubPositions(posViewModel);
                cachService.SetCurrentView(currentUser.Id, POSITION);
                cachService.AddOrUpdateEntity(currentUser, position);
                cachService.AddOrUpdateEntity(currentUser, posViewModel);
                return View(POSITION, posViewModel);
            }
            return View("Error");
        }

        /// <summary>
        /// Получение списка дочерних должностей
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("PositionSubPositions/{pageNumber}")]
        public IActionResult PositionSubPositions(int pageNumber)
        {
            if(cachService.TryGetEntityCache(currentUser, out PositionViewModel positionViewModel))
            {
                PositionRepository positionRepository = new PositionRepository(serviceProvider, context);
                positionRepository.SetViewInfo(POS_EMPLOYEES, pageNumber);
                positionRepository.AttachSubPositions(positionViewModel);
                return View($"{POS_VIEWS_REL_PATH}{POSITION}.cshtml", positionViewModel);
            }
            return View("Error");
        }

        /// <summary>
        /// Получение списка сотрудников, занимающих должность
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("PositionEmployees/{pageNumber}")]
        public IActionResult PositionEmployees(int pageNumber)
        {
            if(cachService.TryGetEntityCache(currentUser, out PositionViewModel positionViewModel))
            {
                PositionRepository positionRepository = new PositionRepository(serviceProvider, context);
                positionRepository.SetViewInfo(POS_EMPLOYEES, pageNumber);
                positionRepository.AttachEmployees(positionViewModel);
                return View($"{POS_VIEWS_REL_PATH}{POSITION}.cshtml", positionViewModel);
            }
            return View("Error");
        }

        [HttpPost("ChangeDivision")]
        public IActionResult ChangeDivision(PositionViewModel positionViewModel)
        {
            ModelStateDictionary modelState = ModelState;
            if (!new PositionRepository(serviceProvider, context).TryChangeDivision(positionViewModel, out Dictionary<string, string> errors))
            {
                AddErrorsToModel(modelState, errors);
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
                AddErrorsToModel(modelState, errors);
                return BadRequest(modelState);
            }
            return Json("");
        }

        [HttpPost("SearchEmployee")]
        public IActionResult SearchEmployee(PositionViewModel positionViewModel)
        {
            cachService.AddOrUpdate(currentUser, POS_EMPLOYEES, positionViewModel);
            return RedirectToAction(POSITION, POSITION, new { id = cachService.GetEntityId<PositionViewModel>(currentUser) });
        }

        [HttpGet("ClearSearchEmployee")]
        public IActionResult ClearSearchEmployee()
        {
            new PositionRepository(serviceProvider, context).ClearSearchEmployee();
            return RedirectToAction(POSITION, POSITION, new { id = cachService.GetEntityId<PositionViewModel>(currentUser) });
        }

        [HttpPost("SearchSubPosition")]
        public IActionResult SearchSubPosition(PositionViewModel positionViewModel)
        {
            cachService.AddOrUpdate(currentUser, POS_SUB_POSS, positionViewModel);
            return RedirectToAction(POSITION, POSITION, new { id = cachService.GetEntityId<PositionViewModel>(currentUser) });
        }

        [HttpGet("ClearSearchSubPosition")]
        public IActionResult ClearSearchSubPosition()
        {
            new PositionRepository(serviceProvider, context).ClearSearchSubPosition();
            return RedirectToAction(POSITION, POSITION, new { id = cachService.GetEntityId<PositionViewModel>(currentUser) });
        }
    }
}
