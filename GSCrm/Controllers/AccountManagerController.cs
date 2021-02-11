using GSCrm.Data.ApplicationInfo;
using GSCrm.Mapping;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using GSCrm.Data;
using GSCrm.Models.Enums;
using static GSCrm.CommonConsts;
using static GSCrm.Repository.AccountRepository;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(ACC_MANAGER)]
    public class AccountManagerController
        : MainController<AccountManager, AccountManagerViewModel>
    {
        public AccountManagerController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }

        [HttpGet("InitializeAccTeam")]
        public IActionResult InitializeAccTeam()
        {
            if (cachService.GetMainEntity(currentUser, MainEntityType.AccountData) is Account account)
            {
                // Получение списка со всеми сотрудниками организации и команды по клиенту
                AccountManagerRepository accountManagerRepository = new AccountManagerRepository(serviceProvider, context);
                List<Employee> teamAllEmployees = accountManagerRepository.AttachTeamAllEmployees(account);
                List<AccountManager> teamSelectedEmployees = accountManagerRepository.GetTeamSelectedEmployees(account);
                List<EmployeeViewModel> teamAllEmployeesVMs = teamAllEmployees.GetViewModelsFromData(new EmployeeMap(serviceProvider, context));
                List<AccountManagerViewModel> teamSelectedEmployeesVMs = teamSelectedEmployees.GetViewModelsFromData(new AccountManagerMap(serviceProvider, context));

                // Получение моделей с информацией об установленных для пользователя условиях поиска по менеджерам
                AccountMap accountMap = new AccountMap(serviceProvider, context);
                AccountViewModel allAccManagersCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES);
                allAccManagersCash = accountMap.Refresh(allAccManagersCash, currentUser, AccAllViewTypes);
                AccountViewModel selectedAccManagersCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_TEAM_SELECTED_EMPLOYEES);
                selectedAccManagersCash = accountMap.Refresh(selectedAccManagersCash, currentUser, AccAllViewTypes);

                // Возврат результата
                Dictionary<string, object> result = new Dictionary<string, object>()
                {
                    { "teamAllEmployees", teamAllEmployeesVMs },
                    { "teamSelectedEmployees", teamSelectedEmployeesVMs },
                    { "teamAllEmployeesVM", allAccManagersCash },
                    { "teamSelectedEmployeesVM", selectedAccManagersCash }
                };
                return Json(result);
            }
            return Json("");
        }

        [HttpGet("ClearAccTeamManagementSearch")]
        public IActionResult ClearAccTeamManagementSearch()
        {
            new AccountManagerRepository(serviceProvider, context).ClearAllManagersSearch();
            new AccountManagerRepository(serviceProvider, context).ClearSelectedManagersSearch();
            return Ok();
        }

        [HttpPost("SearchAllManagers")]
        public IActionResult SearchAllManagers(AccountViewModel accountViewModel)
        {
            cachService.CacheItem(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES, accountViewModel);
            return RedirectToAction("InitializeAccTeam", ACC_MANAGER);
        }

        [HttpGet("ClearAllManagersSearch")]
        public IActionResult ClearAllManagersSearch()
        {
            new AccountManagerRepository(serviceProvider, context).ClearAllManagersSearch();
            return RedirectToAction("InitializeAccTeam", ACC_MANAGER);
        }

        [HttpPost("SearchSelectedManagers")]
        public IActionResult SearchSelectedManagers(AccountViewModel accountViewModel)
        {
            cachService.CacheItem(currentUser.Id, ACC_TEAM_SELECTED_EMPLOYEES, accountViewModel);
            return RedirectToAction("InitializeAccTeam", ACC_MANAGER);
        }

        [HttpGet("ClearSelectedManagersSearch")]
        public IActionResult ClearSelectedManagersSearch()
        {
            new AccountManagerRepository(serviceProvider, context).ClearSelectedManagersSearch();
            return RedirectToAction("InitializeAccTeam", ACC_MANAGER);
        }

        [HttpGet("NextAllRecords")]
        public IActionResult NextAllRecords()
        {
            if (cachService.GetMainEntity(currentUser, MainEntityType.AccountData) is Account account)
            {
                ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES);
                List<Employee> teamAllEmployees = new AccountManagerRepository(serviceProvider, context).AttachTeamAllEmployees(account, viewInfo.CurrentPageNumber + DEFAULT_PAGE_STEP);
                List<EmployeeViewModel> teamAllEmployeesVMs = teamAllEmployees.GetViewModelsFromData(new EmployeeMap(serviceProvider, context));
                return Json(teamAllEmployeesVMs);
            }
            return Json("");
        }

        [HttpGet("PreviousAllRecords")]
        public IActionResult PreviousAllRecords()
        {
            if (cachService.GetMainEntity(currentUser, MainEntityType.AccountData) is Account account)
            {
                ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES);
                List<Employee> teamAllEmployees = new AccountManagerRepository(serviceProvider, context).AttachTeamAllEmployees(account, viewInfo.CurrentPageNumber - DEFAULT_PAGE_STEP);
                List<EmployeeViewModel> teamAllEmployeesVMs = teamAllEmployees.GetViewModelsFromData(new EmployeeMap(serviceProvider, context));
                return Json(teamAllEmployeesVMs);
            }
            return Json("");
        }

        [HttpPost("Synchronize")]
        public IActionResult Synchronize(SyncAccountViewModel syncViewModel)
        {
            if (new AccountManagerRepository(serviceProvider, context).TrySyncAccTeam(syncViewModel, out Dictionary<string, string> syncErrors))
                return RedirectToAction("InitializeAccTeam", ACC_MANAGER);

            ModelStateDictionary modelState = ModelState;
            AddErrorsToModel(modelState, syncErrors);
            return BadRequest(modelState);
        }
    }
}
