using System;
using System.Collections.Generic;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Mapping;
using GSCrm.Repository;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("{employeeId}/GetPositions")]
        public IActionResult GetPositions()
        {
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);
            EmployeeViewModel employeeViewModel = cachService.GetCachedCurrentEntity<EmployeeViewModel>(currentUser);

            // Получение списка со всеми должностями организации и списка с должностями сотрудника
            EmployeePositionRepository employeeRepository = new EmployeePositionRepository(serviceProvider, context);
            List<Position> allPositions = employeeRepository.GetAllPositions(employee, employeeViewModel);
            List<EmployeePosition> selectedPositions = employeeRepository.GetSelectedPositions(employee, employeeViewModel);
            List<PositionViewModel> allPositionVMs = allPositions.GetViewModelsFromData(new PositionMap(serviceProvider, context));
            List<EmployeePositionViewModel> selectedPositionsVMs = selectedPositions.GetViewModelsFromData(map);

            // Возврат результата
            Dictionary<string, object> result = new Dictionary<string, object>()
                {
                    { "allPositions", allPositionVMs },
                    { "selectedPositions", selectedPositionsVMs },
                    { "employeeViewModel", employeeViewModel }
                };
            return Json(result);
        }

        [HttpGet("{employeeId}/NextAllRecords")]
        public IActionResult NextAllRecords()
        {
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);
            EmployeePositionRepository employeePositionRepository = new EmployeePositionRepository(serviceProvider, context);
            List<PositionViewModel> allPositionViewModels = employeePositionRepository.NavigateGetAllRecords(employee, NavigateDirection.Forward);
            return Json(allPositionViewModels);
        }

        [HttpGet("{employeeId}/PreviousAllRecords")]
        public IActionResult PreviousAllRecords()
        {
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);
            EmployeePositionRepository employeePositionRepository = new EmployeePositionRepository(serviceProvider, context);
            List<PositionViewModel> allPositionViewModels = employeePositionRepository.NavigateGetAllRecords(employee, NavigateDirection.Backward);
            return Json(allPositionViewModels);
        }

        [HttpGet("{employeeId}/NextSelectedRecords")]
        public IActionResult NextSelectedRecords()
        {
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);
            EmployeePositionRepository employeePositionRepository = new EmployeePositionRepository(serviceProvider, context);
            List<EmployeePositionViewModel> selectedPositionViewModels = employeePositionRepository.NavigateGetSelectedRecords(employee, NavigateDirection.Forward);
            return Json(selectedPositionViewModels);
        }

        [HttpGet("{employeeId}/PreviousSelectedRecords")]
        public IActionResult PreviousSelectedRecords()
        {
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);
            EmployeePositionRepository employeePositionRepository = new EmployeePositionRepository(serviceProvider, context);
            List<EmployeePositionViewModel> selectedPositionViewModels = employeePositionRepository.NavigateGetSelectedRecords(employee, NavigateDirection.Backward);
            return Json(selectedPositionViewModels);
        }

        [HttpGet("{employeeId}/ClearPositionManagementSearch")]
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
            new EmployeePositionRepository(serviceProvider, context).SearchAllPositions(employeeViewModel);
            return Redirect($"/{EMP_POSITION}/{employeeViewModel.Id}/GetPositions/");
        }

        [HttpGet("{employeeId}/ClearAllPositionSearch")]
        public IActionResult ClearAllPositionSearch(string employeeId)
        {
            new EmployeePositionRepository(serviceProvider, context).ClearAllPositionSearch();
            return Redirect($"/{EMP_POSITION}/{employeeId}/GetPositions/");
        }

        [HttpPost("SearchSelectedPosition")]
        public IActionResult SearchSelectedPosition(EmployeeViewModel employeeViewModel)
        {
            new EmployeePositionRepository(serviceProvider, context).SearchSelectedPositions(employeeViewModel);
            return Redirect($"/{EMP_POSITION}/{employeeViewModel.Id}/GetPositions/");
        }

        [HttpGet("{employeeId}/ClearSelectedPositionSearch")]
        public IActionResult ClearSelectedPositionSearch(string employeeId)
        {
            new EmployeePositionRepository(serviceProvider, context).ClearSelectedPositionSearch();
            return Redirect($"/{EMP_POSITION}/{employeeId}/GetPositions/");
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
