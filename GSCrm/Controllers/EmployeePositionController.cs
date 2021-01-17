using GSCrm.Data.ApplicationInfo;
using GSCrm.Mapping;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using GSCrm.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;
using GSCrm.Data;
using GSCrm.Models.ViewTypes;
using GSCrm.Models.Enums;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(EMP_POSITION)]
    public class EmployeePositionController
        : MainController<EmployeePosition, EmployeePositionViewModel>
    {
        public EmployeePositionController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base (context, serviceProvider)
        { }

        [HttpGet("ListOfPositions/{pageNumber}")]
        public IActionResult Positions(int pageNumber)
        {
            EmployeeViewModel employeeViewModel = (EmployeeViewModel)cachService.GetMainEntity(currentUser, MainEntityType.EmployeeView);
            EmployeeRepository employeeRepository = new EmployeeRepository(serviceProvider, context);
            employeeRepository.SetViewInfo(EMP_POSITIONS, pageNumber);
            employeeRepository.AttachPositions(employeeViewModel);
            return View($"{EMP_VIEWS_REL_PATH}{EMPLOYEE}.cshtml", employeeViewModel);
        }

        [HttpGet("GetPositions/{employeeId}")]
        public IActionResult GetPositions(string employeeId)
        {
            if (!string.IsNullOrEmpty(employeeId) && Guid.TryParse(employeeId, out Guid guid))
            {
                // Получение списка со всеми должностями организации и списка с должностями сотрудника
                EmployeePositionRepository employeeRepository = new EmployeePositionRepository(serviceProvider, context);
                List<Position> allPositions = employeeRepository.AttachAllPositions(guid);
                List<EmployeePosition> selectedPositions = employeeRepository.AttachSelectedPositions(guid);
                List<PositionViewModel> allPositionVMs = allPositions.GetViewModelsFromData(new PositionMap(serviceProvider, context));
                List<EmployeePositionViewModel> selectedPositionsVMs = selectedPositions.GetViewModelsFromData(map);

                // Получение моделей с информацией об установленных для пользователя условиях поиска по должностям
                EmployeeViewModel allEmployeePossCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, ALL_EMP_POSS);
                allEmployeePossCash = new EmployeeMap(serviceProvider, context).Refresh(allEmployeePossCash, currentUser, EmployeeViewType.ALL_EMP_POSS);
                EmployeeViewModel selectedEmployeePossCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, SELECTED_EMP_POSS);
                selectedEmployeePossCash = new EmployeeMap(serviceProvider, context).Refresh(selectedEmployeePossCash, currentUser, EmployeeViewType.SELECTED_EMP_POSS);

                // Возврат результата
                Dictionary<string, object> result = new Dictionary<string, object>()
                {
                    { "allPositions", allPositionVMs },
                    { "selectedPositions", selectedPositionsVMs },
                    { "allPositionsVM", allEmployeePossCash },
                    { "selectedPositionsVM", selectedEmployeePossCash }
                };
                return Json(result);
            }
            else return BadRequest(resManager.GetString("PositionsExtractError"));
        }

        [HttpGet("NextAllRecords/{employeeId}")]
        public IActionResult NextAllRecords(string employeeId)
        {
            if (!string.IsNullOrEmpty(employeeId) && Guid.TryParse(employeeId, out Guid guid))
            {
                ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, ALL_EMP_POSS);
                EmployeePositionRepository employeeRepository = new EmployeePositionRepository(serviceProvider, context);
                List<Position> allPositions = employeeRepository.AttachAllPositions(guid, viewInfo.CurrentPageNumber + DEFAULT_PAGE_STEP);
                List<PositionViewModel> allPositionViewModels = allPositions.GetViewModelsFromData(new PositionMap(serviceProvider, context));
                return Json(allPositionViewModels);
            }
            return BadRequest("NextAllRecords");
        }

        [HttpGet("PreviousAllRecords/{employeeId}")]
        public IActionResult PreviousAllRecords(string employeeId)
        {
            if (!string.IsNullOrEmpty(employeeId) && Guid.TryParse(employeeId, out Guid guid))
            {
                ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, ALL_EMP_POSS);
                EmployeePositionRepository employeeRepository = new EmployeePositionRepository(serviceProvider, context);
                List<Position> allPositions = employeeRepository.AttachAllPositions(guid, viewInfo.CurrentPageNumber - DEFAULT_PAGE_STEP);
                List<PositionViewModel> allPositionViewModels = allPositions.GetViewModelsFromData(new PositionMap(serviceProvider, context));
                return Json(allPositionViewModels);
            }
            return BadRequest("PreviousAllRecords");
        }

        [HttpGet("NextSelectedRecords/{employeeId}")]
        public IActionResult NextSelectedRecords(string employeeId)
        {
            if (!string.IsNullOrEmpty(employeeId) && Guid.TryParse(employeeId, out Guid guid))
            {
                ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, SELECTED_EMP_POSS);
                EmployeePositionRepository employeeRepository = new EmployeePositionRepository(serviceProvider, context);
                List<EmployeePosition> selectedPositions = employeeRepository.AttachSelectedPositions(guid, viewInfo.CurrentPageNumber + DEFAULT_PAGE_STEP);
                List<EmployeePositionViewModel> selectedPositionViewModels = selectedPositions.GetViewModelsFromData(map);
                return Json(selectedPositionViewModels);
            }
            return BadRequest("NextSelectedRecords");
        }

        [HttpGet("PreviousSelectedRecords/{employeeId}")]
        public IActionResult PreviousSelectedRecords(string employeeId)
        {
            if (!string.IsNullOrEmpty(employeeId) && Guid.TryParse(employeeId, out Guid guid))
            {
                ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, SELECTED_EMP_POSS);
                EmployeePositionRepository employeeRepository = new EmployeePositionRepository(serviceProvider, context);
                List<EmployeePosition> selectedPositions = employeeRepository.AttachSelectedPositions(guid, viewInfo.CurrentPageNumber - DEFAULT_PAGE_STEP);
                List<EmployeePositionViewModel> selectedPositionViewModels = selectedPositions.GetViewModelsFromData(map);
                return Json(selectedPositionViewModels);
            }
            return BadRequest("PreviousSelectedRecords");
        }

        [HttpGet("ClearPositionManagementSearch")]
        public IActionResult ClearPositionManagementSearch()
        {
            EmployeePositionRepository employeeRepository = new EmployeePositionRepository(serviceProvider, context);
            employeeRepository.ClearAllPositionSearch();
            employeeRepository.ClearSelectedPositionSearch();
            return Ok();
        }

        [HttpPost("SearchAllPosition")]
        public IActionResult SearchAllPosition(EmployeeViewModel employeeViewModel)
        {
            cachService.CacheItem(currentUser.Id, ALL_EMP_POSS, employeeViewModel);
            return RedirectToAction("GetPositions", EMP_POSITION, new { employeeId = cachService.GetMainEntityId(currentUser, MainEntityType.EmployeeView) });
        }

        [HttpGet("ClearAllPositionSearch")]
        public IActionResult ClearAllPositionSearch()
        {
            EmployeePositionRepository employeeRepository = new EmployeePositionRepository(serviceProvider, context);
            employeeRepository.ClearAllPositionSearch();
            return RedirectToAction("GetPositions", EMP_POSITION, new { employeeId = cachService.GetMainEntityId(currentUser, MainEntityType.EmployeeView) });
        }

        [HttpPost("SearchSelectedPosition")]
        public IActionResult SearchSelectedPosition(EmployeeViewModel employeeViewModel)
        {
            cachService.CacheItem(currentUser.Id, SELECTED_EMP_POSS, employeeViewModel);
            return RedirectToAction("GetPositions", EMP_POSITION, new { employeeId = cachService.GetMainEntityId(currentUser, MainEntityType.EmployeeView) });
        }

        [HttpGet("ClearSelectedPositionSearch")]
        public IActionResult ClearSelectedPositionSearch()
        {
            EmployeePositionRepository employeeRepository = new EmployeePositionRepository(serviceProvider, context);
            employeeRepository.ClearSelectedPositionSearch();
            return RedirectToAction("GetPositions", EMP_POSITION, new { employeeId = cachService.GetMainEntityId(currentUser, MainEntityType.EmployeeView) });
        }

        [HttpPost("Synchronize")]
        public IActionResult Synchronize(SyncPositionsViewModel syncViewModel)
        {
            Dictionary<string, string> syncErrors = new Dictionary<string, string>();
            if (new EmployeePositionRepository(serviceProvider, context).TrySyncPositions(syncViewModel, ref syncErrors))
                return Json("");
            return BadRequest(syncErrors);
        }
    }
}
