using GSCrm.Data;
using GSCrm.Data.ApplicationInfo;
using GSCrm.DataTransformers;
using GSCrm.Helpers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using GSCrm.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;
using static GSCrm.Repository.AccountRepository;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(ACC_MANAGER)]
    public class AccountManagerController
        : MainController<AccountManager, AccountManagerViewModel, AccountManagerValidatior, AccountManagerTransformer, AccountManagerRepository>
    {
        public AccountManagerController(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager)
            : base(context, viewsInfo, resManager, new AccountManagerTransformer(context, resManager), new AccountManagerRepository(context, viewsInfo, resManager))
        { }

        [HttpGet("InitializeAccTeam/{accountId}")]
        public IActionResult InitializeAccTeam(string accountId)
        {
            repository = new AccountManagerRepository(context, viewsInfo, resManager, HttpContext);
            List<Employee> teamAllEmployees = repository.GetTeamAllEmployees(accountId);
            List<AccountManager> teamSelectedEmployees = repository.GetTeamSelectedEmployees(accountId);
            List<EmployeeViewModel> teamAllEmployeesVMs = teamAllEmployees.GetViewModelsFromData
                <Employee, EmployeeViewModel, EmployeeTransformer>(new EmployeeTransformer(context, resManager));
            List<AccountManagerViewModel> teamSelectedEmployeesVMs = teamSelectedEmployees.GetViewModelsFromData
                <AccountManager, AccountManagerViewModel, AccountManagerTransformer>(new AccountManagerTransformer(context, resManager));
            User currentUser = context.Users.FirstOrDefault(n => n.UserName == User.Identity.Name);
            Dictionary<string, object> result = new Dictionary<string, object>()
            {
                { "teamAllEmployees", teamAllEmployeesVMs },
                { "teamSelectedEmployees", teamSelectedEmployeesVMs },
                { "teamAllEmployeesVM", ModelCash<AccountViewModel>.GetViewModel(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES) },
                { "teamSelectedEmployeesVM", ModelCash<AccountViewModel>.GetViewModel(currentUser.Id, ACC_TEAM_SELECTED_EMPLOYEES) }
            };
            return Json(result);
        }

        [HttpGet("ClearAccTeamManagementSearch")]
        public IActionResult ClearAccTeamManagementSearch()
        {
            repository = new AccountManagerRepository(context, viewsInfo, resManager, HttpContext);
            repository.ClearAllManagersSearch();
            repository.ClearSelectedManagersSearch();
            return Ok();
        }

        [HttpPost("SearchAllManagers")]
        public IActionResult SearchAllManagers(AccountViewModel accountViewModel)
        {
            repository = new AccountManagerRepository(context, viewsInfo, resManager, HttpContext);
            repository.SearchAllManagers(accountViewModel);
            return RedirectToAction("InitializeAccTeam", ACC_MANAGER, new { accountId = CurrentAccount.Id });
        }

        [HttpGet("ClearAllManagersSearch")]
        public IActionResult ClearAllManagersSearch()
        {
            repository = new AccountManagerRepository(context, viewsInfo, resManager, HttpContext);
            repository.ClearAllManagersSearch();
            return RedirectToAction("InitializeAccTeam", ACC_MANAGER, new { accountId = CurrentAccount.Id });
        }

        [HttpPost("SearchSelectedManagers")]
        public IActionResult SearchSelectedManagers(AccountViewModel accountViewModel)
        {
            repository = new AccountManagerRepository(context, viewsInfo, resManager, HttpContext);
            repository.SearchSelectedManagers(accountViewModel);
            return RedirectToAction("InitializeAccTeam", ACC_MANAGER, new { accountId = CurrentAccount.Id });
        }

        [HttpGet("ClearSelectedManagersSearch")]
        public IActionResult ClearSelectedManagersSearch()
        {
            repository = new AccountManagerRepository(context, viewsInfo, resManager, HttpContext);
            repository.ClearSelectedManagersSearch();
            return RedirectToAction("InitializeAccTeam", ACC_MANAGER, new { accountId = CurrentAccount.Id });
        }

        [HttpGet("NextAllRecords/{accountId}")]
        public IActionResult NextAllRecords(string accountId)
        {
            User currentUser = context.Users.FirstOrDefault(n => n.UserName == User.Identity.Name);
            ViewInfo viewInfo = viewsInfo.Get(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES);
            repository = new AccountManagerRepository(context, viewsInfo, resManager, HttpContext);
            List<Employee> teamAllEmployees = repository.GetTeamAllEmployees(accountId, viewInfo.CurrentPageNumber + DEFAULT_PAGE_STEP);
            List<EmployeeViewModel> teamAllEmployeesVMs = teamAllEmployees.GetViewModelsFromData
                <Employee, EmployeeViewModel, EmployeeTransformer>(new EmployeeTransformer(context, resManager));
            return Json(teamAllEmployeesVMs);
        }

        [HttpGet("PreviousAllRecords/{accountId}")]
        public IActionResult PreviousAllRecords(string accountId)
        {
            User currentUser = context.Users.FirstOrDefault(n => n.UserName == User.Identity.Name);
            ViewInfo viewInfo = viewsInfo.Get(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES);
            repository = new AccountManagerRepository(context, viewsInfo, resManager, HttpContext);
            List<Employee> teamAllEmployees = repository.GetTeamAllEmployees(accountId, viewInfo.CurrentPageNumber - DEFAULT_PAGE_STEP);
            List<EmployeeViewModel> teamAllEmployeesVMs = teamAllEmployees.GetViewModelsFromData
                <Employee, EmployeeViewModel, EmployeeTransformer>(new EmployeeTransformer(context, resManager));
            return Json(teamAllEmployeesVMs);
        }

        [HttpPost("Synchronize")]
        public IActionResult Synchronize(SyncAccountViewModel syncViewModel)
        {
            Dictionary<string, string> syncErrors = new Dictionary<string, string>();
            if (repository.TrySyncPositions(syncViewModel, syncErrors))
                return RedirectToAction("InitializeAccTeam", ACC_MANAGER, new { accountId = CurrentAccount.Id });

            ModelStateDictionary modelState = ModelState;
            foreach (KeyValuePair<string, string> syncError in syncErrors)
                modelState.AddModelError(syncError.Key, syncError.Value);
            return BadRequest(modelState);
        }
    }
}
