using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using GSCrm.Data;
using GSCrm.Utils;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Mapping;
using GSCrm.Factories;
using GSCrm.Models.ViewModels;
using static GSCrm.CommonConsts;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route("Autocomplite")]
    public class AutocompliteController : Controller
    {
        #region Declarations
        private readonly User currentUser;
        private readonly ApplicationDbContext context;
        private readonly AutocompliteUtils autocompliteUtils;
        private readonly DivisionMap divisionMap;
        private readonly PositionMap positionMap;
        #endregion

        public AutocompliteController(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            this.context = context;
            IUserContextFactory userContextServices = serviceProvider.GetService<IUserContextFactory>();
            currentUser = userContextServices.HttpContext.GetCurrentUser(context);
            autocompliteUtils = new AutocompliteUtils(serviceProvider, context);
            divisionMap = new DivisionMap(serviceProvider, context);
            positionMap = new PositionMap(serviceProvider, context);

        }

        [HttpGet("GetCountries/{countryPart?}")]
        public IActionResult GetCountries(string countryPart = "")
        {
            LangType? langType = HttpContext.GetCurrentUser(context)?.DefaultLanguage;
            IEnumerable<JToken> result = string.IsNullOrEmpty(countryPart) switch
            {
                true => AppUtils.GetCountries(langType).ToList(),
                _ => AppUtils.GetCountries(langType).Where(n => n.ToString().ToLower().Contains(countryPart.ToLower().TrimStartAndEnd())).ToList()
            };
            return Ok(result);
        }

        [HttpGet("GetEmployeesByFullName/{orgId}/{divNamePart}/{employeePart?}")]
        public IActionResult GetEmployeesByFullName(string orgId, string divNamePart, string employeePart = "")
            => Json(autocompliteUtils.GetEmployeesByFullName(orgId, divNamePart, employeePart));

        [HttpGet("GetEmployeesByInitials/{orgId}/{divNamePart}/{employeePart?}")]
        public IActionResult GetEmployees(string orgId, string divNamePart, string employeePart = "")
            => Json(autocompliteUtils.GetEmployeesByInitials(orgId, divNamePart, employeePart));

        [HttpGet("GetDivisions/{orgId}/{namePath?}")]
        public IActionResult GetDivisions(string orgId, string namePath = "")
        {
            if (string.IsNullOrEmpty(orgId) || !Guid.TryParse(orgId, out Guid organizationId))
                return Json("");

            IEnumerable<Division> divisions = context.Divisions.AsNoTracking().Take(AUTOCOMPLITE_ITEMS_DEF_COUNT);
            List<DivisionViewModel> divViewModels = namePath switch
            {
                "" => divisions.MapToViewModels(divisionMap, div => div.OrganizationId == organizationId),
                _ => divisions.MapToViewModels(divisionMap, div => div.OrganizationId == organizationId && div.Name.ToLower().Contains(namePath.TrimStartAndEnd().ToLower()))
            };
            return Json(divViewModels);

        }

        [HttpGet("GetDivisionsExceptCurrent/{orgId}/{divId}/{divNamePath?}")]
        public IActionResult GetDivisionsExceptCurrent(string orgId, string divId, string divNamePath = "")
        {
            if (string.IsNullOrEmpty(orgId) || !Guid.TryParse(orgId, out Guid organizationId) ||
                string.IsNullOrEmpty(divId) || !Guid.TryParse(divId, out Guid divisionId))
            {
                return Json("");
            }

            Func<Division, bool> predicate = string.IsNullOrEmpty(divNamePath) switch
            {
                true => div => div.OrganizationId == organizationId && div.Id != divisionId,
                _ => div => div.OrganizationId == organizationId && div.Name.ToLower().Contains(divNamePath.TrimStartAndEnd().ToLower()) && div.Id != divisionId
            };
            List<DivisionViewModel> divViewModels = context.Divisions.AsNoTracking().MapToViewModels(divisionMap, predicate);
            return Json(divViewModels);
        }

        [HttpGet("GetPositions/{orgId}/{divNamePart}/{posNamePart?}")]
        public IActionResult GetPositions(string orgId, string divNamePart, string posNamePart = "")
        {
            if (string.IsNullOrEmpty(orgId) ||
                string.IsNullOrEmpty(divNamePart) ||
                !Guid.TryParse(orgId, out Guid organizationId))
            {
                return Json("");
            }

            // Получение подразделения
            if (context.GetOrgDivisions(organizationId).FirstOrDefault(n => n.Name.ToLower() == divNamePart) is not Division selectedDivision)
                return Json("");

            // Получение списка должностей подразделения
            IEnumerable<Position> positions = selectedDivision.GetPositions(context).Take(AUTOCOMPLITE_ITEMS_DEF_COUNT);
            string searchPositionName = posNamePart.TrimStartAndEnd().ToLower();
            List<PositionViewModel> positionViewModels = searchPositionName switch
            {
                "" => positions.GetViewModelsFromData(positionMap),
                _ => positions.MapToViewModels(positionMap, pos => pos.Name.ToLower().Contains(searchPositionName))
            };
            return Json(positionViewModels);
        }

        [HttpGet("GetOrgEmployeesByInitials/{orgId}/{employeePart?}")]
        public IActionResult GetOrgEmployeesByInitials(string orgId, string employeePart = "")
            => Json(autocompliteUtils.GetOrgEmployeesByInitials(orgId, employeePart));

        [HttpGet("GetManagers/{accId}/{managerPart?}")]
        public IActionResult GetManagers(string accId, string managerPart = "")
        {
            if (string.IsNullOrEmpty(accId) || !Guid.TryParse(accId, out Guid accountId))
                return Json("");

            Account account = context.Accounts
                .AsNoTracking()
                .Include(acc => acc.AccountManagers)
                    .ThenInclude(man => man.Manager)
                .FirstOrDefault(i => i.Id == accountId);
            if (account?.AccountManagers.Count > 0)
                return Json(autocompliteUtils.GetAccountManagersByFullName(account, managerPart));
            return Json("");
        }
    }
}
