using System;
using System.Collections.Generic;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using static GSCrm.CommonConsts;

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

        [HttpGet("{id}")]
        public IActionResult Position(string id) => GetPosition(id);

        #region Child Entities
        /// <summary>
        /// Получение списка дочерних должностей
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("{id}/PositionSubPositions/{pageNumber}")]
        public IActionResult PositionSubPositions(string id, int pageNumber)
        {
            repository.SetViewInfo(id, POS_SUB_POSS, pageNumber);
            return GetPosition(id);
        }

        /// <summary>
        /// Получение списка сотрудников, занимающих должность
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("{id}/PositionEmployees/{pageNumber}")]
        public IActionResult PositionEmployees(string id, int pageNumber)
        {
            repository.SetViewInfo(id, POS_EMPLOYEES, pageNumber);
            return GetPosition(id);
        }
        #endregion

        #region Actions
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
        #endregion

        #region Searching
        [HttpPost("SearchEmployee")]
        public IActionResult SearchEmployee(PositionViewModel positionViewModel)
        {
            new PositionRepository(serviceProvider, context).SearchEmployee(positionViewModel);
            return RedirectToAction(POSITION, POSITION, new { id = positionViewModel.Id });
        }

        [HttpGet("{id}/ClearSearchEmployee")]
        public IActionResult ClearSearchEmployee(string id)
        {
            new PositionRepository(serviceProvider, context).ClearSearchEmployee();
            return RedirectToAction(POSITION, POSITION, new { id });
        }

        [HttpPost("SearchSubPosition")]
        public IActionResult SearchSubPosition(PositionViewModel positionViewModel)
        {
            new PositionRepository(serviceProvider, context).SearchSubPosition(positionViewModel);
            return RedirectToAction(POSITION, POSITION, new { id = positionViewModel.Id });
        }

        [HttpGet("{id}/ClearSearchSubPosition")]
        public IActionResult ClearSearchSubPosition(string id)
        {
            new PositionRepository(serviceProvider, context).ClearSearchSubPosition();
            return RedirectToAction(POSITION, POSITION, new { id });
        }
        #endregion

        #region Addition Methods
        private IActionResult GetPosition(string positionId)
        {
            if (cachService.TryGetCachedEntity(currentUser, positionId, out Position position))
                return View(POSITION, repository.LoadView(position));
            return View("Error");
        }
        #endregion
    }
}
