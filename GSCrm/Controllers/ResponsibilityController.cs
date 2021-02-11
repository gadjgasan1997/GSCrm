using System;
using GSCrm.Data;
using GSCrm.Data.ApplicationInfo;
using GSCrm.Mapping;
using GSCrm.Models;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
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

        [HttpGet("HasNoPermissionsForSee")]
        public IActionResult HasNoPermissionsForSee()
            => View($"{RESP_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new ResponsibilityViewModel());

        [HttpGet("{id}")]
        public ViewResult Responsibility()
        {
            if (cachService.TryGetEntityCache(currentUser, out Responsibility responsibility))
            {
                ResponsibilityViewModel respViewModel = new ResponsibilityMap(serviceProvider, context).DataToViewModel(responsibility);
                cachService.SetCurrentView(currentUser.Id, RESPONSIBILITY);
                cachService.AddOrUpdateEntity(currentUser, responsibility);
                cachService.AddOrUpdateEntity(currentUser, respViewModel);
                return View(RESPONSIBILITY, respViewModel);
            }
            return View("Error");
        }

        protected override IActionResult DeleteSuccessHandler()
        {
            if (cachService.TryGetEntityCache(currentUser, out OrganizationViewModel orgViewModel))
            {
                OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
                organizationRepository.AttachResponsibilities(orgViewModel);
                return Json(orgViewModel.Responsibilities);
            }
            return Json("");
        }

        [HttpGet("NextResponsibilities")]
        public IActionResult NextResponsibilities()
        {
            if (cachService.TryGetEntityCache(currentUser, out OrganizationViewModel orgViewModel))
            {
                ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, RESPONSIBILITIES);
                OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
                organizationRepository.SetViewInfo(RESPONSIBILITIES, viewInfo.CurrentPageNumber + DEFAULT_PAGE_STEP);
                organizationRepository.AttachResponsibilities(orgViewModel);
                return Json(orgViewModel.Responsibilities);
            }
            return BadRequest();
        }

        [HttpGet("PreviousResponsibilities")]
        public IActionResult PreviousResponsibilities()
        {
            if (cachService.TryGetEntityCache(currentUser, out OrganizationViewModel orgViewModel))
            {
                ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, RESPONSIBILITIES);
                OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
                organizationRepository.SetViewInfo(RESPONSIBILITIES, viewInfo.CurrentPageNumber - DEFAULT_PAGE_STEP);
                organizationRepository.AttachResponsibilities(orgViewModel);
                return Json(orgViewModel.Responsibilities);
            }
            return BadRequest();
        }
    }
}
