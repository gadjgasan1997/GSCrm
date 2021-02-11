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
            Responsibility responsibility = cachService.GetMainEntity(currentUser, MainEntityType.ResponsibilityData) as Responsibility;
            ResponsibilityViewModel respViewModel = new ResponsibilityMap(serviceProvider, context).DataToViewModel(responsibility);
            cachService.SetCurrentView(currentUser.Id, RESPONSIBILITY);
            cachService.CacheResponsibility(currentUser, responsibility, respViewModel);
            return View(RESPONSIBILITY, respViewModel);
        }

        protected override IActionResult DeleteSuccessHandler()
        {
            OrganizationViewModel orgViewModel = (OrganizationViewModel)cachService.GetMainEntity(currentUser, MainEntityType.OrganizationView);
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            organizationRepository.AttachResponsibilities(orgViewModel);
            return Json(orgViewModel.Responsibilities);
        }

        [HttpGet("NextResponsibilities")]
        public IActionResult NextResponsibilities()
        {
            ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, RESPONSIBILITIES);
            OrganizationViewModel orgViewModel = (OrganizationViewModel)cachService.GetMainEntity(currentUser, MainEntityType.OrganizationView);
            if (orgViewModel != null)
            {
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
            ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, RESPONSIBILITIES);
            OrganizationViewModel orgViewModel = (OrganizationViewModel)cachService.GetMainEntity(currentUser, MainEntityType.OrganizationView);
            if (orgViewModel != null)
            {
                OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
                organizationRepository.SetViewInfo(RESPONSIBILITIES, viewInfo.CurrentPageNumber - DEFAULT_PAGE_STEP);
                organizationRepository.AttachResponsibilities(orgViewModel);
                return Json(orgViewModel.Responsibilities);
            }
            return BadRequest();
        }
    }
}
