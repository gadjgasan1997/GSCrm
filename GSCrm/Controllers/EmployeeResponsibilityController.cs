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
    [Route(EMP_RESPONSIBILITY)]
    public class EmployeeResponsibilityController
        : MainController<EmployeeResponsibility, EmployeeResponsibilityViewModel>
    {
        public EmployeeResponsibilityController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }

        [HttpGet("GetResponsibilities")]
        public IActionResult GetResponsibilities()
        {
            if (cachService.TryGetEntityCache(currentUser, out Employee employee))
            {
                // Получение моделей с информацией об установленных для пользователя условиях поиска по полномочиям
                if (!cachService.TryGetEntityCache(currentUser, out EmployeeViewModel allEmployeeRespsCash, ALL_EMP_RESPS))
                    allEmployeeRespsCash = new EmployeeViewModel();
                if (!cachService.TryGetEntityCache(currentUser, out EmployeeViewModel selectedEmployeeRespsCash, SELECTED_EMP_RESPS))
                    selectedEmployeeRespsCash = new EmployeeViewModel();

                // Получение списка со всеми полномочиями организации и списка с полномочиями сотрудника
                EmployeeResponsibilityRepository responsibilityRepository = new EmployeeResponsibilityRepository(serviceProvider, context);
                List<Responsibility> allResponsibilities = responsibilityRepository.GetAllResponsibilities(employee, allEmployeeRespsCash);
                List<Responsibility> selectedResponsibilities = responsibilityRepository.GetSelectedResponsibilities(employee, selectedEmployeeRespsCash);
                List<ResponsibilityViewModel> allResponsibilityVMs = allResponsibilities.GetViewModelsFromData(new ResponsibilityMap(serviceProvider, context));
                List<ResponsibilityViewModel> selectedResponsibilityVMs = selectedResponsibilities.GetViewModelsFromData(new ResponsibilityMap(serviceProvider, context));

                // Возврат результата
                Dictionary<string, object> result = new Dictionary<string, object>()
                {
                    { "allResponsibilitiesVMs", allResponsibilityVMs },
                    { "selectedResponsibilitiesVMs", selectedResponsibilityVMs },
                    { "allResponsibilitiesVM", allEmployeeRespsCash },
                    { "selectedResponsibilitiesVM", selectedEmployeeRespsCash }
                };
                return Json(result);
            }
            else return BadRequest(resManager.GetString("ResponsibilitiesExtractError"));
        }

        [HttpGet("NextAllRecords")]
        public IActionResult NextAllRecords()
        {
            if (cachService.TryGetEntityCache(currentUser, out Employee employee))
            {
                EmployeeResponsibilityRepository responsibilityRepository = new EmployeeResponsibilityRepository(serviceProvider, context);
                List<ResponsibilityViewModel> allResponsibilityVMs = responsibilityRepository.NavigateGetAllRecords(employee, NavigateDirection.Forward);
                return Json(allResponsibilityVMs);
            }
            return BadRequest("NextAllRecords");
        }

        [HttpGet("PreviousAllRecords")]
        public IActionResult PreviousAllRecords()
        {
            if (cachService.TryGetEntityCache(currentUser, out Employee employee))
            {
                EmployeeResponsibilityRepository responsibilityRepository = new EmployeeResponsibilityRepository(serviceProvider, context);
                List<ResponsibilityViewModel> allResponsibilityVMs = responsibilityRepository.NavigateGetAllRecords(employee, NavigateDirection.Backward);
                return Json(allResponsibilityVMs);
            }
            return BadRequest("PreviousAllRecords");
        }

        [HttpGet("NextSelectedRecords")]
        public IActionResult NextSelectedRecords()
        {
            if (cachService.TryGetEntityCache(currentUser, out Employee employee))
            {
                EmployeeResponsibilityRepository responsibilityRepository = new EmployeeResponsibilityRepository(serviceProvider, context);
                List<ResponsibilityViewModel> selectedResponsibilityVMs = responsibilityRepository.NavigateGetSelectedRecords(employee, NavigateDirection.Forward);
                return Json(selectedResponsibilityVMs);
            }
            return BadRequest("NextSelectedRecords");
        }

        [HttpGet("PreviousSelectedRecords")]
        public IActionResult PreviousSelectedRecords()
        {
            if (cachService.TryGetEntityCache(currentUser, out Employee employee))
            {
                EmployeeResponsibilityRepository responsibilityRepository = new EmployeeResponsibilityRepository(serviceProvider, context);
                List<ResponsibilityViewModel> selectedResponsibilityVMs = responsibilityRepository.NavigateGetSelectedRecords(employee, NavigateDirection.Backward);
                return Json(selectedResponsibilityVMs);
            }
            return BadRequest("PreviousSelectedRecords");
        }

        [HttpGet("ClearResponsibilityManagementSearch")]
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
            cachService.AddOrUpdate(currentUser, ALL_EMP_RESPS, employeeViewModel);
            return RedirectToAction("GetResponsibilities", EMP_RESPONSIBILITY);
        }

        [HttpGet("ClearAllResponsibilitiesSearch")]
        public IActionResult ClearAllResponsibilitiesSearch()
        {
            EmployeeResponsibilityRepository responsibilityRepository = new EmployeeResponsibilityRepository(serviceProvider, context);
            responsibilityRepository.ClearAllResponsibilitiesSearch();
            return RedirectToAction("GetResponsibilities", EMP_RESPONSIBILITY);
        }

        [HttpPost("SearchSelectedResponsibilities")]
        public IActionResult SearchSelectedResponsibilities(EmployeeViewModel employeeViewModel)
        {
            cachService.AddOrUpdate(currentUser, SELECTED_EMP_RESPS, employeeViewModel);
            return RedirectToAction("GetResponsibilities", EMP_RESPONSIBILITY);
        }

        [HttpGet("ClearSelectedResponsibilitiesSearch")]
        public IActionResult ClearSelectedResponsibilitiesSearch()
        {
            EmployeeResponsibilityRepository responsibilityRepository = new EmployeeResponsibilityRepository(serviceProvider, context);
            responsibilityRepository.ClearSelectedResponsibilitiesSearch();
            return RedirectToAction("GetResponsibilities", EMP_RESPONSIBILITY);
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
