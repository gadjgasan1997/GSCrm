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
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;
using static GSCrm.Repository.OrganizationRepository;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(DIVISION)]
    public class DivisionController
        : MainController<Division, DivisionViewModel, DivisionValidatior, DivisionTransformer, DivisionRepository>
    {
        public DivisionController(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager)
            : base(context, viewsInfo, resManager, new DivisionTransformer(context, resManager), new DivisionRepository(context, viewsInfo, resManager))
        { }

        [HttpGet("ListOfDivisions/{pageNumber}")]
        public IActionResult Divisions(int pageNumber)
        {
            User currentUser = context.Users.FirstOrDefault(n => n.UserName == User.Identity.Name);
            OrganizationViewModel orgViewModel = CurrentOrganization;
            OrganizationRepository organizationRepository = new OrganizationRepository(context, viewsInfo, resManager, HttpContext);
            organizationRepository.SetViewInfo(currentUser.Id, DIVISIONS, pageNumber);
            organizationRepository.AttachDivisions(orgViewModel);
            return View($"{ORG_VIEWS_REL_PATH}{ORGANIZATION}.cshtml", orgViewModel);
        }

        [HttpGet("{id}")]
        public ViewResult Division(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid Id))
                return View("Error");
            Division division = context.Divisions.FirstOrDefault(i => i.Id == Id);
            if (division == null)
                return View("Error");
            DivisionViewModel divViewModel = transformer.DataToViewModel(division);
            return View($"{ORG_VIEWS_REL_PATH}{DIVISION}.cshtml", divViewModel);
        }

        [HttpGet("GetDivisions/{orgId}/{namePath}")]
        public IActionResult GetDivisions(string orgId, string namePath)
        {
            List<DivisionViewModel> divViewModels = context.Divisions.ToList()
                .TransformToViewModels<Division, DivisionViewModel, DivisionTransformer>(
                    transformer: transformer,
                    limitingFunc: div => div.OrganizationId == Guid.Parse(orgId) && div.Name.Contains(namePath, StringComparison.OrdinalIgnoreCase))
                .Take(10).ToList();
            return Json(divViewModels);
        }

        [HttpGet("GetDivisionsExceptCurrent/{orgId}/{divisionId}/{namePath}")]
        public IActionResult GetDivisionsExceptCurrent(string orgId, string divisionId, string namePath)
        {
            List<DivisionViewModel> divViewModels = context.Divisions.ToList()
                .TransformToViewModels<Division, DivisionViewModel, DivisionTransformer>(
                    transformer: transformer,
                    limitingFunc: div => div.OrganizationId == Guid.Parse(orgId) && div.Name.Contains(namePath, StringComparison.OrdinalIgnoreCase) && div.Id != Guid.Parse(divisionId))
                .Take(10).ToList();
            return Json(divViewModels);
        }
    }
}
