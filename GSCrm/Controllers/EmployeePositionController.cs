using GSCrm.Mapping;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using GSCrm.Data;
using GSCrm.Models.Enums;
using static GSCrm.CommonConsts;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(EMP_POSITION)]
    public class EmployeePositionController
        : MainController<EmployeePosition, EmployeePositionViewModel>
    {
        public EmployeePositionController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }

        [HttpGet("GetPositions")]
        public IActionResult GetPositions()
        {
            if (cachService.TryGetEntityCache(currentUser, out Employee employee))
            {
                // Получение моделей с информацией об установленных для пользователя условиях поиска по должностям
                if (!cachService.TryGetEntityCache(currentUser, out EmployeeViewModel allEmployeePossCash, ALL_EMP_POSS))
                    allEmployeePossCash = new EmployeeViewModel();
                if (!cachService.TryGetEntityCache(currentUser, out EmployeeViewModel selectedEmployeePossCash, SELECTED_EMP_POSS))
                    selectedEmployeePossCash = new EmployeeViewModel();

                // Получение списка со всеми должностями организации и списка с должностями сотрудника
                EmployeePositionRepository employeeRepository = new EmployeePositionRepository(serviceProvider, context);
                List<Position> allPositions = employeeRepository.GetAllPositions(employee, allEmployeePossCash);
                List<EmployeePosition> selectedPositions = employeeRepository.GetSelectedPositions(employee, selectedEmployeePossCash);
                List<PositionViewModel> allPositionVMs = allPositions.GetViewModelsFromData(new PositionMap(serviceProvider, context));
                List<EmployeePositionViewModel> selectedPositionsVMs = selectedPositions.GetViewModelsFromData(map);

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
            return BadRequest(resManager.GetString("PositionsExtractError"));
        }

        [HttpGet("NextAllRecords")]
        public IActionResult NextAllRecords()
        {
            if (cachService.TryGetEntityCache(currentUser, out Employee employee))
            {
                EmployeePositionRepository employeePositionRepository = new EmployeePositionRepository(serviceProvider, context);
                List<PositionViewModel> allPositionViewModels = employeePositionRepository.NavigateGetAllRecords(employee, NavigateDirection.Forward);
                return Json(allPositionViewModels);
            }
            return BadRequest("NextAllRecords");
        }

        [HttpGet("PreviousAllRecords")]
        public IActionResult PreviousAllRecords()
        {
            if (cachService.TryGetEntityCache(currentUser, out Employee employee))
            {
                EmployeePositionRepository employeePositionRepository = new EmployeePositionRepository(serviceProvider, context);
                List<PositionViewModel> allPositionViewModels = employeePositionRepository.NavigateGetAllRecords(employee, NavigateDirection.Backward);
                return Json(allPositionViewModels);
            }
            return BadRequest("PreviousAllRecords");
        }

        [HttpGet("NextSelectedRecords")]
        public IActionResult NextSelectedRecords()
        {
            if (cachService.TryGetEntityCache(currentUser, out Employee employee))
            {
                EmployeePositionRepository employeePositionRepository = new EmployeePositionRepository(serviceProvider, context);
                List<EmployeePositionViewModel> selectedPositionViewModels = employeePositionRepository.NavigateGetSelectedRecords(employee, NavigateDirection.Forward);
                return Json(selectedPositionViewModels);
            }
            return BadRequest("NextSelectedRecords");
        }

        [HttpGet("PreviousSelectedRecords")]
        public IActionResult PreviousSelectedRecords()
        {
            if (cachService.TryGetEntityCache(currentUser, out Employee employee))
            {
                EmployeePositionRepository employeePositionRepository = new EmployeePositionRepository(serviceProvider, context);
                List<EmployeePositionViewModel> selectedPositionViewModels = employeePositionRepository.NavigateGetSelectedRecords(employee, NavigateDirection.Backward);
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
            cachService.AddOrUpdate(currentUser, ALL_EMP_POSS, employeeViewModel);
            return RedirectToAction("GetPositions", EMP_POSITION);
        }

        [HttpGet("ClearAllPositionSearch")]
        public IActionResult ClearAllPositionSearch()
        {
            EmployeePositionRepository employeeRepository = new EmployeePositionRepository(serviceProvider, context);
            employeeRepository.ClearAllPositionSearch();
            return RedirectToAction("GetPositions", EMP_POSITION);
        }

        [HttpPost("SearchSelectedPosition")]
        public IActionResult SearchSelectedPosition(EmployeeViewModel employeeViewModel)
        {
            cachService.AddOrUpdate(currentUser, SELECTED_EMP_POSS, employeeViewModel);
            return RedirectToAction("GetPositions", EMP_POSITION);
        }

        [HttpGet("ClearSelectedPositionSearch")]
        public IActionResult ClearSelectedPositionSearch()
        {
            EmployeePositionRepository employeeRepository = new EmployeePositionRepository(serviceProvider, context);
            employeeRepository.ClearSelectedPositionSearch();
            return RedirectToAction("GetPositions", EMP_POSITION);
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
