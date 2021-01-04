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
using static GSCrm.CommonConsts;
using static GSCrm.Repository.AccountRepository;
using GSCrm.Data;
using GSCrm.Models.Enums;

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

        [HttpGet("InitializeAccTeam/{accountId}")]
        public IActionResult InitializeAccTeam(string accountId)
        {
            // Получение списка со всеми сотрудниками организации и команды по клиенту
            AccountManagerRepository accountManagerRepository = new AccountManagerRepository(serviceProvider, context);
            List<Employee> teamAllEmployees = accountManagerRepository.AttachTeamAllEmployees(accountId);
            List<AccountManager> teamSelectedEmployees = accountManagerRepository.GetTeamSelectedEmployees(accountId);
            List<EmployeeViewModel> teamAllEmployeesVMs = teamAllEmployees.GetViewModelsFromData(new EmployeeMap(serviceProvider, context));
            List<AccountManagerViewModel> teamSelectedEmployeesVMs = teamSelectedEmployees.GetViewModelsFromData(new AccountManagerMap(serviceProvider, context));

            // Получение моделей с информацией об установленных для пользователя условиях поиска по менеджерам
            AccountViewModel allAccManagersCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES);
            allAccManagersCash = new AccountMap(serviceProvider, context).Refresh(allAccManagersCash, currentUser, AccAllViewTypes);
            AccountViewModel selectedAccManagersCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_TEAM_SELECTED_EMPLOYEES);
            selectedAccManagersCash = new AccountMap(serviceProvider, context).Refresh(selectedAccManagersCash, currentUser, AccAllViewTypes);

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
            return RedirectToAction("InitializeAccTeam", ACC_MANAGER, new { accountId = cachService.GetMainEntityId(currentUser, MainEntityType.AccountView) });
        }

        [HttpGet("ClearAllManagersSearch")]
        public IActionResult ClearAllManagersSearch()
        {
            new AccountManagerRepository(serviceProvider, context).ClearAllManagersSearch();
            return RedirectToAction("InitializeAccTeam", ACC_MANAGER, new { accountId = cachService.GetMainEntityId(currentUser, MainEntityType.AccountView) });
        }

        [HttpPost("SearchSelectedManagers")]
        public IActionResult SearchSelectedManagers(AccountViewModel accountViewModel)
        {
            cachService.CacheItem(currentUser.Id, ACC_TEAM_SELECTED_EMPLOYEES, accountViewModel);
            return RedirectToAction("InitializeAccTeam", ACC_MANAGER, new { accountId = cachService.GetMainEntityId(currentUser, MainEntityType.AccountView) });
        }

        [HttpGet("ClearSelectedManagersSearch")]
        public IActionResult ClearSelectedManagersSearch()
        {
            new AccountManagerRepository(serviceProvider, context).ClearSelectedManagersSearch();
            return RedirectToAction("InitializeAccTeam", ACC_MANAGER, new { accountId = cachService.GetMainEntityId(currentUser, MainEntityType.AccountView) });
        }

        [HttpGet("NextAllRecords/{accountId}")]
        public IActionResult NextAllRecords(string accountId)
        {
            ViewInfo viewInfo = viewsInfo.Get(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES);
            List<Employee> teamAllEmployees = new AccountManagerRepository(serviceProvider, context).AttachTeamAllEmployees(accountId, viewInfo.CurrentPageNumber + DEFAULT_PAGE_STEP);
            List<EmployeeViewModel> teamAllEmployeesVMs = teamAllEmployees.GetViewModelsFromData(new EmployeeMap(serviceProvider, context));
            return Json(teamAllEmployeesVMs);
        }

        [HttpGet("PreviousAllRecords/{accountId}")]
        public IActionResult PreviousAllRecords(string accountId)
        {
            ViewInfo viewInfo = viewsInfo.Get(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES);
            List<Employee> teamAllEmployees = new AccountManagerRepository(serviceProvider, context).AttachTeamAllEmployees(accountId, viewInfo.CurrentPageNumber - DEFAULT_PAGE_STEP);
            List<EmployeeViewModel> teamAllEmployeesVMs = teamAllEmployees.GetViewModelsFromData(new EmployeeMap(serviceProvider, context));
            return Json(teamAllEmployeesVMs);
        }

        [HttpPost("Synchronize")]
        public IActionResult Synchronize(SyncAccountViewModel syncViewModel)
        {
            if (new AccountManagerRepository(serviceProvider, context).TrySyncAccTeam(syncViewModel, out Dictionary<string, string> syncErrors))
                return RedirectToAction("InitializeAccTeam", ACC_MANAGER, new { accountId = cachService.GetMainEntityId(currentUser, MainEntityType.AccountView) });

            ModelStateDictionary modelState = ModelState;
            foreach (KeyValuePair<string, string> syncError in syncErrors)
                modelState.AddModelError(syncError.Key, syncError.Value);
            return BadRequest(modelState);
        }
    }
}
