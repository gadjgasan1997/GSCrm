using System;
using System.Collections.Generic;
using GSCrm.Data.ApplicationInfo;
using GSCrm.Data;
using GSCrm.Mapping;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using static GSCrm.CommonConsts;

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
            if (cachService.TryGetEntityCache(currentUser, out Account account))
            {
                // Получение моделей с информацией об установленных для пользователя условиях поиска по менеджерам
                if (!cachService.TryGetEntityCache(currentUser, out AccountViewModel allAccManagersCash, ACC_TEAM_ALL_EMPLOYEES))
                    allAccManagersCash = new AccountViewModel();
                if (!cachService.TryGetEntityCache(currentUser, out AccountViewModel selectedAccManagersCash, ACC_TEAM_SELECTED_EMPLOYEES))
                    selectedAccManagersCash = new AccountViewModel();

                // Получение списка со всеми сотрудниками организации и команды по клиенту
                AccountManagerRepository accountManagerRepository = new AccountManagerRepository(serviceProvider, context);
                List<Employee> teamAllEmployees = accountManagerRepository.GetTeamAllEmployees(account, allAccManagersCash);
                List<AccountManager> teamSelectedEmployees = accountManagerRepository.GetTeamSelectedEmployees(account, selectedAccManagersCash);
                List<EmployeeViewModel> teamAllEmployeesVMs = teamAllEmployees.GetViewModelsFromData(new EmployeeMap(serviceProvider, context));
                List<AccountManagerViewModel> teamSelectedEmployeesVMs = teamSelectedEmployees.GetViewModelsFromData(new AccountManagerMap(serviceProvider, context));

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
            cachService.AddOrUpdate(currentUser, ACC_TEAM_ALL_EMPLOYEES, accountViewModel);
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
            cachService.AddOrUpdate(currentUser, ACC_TEAM_SELECTED_EMPLOYEES, accountViewModel);
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
            if (cachService.TryGetEntityCache(currentUser, out Account account))
            {
                ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES);
                if (!cachService.TryGetEntityCache(currentUser, out AccountViewModel allAccManagersCash, ACC_TEAM_ALL_EMPLOYEES))
                    allAccManagersCash = new AccountViewModel();
                AccountManagerRepository accountManagerRepository = new AccountManagerRepository(serviceProvider, context);
                List<Employee> teamAllEmployees = accountManagerRepository.GetTeamAllEmployees(account, allAccManagersCash, viewInfo.CurrentPageNumber + DEFAULT_PAGE_STEP);
                List<EmployeeViewModel> teamAllEmployeesVMs = teamAllEmployees.GetViewModelsFromData(new EmployeeMap(serviceProvider, context));
                return Json(teamAllEmployeesVMs);
            }
            return Json("");
        }

        [HttpGet("PreviousAllRecords")]
        public IActionResult PreviousAllRecords()
        {
            if (cachService.TryGetEntityCache(currentUser, out Account account))
            {
                ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES);
                if (!cachService.TryGetEntityCache(currentUser, out AccountViewModel selectedAccManagersCash, ACC_TEAM_SELECTED_EMPLOYEES))
                    selectedAccManagersCash = new AccountViewModel();
                AccountManagerRepository accountManagerRepository = new AccountManagerRepository(serviceProvider, context);
                List<Employee> teamAllEmployees = accountManagerRepository.GetTeamAllEmployees(account, selectedAccManagersCash, viewInfo.CurrentPageNumber - DEFAULT_PAGE_STEP);
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
