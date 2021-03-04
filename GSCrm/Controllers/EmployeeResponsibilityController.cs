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
    [Route(EMP_RESPONSIBILITY)]
    public class EmployeeResponsibilityController
        : MainController<EmployeeResponsibility, EmployeeResponsibilityViewModel>
    {
        public EmployeeResponsibilityController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }

        [HttpGet("{employeeId}/GetResponsibilities")]
        public IActionResult GetResponsibilities()
        {
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);
            EmployeeViewModel employeeViewModel = cachService.GetCachedCurrentEntity<EmployeeViewModel>(currentUser);

            // Получение списка со всеми полномочиями организации и списка с полномочиями сотрудника
            EmployeeResponsibilityRepository responsibilityRepository = new EmployeeResponsibilityRepository(serviceProvider, context);
            List<Responsibility> allResponsibilities = responsibilityRepository.GetAllResponsibilities(employee, employeeViewModel);
            List<Responsibility> selectedResponsibilities = responsibilityRepository.GetSelectedResponsibilities(employee, employeeViewModel);
            ResponsibilityMap responsibilityMap = new ResponsibilityMap(serviceProvider, context);
            List<ResponsibilityViewModel> allResponsibilityVMs = allResponsibilities.GetViewModelsFromData(responsibilityMap);
            List<ResponsibilityViewModel> selectedResponsibilityVMs = selectedResponsibilities.GetViewModelsFromData(responsibilityMap);

            // Возврат результата
            Dictionary<string, object> result = new Dictionary<string, object>()
                {
                    { "allResponsibilitiesVMs", allResponsibilityVMs },
                    { "selectedResponsibilitiesVMs", selectedResponsibilityVMs },
                    { "employeeViewModel", employeeViewModel }
                };
            return Json(result);
        }

        [HttpGet("{employeeId}/NextAllRecords")]
        public IActionResult NextAllRecords()
        {
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);
            EmployeeResponsibilityRepository responsibilityRepository = new EmployeeResponsibilityRepository(serviceProvider, context);
            List<ResponsibilityViewModel> allResponsibilityVMs = responsibilityRepository.NavigateGetAllRecords(employee, NavigateDirection.Forward);
            return Json(allResponsibilityVMs);
        }

        [HttpGet("{employeeId}/PreviousAllRecords")]
        public IActionResult PreviousAllRecords()
        {
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);
            EmployeeResponsibilityRepository responsibilityRepository = new EmployeeResponsibilityRepository(serviceProvider, context);
            List<ResponsibilityViewModel> allResponsibilityVMs = responsibilityRepository.NavigateGetAllRecords(employee, NavigateDirection.Backward);
            return Json(allResponsibilityVMs);
        }

        [HttpGet("{employeeId}/NextSelectedRecords")]
        public IActionResult NextSelectedRecords()
        {
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);
            EmployeeResponsibilityRepository responsibilityRepository = new EmployeeResponsibilityRepository(serviceProvider, context);
            List<ResponsibilityViewModel> selectedResponsibilityVMs = responsibilityRepository.NavigateGetSelectedRecords(employee, NavigateDirection.Forward);
            return Json(selectedResponsibilityVMs);
        }

        [HttpGet("{employeeId}/PreviousSelectedRecords")]
        public IActionResult PreviousSelectedRecords()
        {
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);
            EmployeeResponsibilityRepository responsibilityRepository = new EmployeeResponsibilityRepository(serviceProvider, context);
            List<ResponsibilityViewModel> selectedResponsibilityVMs = responsibilityRepository.NavigateGetSelectedRecords(employee, NavigateDirection.Backward);
            return Json(selectedResponsibilityVMs);
        }

        [HttpGet("{employeeId}/ClearResponsibilityManagementSearch")]
        public IActionResult ClearResponsibilityManagementSearch()
        {
            EmployeeResponsibilityRepository responsibilityRepository = new EmployeeResponsibilityRepository(serviceProvider, context);
            responsibilityRepository.ClearAllResponsibilitiesSearch();
            responsibilityRepository.ClearSelectedResponsibilitiesSearch();
            return Ok();
        }

        [HttpPost("SearchAllResponsibilities")]
        public IActionResult SearchAllResponsibilities(EmployeeViewModel employeeViewModel)
        {
            new EmployeeResponsibilityRepository(serviceProvider, context).SearchAllResponsibilities(employeeViewModel);
            return Redirect($"/{EMP_RESPONSIBILITY}/{employeeViewModel.Id}/GetResponsibilities/");
        }

        [HttpGet("{employeeId}/ClearAllResponsibilitiesSearch")]
        public IActionResult ClearAllResponsibilitiesSearch(string employeeId)
        {
            new EmployeeResponsibilityRepository(serviceProvider, context).ClearAllResponsibilitiesSearch();
            return Redirect($"/{EMP_RESPONSIBILITY}/{employeeId}/GetResponsibilities/");
        }

        [HttpPost("SearchSelectedResponsibilities")]
        public IActionResult SearchSelectedResponsibilities(EmployeeViewModel employeeViewModel)
        {
            new EmployeeResponsibilityRepository(serviceProvider, context).SearchSelectedResponsibilities(employeeViewModel);
            return Redirect($"/{EMP_RESPONSIBILITY}/{employeeViewModel.Id}/GetResponsibilities/");
        }

        [HttpGet("{employeeId}/ClearSelectedResponsibilitiesSearch")]
        public IActionResult ClearSelectedResponsibilitiesSearch(string employeeId)
        {
            new EmployeeResponsibilityRepository(serviceProvider, context).ClearSelectedResponsibilitiesSearch();
            return Redirect($"/{EMP_RESPONSIBILITY}/{employeeId}/GetResponsibilities/");
        }

        [HttpPost("Synchronize")]
        public IActionResult Synchronize(SyncRespsViewModel syncViewModel)
        {
            Dictionary<string, string> syncErrors = new Dictionary<string, string>();
            if (new EmployeeResponsibilityRepository(serviceProvider, context).TrySyncResponsibilities(syncViewModel, ref syncErrors))
                return Json("");
            return BadRequest(syncErrors);
        }
    }
}
