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
using static GSCrm.CommonConsts;
using GSCrm.Data;
using GSCrm.Models.ViewTypes;
using GSCrm.Models.Enums;

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

        [HttpGet("GetResponsibilities/{employeeId}")]
        public IActionResult GetResponsibilities(string employeeId)
        {
            if (!string.IsNullOrEmpty(employeeId) && Guid.TryParse(employeeId, out Guid guid))
            {
                // Получение списка со всеми полномочиями организации и списка с полномочиями сотрудника
                EmployeeResponsibilityRepository responsibilityRepository = new EmployeeResponsibilityRepository(serviceProvider, context);
                List<Responsibility> allResponsibilities = responsibilityRepository.AttachAllResponsibilities(guid);
                List<Responsibility> selectedResponsibilities = responsibilityRepository.AttachSelectedResponsibilities(guid);
                List<ResponsibilityViewModel> allResponsibilityVMs = allResponsibilities.GetViewModelsFromData(new ResponsibilityMap(serviceProvider, context));
                List<ResponsibilityViewModel> selectedResponsibilityVMs = selectedResponsibilities.GetViewModelsFromData(new ResponsibilityMap(serviceProvider, context));

                // Получение моделей с информацией об установленных для пользователя условиях поиска по полномочиям
                EmployeeViewModel allEmployeeRespsCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, ALL_EMP_RESPS);
                allEmployeeRespsCash = new EmployeeMap(serviceProvider, context).Refresh(allEmployeeRespsCash, currentUser, EmployeeViewType.ALL_EMP_RESPS);
                EmployeeViewModel selectedEmployeeRespsCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, SELECTED_EMP_RESPS);
                selectedEmployeeRespsCash = new EmployeeMap(serviceProvider, context).Refresh(selectedEmployeeRespsCash, currentUser, EmployeeViewType.SELECTED_EMP_RESPS);

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

        [HttpGet("NextAllRecords/{employeeId}")]
        public IActionResult NextAllRecords(string employeeId)
        {
            if (!string.IsNullOrEmpty(employeeId) && Guid.TryParse(employeeId, out Guid guid))
            {
                ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, ALL_EMP_RESPS);
                EmployeeResponsibilityRepository responsibilityRepository = new EmployeeResponsibilityRepository(serviceProvider, context);
                List<Responsibility> allResponsibilities = responsibilityRepository.AttachAllResponsibilities(guid, viewInfo.CurrentPageNumber + DEFAULT_PAGE_STEP);
                List<ResponsibilityViewModel> allResponsibilityVMs = allResponsibilities.GetViewModelsFromData(new ResponsibilityMap(serviceProvider, context));
                return Json(allResponsibilityVMs);
            }
            return BadRequest("NextAllRecords");
        }

        [HttpGet("PreviousAllRecords/{employeeId}")]
        public IActionResult PreviousAllRecords(string employeeId)
        {
            if (!string.IsNullOrEmpty(employeeId) && Guid.TryParse(employeeId, out Guid guid))
            {
                ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, ALL_EMP_RESPS);
                EmployeeResponsibilityRepository responsibilityRepository = new EmployeeResponsibilityRepository(serviceProvider, context);
                List<Responsibility> allResponsibilities = responsibilityRepository.AttachAllResponsibilities(guid, viewInfo.CurrentPageNumber - DEFAULT_PAGE_STEP);
                List<ResponsibilityViewModel> allResponsibilityVMs = allResponsibilities.GetViewModelsFromData(new ResponsibilityMap(serviceProvider, context));
                return Json(allResponsibilityVMs);
            }
            return BadRequest("PreviousAllRecords");
        }

        [HttpGet("NextSelectedRecords/{employeeId}")]
        public IActionResult NextSelectedRecords(string employeeId)
        {
            if (!string.IsNullOrEmpty(employeeId) && Guid.TryParse(employeeId, out Guid guid))
            {
                ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, SELECTED_EMP_RESPS);
                EmployeeResponsibilityRepository responsibilityRepository = new EmployeeResponsibilityRepository(serviceProvider, context);
                List<Responsibility> selectedResponsibilities = responsibilityRepository.AttachSelectedResponsibilities(guid, viewInfo.CurrentPageNumber + DEFAULT_PAGE_STEP);
                List<ResponsibilityViewModel> selectedResponsibilityVMs = selectedResponsibilities.GetViewModelsFromData(new ResponsibilityMap(serviceProvider, context));
                return Json(selectedResponsibilityVMs);
            }
            return BadRequest("NextSelectedRecords");
        }

        [HttpGet("PreviousSelectedRecords/{employeeId}")]
        public IActionResult PreviousSelectedRecords(string employeeId)
        {
            if (!string.IsNullOrEmpty(employeeId) && Guid.TryParse(employeeId, out Guid guid))
            {
                ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, SELECTED_EMP_RESPS);
                EmployeeResponsibilityRepository responsibilityRepository = new EmployeeResponsibilityRepository(serviceProvider, context);
                List<Responsibility> selectedResponsibilities = responsibilityRepository.AttachSelectedResponsibilities(guid, viewInfo.CurrentPageNumber - DEFAULT_PAGE_STEP);
                List<ResponsibilityViewModel> selectedResponsibilityVMs = selectedResponsibilities.GetViewModelsFromData(new ResponsibilityMap(serviceProvider, context));
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
            cachService.CacheItem(currentUser.Id, ALL_EMP_RESPS, employeeViewModel);
            return RedirectToAction("GetResponsibilities", EMP_RESPONSIBILITY, new { employeeId = cachService.GetMainEntityId(currentUser, MainEntityType.EmployeeView) });
        }

        [HttpGet("ClearAllResponsibilitiesSearch")]
        public IActionResult ClearAllResponsibilitiesSearch()
        {
            EmployeeResponsibilityRepository responsibilityRepository = new EmployeeResponsibilityRepository(serviceProvider, context);
            responsibilityRepository.ClearAllResponsibilitiesSearch();
            return RedirectToAction("GetResponsibilities", EMP_RESPONSIBILITY, new { employeeId = cachService.GetMainEntityId(currentUser, MainEntityType.EmployeeView) });
        }

        [HttpPost("SearchSelectedResponsibilities")]
        public IActionResult SearchSelectedResponsibilities(EmployeeViewModel employeeViewModel)
        {
            cachService.CacheItem(currentUser.Id, ALL_EMP_RESPS, employeeViewModel);
            return RedirectToAction("GetResponsibilities", EMP_RESPONSIBILITY, new { employeeId = cachService.GetMainEntityId(currentUser, MainEntityType.EmployeeView) });
        }

        [HttpGet("ClearSelectedResponsibilitiesSearch")]
        public IActionResult ClearSelectedResponsibilitiesSearch()
        {
            EmployeeResponsibilityRepository responsibilityRepository = new EmployeeResponsibilityRepository(serviceProvider, context);
            responsibilityRepository.ClearSelectedResponsibilitiesSearch();
            return RedirectToAction("GetResponsibilities", EMP_RESPONSIBILITY, new { employeeId = cachService.GetMainEntityId(currentUser, MainEntityType.EmployeeView) });
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
