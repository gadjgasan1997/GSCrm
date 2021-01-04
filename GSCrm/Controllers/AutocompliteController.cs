using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using GSCrm.Data;
using GSCrm.Utils;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using GSCrm.Helpers;
using System.Linq;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using GSCrm.Mapping;
using GSCrm.Models;
using GSCrm.Factories;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route("Autocomplite")]
    public class AutocompliteController : Controller
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ApplicationDbContext context;
        private readonly User currentUser;
        private readonly AutocompliteUtils autocompliteUtils;
        private readonly DivisionMap divisionMap;
        private readonly PositionMap positionMap;
        public AutocompliteController(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            this.serviceProvider = serviceProvider;
            this.context = context;
            IUserContextFactory userContextServices = serviceProvider.GetService(typeof(IUserContextFactory)) as IUserContextFactory;
            currentUser = userContextServices.HttpContext.GetCurrentUser(context);
            autocompliteUtils = new AutocompliteUtils(serviceProvider, context);
            divisionMap = new DivisionMap(serviceProvider, context);
            positionMap = new PositionMap(serviceProvider, context);

        }

        [HttpGet("GetCountries/{countryPart}")]
        public IActionResult GetCountries(string countryPart)
        {
            Func<JToken, bool> predicate = n => n.ToString().ToLower().Contains(countryPart.ToLower().TrimStartAndEnd());
            IEnumerable<JToken> result = AppUtils.GetCountries(HttpContext.GetCurrentUser(context)?.DefaultLanguage).Where(predicate).ToList();
            return Ok(result);
        }

        [HttpGet("GetEmployeesByFullName/{orgId}/{divNamePart}/{employeePart}")]
        public IActionResult GetEmployeesByFullName(string orgId, string divNamePart, string employeePart)
            => Json(autocompliteUtils.GetEmployeesByFullName(orgId, divNamePart, employeePart));

        [HttpGet("GetEmployeesByInitials/{orgId}/{divNamePart}/{employeePart}")]
        public IActionResult GetEmployees(string orgId, string divNamePart, string employeePart)
            => Json(autocompliteUtils.GetEmployeesByInitials(orgId, divNamePart, employeePart));

        [HttpGet("GetDivisions/{orgId}/{namePath}")]
        public IActionResult GetDivisions(string orgId, string namePath)
        {
            List<DivisionViewModel> divViewModels = context.Divisions.AsNoTracking().ToList()
                .MapToViewModels(divisionMap, div => div.OrganizationId == Guid.Parse(orgId) && div.Name.ToLower().Contains(namePath.TrimStartAndEnd().ToLower()));
            return Json(divViewModels);
        }

        [HttpGet("GetDivisionsExceptCurrent/{orgId}/{divisionId}/{namePath}")]
        public IActionResult GetDivisionsExceptCurrent(string orgId, string divisionId, string namePath)
        {
            List<DivisionViewModel> divViewModels = context.Divisions.AsNoTracking().ToList()
                .MapToViewModels(divisionMap, div => div.OrganizationId == Guid.Parse(orgId) &&
                    div.Name.ToLower().Contains(namePath.TrimStartAndEnd().ToLower()) && div.Id != Guid.Parse(divisionId));
            return Json(divViewModels);
        }

        [HttpGet("GetPositions/{orgId}/{divNamePart}/{posNamePart}")]
        public IActionResult GetPositions(string orgId, string divNamePart, string posNamePart)
        {
            if (!string.IsNullOrEmpty(divNamePart) && !string.IsNullOrEmpty(posNamePart) && Guid.TryParse(orgId, out Guid guid))
            {
                List<Division> divisions = context.GetOrgDivisions(guid);
                if (divisions?.Count > 0)
                {
                    Division selectedDivision = divisions.FirstOrDefault(n => n.Name == divNamePart);
                    if (selectedDivision == null) return Json("");

                    List<PositionViewModel> positionViewModels = selectedDivision.Positions
                        .MapToViewModels(positionMap, pos => pos.Name.ToLower().Contains(posNamePart.TrimStartAndEnd().ToLower()));
                    return Json(positionViewModels);
                }
            }
            return Json("");
        }

        [HttpGet("GetOrgEmployeesByInitials/{orgId}/{employeePart}")]
        public IActionResult GetOrgEmployeesByInitials(string orgId, string employeePart)
            => Json(autocompliteUtils.GetOrgEmployeesByInitials(orgId, employeePart));

        [HttpGet("GetOrgEmployeesByInitials/{employeePart}")]
        public IActionResult GetOrgEmployeesByInitials(string employeePart)
            => Json(autocompliteUtils.GetOrgEmployeesByInitials(currentUser, employeePart));

        [HttpGet("GetManagers/{accId}/{managerPart}")]
        public IActionResult GetManagers(string accId, string managerPart)
        {
            if (!string.IsNullOrEmpty(accId) && !string.IsNullOrEmpty(managerPart) && Guid.TryParse(accId, out Guid accountId))
            {
                Account account = context.Accounts
                    .AsNoTracking()
                    .Include(acc => acc.AccountManagers)
                        .ThenInclude(man => man.Manager)
                    .FirstOrDefault(i => i.Id == accountId);
                if (account?.AccountManagers.Count > 0)
                    return Json(autocompliteUtils.GetAccountManagersByFullName(account, managerPart));
            }
            return Json("");
        }
    }
}
