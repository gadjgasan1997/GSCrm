﻿using GSCrm.Data;
using GSCrm.Data.ApplicationInfo;
using GSCrm.Helpers;
using GSCrm.Mapping;
using GSCrm.Models;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using GSCrm.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using System;
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

        [HttpGet("Responsibilities/{pageNumber}")]
        public IActionResult Responsibilities(int pageNumber)
        {
            try
            {
                OrganizationViewModel orgViewModel = (OrganizationViewModel)cachService.GetMainEntity(currentUser, MainEntityType.OrganizationView);
                OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
                organizationRepository.SetViewInfo(currentUser.Id, RESPONSIBILITIES, pageNumber);
                organizationRepository.AttachResponsibilities(orgViewModel);
                return Json(orgViewModel.Responsibilities);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public ViewResult Responsibility(string id)
        {
            ResponsibilityRepository responsibilityRepository = new ResponsibilityRepository(serviceProvider, context);
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            if (!responsibilityRepository.TryGetItemById(id, out Responsibility responsibility))
                return View($"{RESP_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new ResponsibilityViewModel());
            if (!organizationRepository.HasPermissionsForSeeItem(responsibility.GetOrganization(context)))
                return View($"{ORG_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new OrganizationViewModel());
            if (!responsibilityRepository.HasPermissionsForSeeItem(responsibility))
                return View($"{RESP_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new ResponsibilityViewModel());

            ResponsibilityMap responsibilitMap = new ResponsibilityMap(serviceProvider, context);
            ResponsibilityViewModel responsibilityViewModel = responsibilitMap.DataToViewModel(responsibility);
            cachService.CacheItem(currentUser.Id, "CurrentResponsibilityData", responsibility);
            cachService.CacheItem(currentUser.Id, "CurrentResponsibilityView", responsibilityViewModel);
            return View(RESPONSIBILITY, responsibilityViewModel);
        }

        [HttpPost("Create")]
        public override IActionResult Create(ResponsibilityViewModel responsibilityViewModel)
        {
            try
            {
                ModelStateDictionary modelState = ModelState;
                OrganizationViewModel currentOrganization = (OrganizationViewModel)cachService.GetMainEntity(currentUser, MainEntityType.OrganizationView);
                ResponsibilityRepository responsibilityRepository = new ResponsibilityRepository(serviceProvider, context, currentOrganization);
                if (responsibilityRepository.TryCreate(ref responsibilityViewModel, modelState))
                    return Json(Url.Action(RESPONSIBILITY, RESPONSIBILITY, new { id = responsibilityRepository.NewRecord.Id.ToString() }));
                return BadRequest(modelState);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("Update")]
        public override IActionResult Update(ResponsibilityViewModel responsibilityViewModel)
        {
            try
            {
                ModelStateDictionary modelState = ModelState;
                OrganizationViewModel currentOrganization = (OrganizationViewModel)cachService.GetMainEntity(currentUser, MainEntityType.OrganizationView);
                if (new ResponsibilityRepository(serviceProvider, context, currentOrganization).TryUpdate(ref responsibilityViewModel, modelState))
                    return Json("");
                return BadRequest(modelState);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete("Delete")]
        public override IActionResult Delete(string id)
        {
            try
            {
                ModelStateDictionary modelState = ModelState;
                if (repository.TryDelete(id, modelState))
                {
                    OrganizationViewModel orgViewModel = (OrganizationViewModel)cachService.GetMainEntity(currentUser, MainEntityType.OrganizationView);
                    OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
                    organizationRepository.AttachResponsibilities(orgViewModel);
                    return Json(orgViewModel.Responsibilities);
                }
                return BadRequest(modelState);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("NextResponsibilities")]
        public IActionResult NextResponsibilities()
        {
            try
            {
                ViewInfo viewInfo = viewsInfo.Get(currentUser.Id, RESPONSIBILITIES);
                OrganizationViewModel orgViewModel = (OrganizationViewModel)cachService.GetMainEntity(currentUser, MainEntityType.OrganizationView);
                OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
                organizationRepository.SetViewInfo(currentUser.Id, RESPONSIBILITIES, viewInfo.CurrentPageNumber + DEFAULT_PAGE_STEP);
                organizationRepository.AttachResponsibilities(orgViewModel);
                return Json(orgViewModel.Responsibilities);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("PreviousResponsibilities")]
        public IActionResult PreviousResponsibilities()
        {
            try
            {
                ViewInfo viewInfo = viewsInfo.Get(currentUser.Id, RESPONSIBILITIES);
                OrganizationViewModel orgViewModel = (OrganizationViewModel)cachService.GetMainEntity(currentUser, MainEntityType.OrganizationView);
                OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
                organizationRepository.SetViewInfo(currentUser.Id, RESPONSIBILITIES, viewInfo.CurrentPageNumber - DEFAULT_PAGE_STEP);
                organizationRepository.AttachResponsibilities(orgViewModel);
                return Json(orgViewModel.Responsibilities);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
