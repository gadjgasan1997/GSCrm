using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;

namespace GSCrm.Controllers
{
    [Route(APP_INFO)]
    public class AppInfoController : Controller
    {
        private readonly ApplicationDbContext context;
        public AppInfoController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet("GetCountries/{countryPart}")]
        public IActionResult GetCountries(string countryPart)
        {
            Func<JToken, bool> predicate = n => n.ToString().ToLower().Contains(countryPart.ToLower().TrimStartAndEnd());
            IEnumerable<JToken> result = AppUtils.GetCountries(HttpContext.GetCurrentUser(context)?.DefaultLanguage).Where(predicate).ToList();
            return Ok(result);
        }
    }
}
