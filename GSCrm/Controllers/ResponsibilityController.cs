using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using static GSCrm.CommonConsts;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(RESPONSIBILITY)]
    public class ResponsibilityController
        : MainController<Responsibility, ResponsibilityViewModel>
    {
        public ResponsibilityController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }

        [HttpGet("{id}")]
        public IActionResult Responsibility() => GetResponsibility();

        protected override IActionResult DeleteSuccessHandler()
        {
            Organization currentOrganization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
            return Json($"/{ORGANIZATION}/{currentOrganization.Id}/GetResponsibilities/");
        }

        #region Addition Methods
        private IActionResult GetResponsibility()
        {
            Responsibility responsibility = cachService.GetCachedCurrentEntity<Responsibility>(currentUser);
            return View(RESPONSIBILITY, repository.LoadView(responsibility));
        }
        #endregion
    }
}
