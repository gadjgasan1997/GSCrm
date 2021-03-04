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
using GSCrm.Models.Enums;
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

        [HttpGet("{accountId}/GetManagers")]
        public IActionResult GetManagers()
        {
            AccountViewModel accountViewModel = cachService.GetCachedCurrentEntity<AccountViewModel>(currentUser);

            // Получение списка со всеми сотрудниками организации и команды по клиенту
            AccountManagerRepository accountManagerRepository = new AccountManagerRepository(serviceProvider, context);
            List<Employee> teamAllEmployees = accountManagerRepository.GetTeamAllEmployees(accountViewModel);
            List<AccountManager> teamSelectedEmployees = accountManagerRepository.GetTeamSelectedEmployees(accountViewModel);
            List<EmployeeViewModel> teamAllEmployeesVMs = teamAllEmployees.GetViewModelsFromData(new EmployeeMap(serviceProvider, context));
            List<AccountManagerViewModel> teamSelectedEmployeesVMs = teamSelectedEmployees.GetViewModelsFromData(new AccountManagerMap(serviceProvider, context));

            // Возврат результата
            Dictionary<string, object> result = new Dictionary<string, object>()
                {
                    { "teamAllEmployees", teamAllEmployeesVMs },
                    { "teamSelectedEmployees", teamSelectedEmployeesVMs },
                    { "accountViewModel", accountViewModel }
                };
            return Json(result);
        }

        [HttpGet("{accountId}/ClearAccTeamManagementSearch")]
        public IActionResult ClearAccTeamManagementSearch()
        {
            new AccountManagerRepository(serviceProvider, context).ClearAllManagersSearch();
            new AccountManagerRepository(serviceProvider, context).ClearSelectedManagersSearch();
            return Ok();
        }

        [HttpPost("SearchAllManagers")]
        public IActionResult SearchAllManagers(AccountViewModel accountViewModel)
        {
            new AccountManagerRepository(serviceProvider, context).SearchAllManagers(accountViewModel);
            return Redirect($"/{ACC_MANAGER}/{accountViewModel.Id}/GetManagers/");
        }

        [HttpGet("{accountId}/ClearAllManagersSearch")]
        public IActionResult ClearAllManagersSearch(string accountId)
        {
            new AccountManagerRepository(serviceProvider, context).ClearAllManagersSearch();
            return Redirect($"/{ACC_MANAGER}/{accountId}/GetManagers/");
        }

        [HttpPost("SearchSelectedManagers")]
        public IActionResult SearchSelectedManagers(AccountViewModel accountViewModel)
        {
            new AccountManagerRepository(serviceProvider, context).SearchSelectedManagers(accountViewModel);
            return Redirect($"/{ACC_MANAGER}/{accountViewModel.Id}/GetManagers/");
        }

        [HttpGet("{accountId}/ClearSelectedManagersSearch")]
        public IActionResult ClearSelectedManagersSearch(string accountId)
        {
            new AccountManagerRepository(serviceProvider, context).ClearSelectedManagersSearch();
            return Redirect($"/{ACC_MANAGER}/{accountId}/GetManagers/");
        }

        [HttpGet("{accountId}/NextAllRecords")]
        public IActionResult NextAllRecords()
        {
            AccountManagerRepository accountManagerRepository = new AccountManagerRepository(serviceProvider, context);
            return Json(accountManagerRepository.NavigateGetAllRecords(NavigateDirection.Forward));
        }

        [HttpGet("{accountId}/PreviousAllRecords")]
        public IActionResult PreviousAllRecords()
        {
            AccountManagerRepository accountManagerRepository = new AccountManagerRepository(serviceProvider, context);
            return Json(accountManagerRepository.NavigateGetAllRecords(NavigateDirection.Backward));
        }

        [HttpPost("Synchronize")]
        public IActionResult Synchronize(SyncAccountViewModel syncViewModel)
        {
            if (new AccountManagerRepository(serviceProvider, context).TrySyncAccTeam(syncViewModel, out Dictionary<string, string> syncErrors))
                return Redirect($"/{ACC_MANAGER}/{syncViewModel.Id}/GetManagers/");

            ModelStateDictionary modelState = ModelState;
            AddErrorsToModel(modelState, syncErrors);
            return BadRequest(modelState);
        }
    }
}
