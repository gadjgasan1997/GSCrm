using GSCrm.Mapping;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using GSCrm.Data;
using GSCrm.Models.Enums;
using static GSCrm.CommonConsts;
using static GSCrm.Repository.OrganizationRepository;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(ORGANIZATION)]
    public class OrganizationController
        : MainController<Organization, OrganizationViewModel>
    {
        public OrganizationController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }

        [HttpGet("ListOfOrganizations/{pageNumber}")]
        public ViewResult Organizations(int pageNumber)
        {
            OrganizationsViewModel orgsViewModel = cachService.GetCachedItem<OrganizationsViewModel>(currentUser.Id, ORGANIZATIONS);
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            organizationRepository.SetViewInfo(ORGANIZATIONS, pageNumber);
            organizationRepository.AttachOrganizations(ref orgsViewModel);
            return View(ORGANIZATIONS, orgsViewModel);
        }

        [HttpGet("{id}")]
        public ViewResult Organization(string id)
        {
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            if (!organizationRepository.TryGetItemById(id, out Organization organization))
                return View($"{ORG_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new OrganizationViewModel());
            if (!organizationRepository.HasPermissionsForSeeItem(organization))
                return View($"{ORG_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new OrganizationViewModel());

            OrganizationViewModel orgViewModel = map.DataToViewModel(organization);
            orgViewModel = new OrganizationMap(serviceProvider, context).Refresh(orgViewModel, currentUser, OrgAllViewTypes);
            organizationRepository.AttachDivisions(orgViewModel);
            organizationRepository.AttachPositions(orgViewModel);
            organizationRepository.AttachEmployees(orgViewModel);
            organizationRepository.AttachResponsibilities(orgViewModel);
            cachService.SetCurrentViewName(currentUser.Id, ORGANIZATION);
            cachService.CacheOrganization(currentUser, organization, orgViewModel);
            return View(ORGANIZATION, orgViewModel);
        }

        [HttpGet("BackToOrganization/{orgId}")]
        public IActionResult BackToOrganization(string orgId)
        {
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            if (!organizationRepository.TryGetItemById(orgId, out Organization organization))
                return View($"{ORG_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new OrganizationViewModel());
            if (!organizationRepository.HasPermissionsForSeeItem(organization))
                return View($"{ORG_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new OrganizationViewModel());
            return RedirectToAction(ORGANIZATION, ORGANIZATION, new { id = organization.Id });
        }

        [HttpGet("{orgId}/ProductCategories/")]
        public ViewResult ProductCategories(string orgId)
        {
            // Поптыка получить организацию, на которую перешел пользователь
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            if (!organizationRepository.TryGetItemById(orgId, out Organization organization))
                return View($"{ORG_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new OrganizationViewModel());
            if (!organizationRepository.HasPermissionsForSeeItem(organization))
                return View($"{ORG_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new OrganizationViewModel());
            else
            {
                // Если к ней есть доступ, то установка ее как текущей, на которой находится пользователь
                OrganizationViewModel orgViewModel = map.DataToViewModel(organization);
                orgViewModel = new OrganizationMap(serviceProvider, context).Refresh(orgViewModel, currentUser, OrgAllViewTypes);
                cachService.CacheItem(currentUser.Id, "CurrentOrganizationData", organization);
                cachService.CacheItem(currentUser.Id, "CurrentOrganizationView", orgViewModel);
                return View($"{PROD_CAT_VIEWS_REL_PATH}{PROD_CATS}.cshtml", new ProductCategoriesViewModel());
            }
        }

        [HttpGet("ChangePrimaryOrg/{newPrimaryOrgId}")]
        public IActionResult ChangePrimaryOrg(string newPrimaryOrgId)
        {
            ModelStateDictionary modelState = ModelState;
            if (!new OrganizationRepository(serviceProvider, context).TryChangePrimaryOrg(newPrimaryOrgId, out Dictionary<string, string> errors))
            {
                AddErrorsToModel(modelState, errors);
                return BadRequest(modelState);
            }
            return Json("");
        }

        [HttpGet("Leave/{id}")]
        public IActionResult Leave(string id)
        {
            ModelStateDictionary modelState = ModelState;
            if (!new OrganizationRepository(serviceProvider, context).TryLeaveOrg(id, out Dictionary<string, string> errors))
            {
                AddErrorsToModel(modelState, errors);
                return BadRequest(modelState);
            }
            return Json(typeof(OrganizationViewModel).Name.GetReturnUrl(Url));
        }

        [HttpGet("AcceptInvite/{id}")]
        public IActionResult AcceptInvite(string id)
        {
            ModelStateDictionary modelState = ModelState;
            if (!new OrganizationRepository(serviceProvider, context).TryAcceptInvite(id, out Dictionary<string, string> errors))
            {
                AddErrorsToModel(modelState, errors);
                return BadRequest(modelState);
            }
            return Json("");
        }

        [HttpGet("RejectInvite/{id}")]
        public IActionResult RejectInvite(string id)
        {
            new OrganizationRepository(serviceProvider, context).RejectInvite(id);
            return Json(typeof(OrganizationViewModel).Name.GetReturnUrl(Url));
        }

        [HttpPost("Search")]
        public IActionResult Search(OrganizationsViewModel orgViewModels)
        {
            cachService.CacheItem(currentUser.Id, ORGANIZATIONS, orgViewModels);
            return RedirectToAction(ORGANIZATIONS, ORGANIZATION, new { pageNumber = DEFAULT_MIN_PAGE_NUMBER });
        }

        [HttpGet("ClearSearch")]
        public IActionResult ClearSearch()
        {
            new OrganizationRepository(serviceProvider, context).ClearSearch();
            return RedirectToAction(ORGANIZATIONS, ORGANIZATION, new { pageNumber = DEFAULT_MIN_PAGE_NUMBER });
        }

        [HttpPost("SearchDivision")]
        public IActionResult SearchDivision(OrganizationViewModel orgViewModel)
        {
            cachService.CacheItem(currentUser.Id, DIVISIONS, orgViewModel);
            return RedirectToAction(ORGANIZATION, ORGANIZATION, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.OrganizationView) });
        }

        [HttpGet("ClearDivisionSearch")]
        public IActionResult ClearDivisionSearch()
        {
            new OrganizationRepository(serviceProvider, context).ClearDivisionSearch();
            return RedirectToAction(ORGANIZATION, ORGANIZATION, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.OrganizationView) });
        }

        [HttpPost("SearchPosition")]
        public IActionResult SearchPosition(OrganizationViewModel orgViewModel)
        {
            cachService.CacheItem(currentUser.Id, POSITIONS, orgViewModel);
            return RedirectToAction(ORGANIZATION, ORGANIZATION, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.OrganizationView) });
        }

        [HttpGet("ClearPositionSearch")]
        public IActionResult ClearPositionSearch()
        {
            new OrganizationRepository(serviceProvider, context).ClearPositionSearch();
            return RedirectToAction(ORGANIZATION, ORGANIZATION, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.OrganizationView) });
        }

        [HttpPost("SearchEmployee")]
        public IActionResult SearchEmployee(OrganizationViewModel orgViewModel)
        {
            cachService.CacheItem(currentUser.Id, EMPLOYEES, orgViewModel);
            return RedirectToAction(ORGANIZATION, ORGANIZATION, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.OrganizationView) });
        }

        [HttpGet("ClearEmployeeSearch")]
        public IActionResult ClearEmployeeSearch()
        {
            new OrganizationRepository(serviceProvider, context).ClearEmployeeSearch();
            return RedirectToAction(ORGANIZATION, ORGANIZATION, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.OrganizationView) });
        }

        [HttpPost("SearchResponsibility")]
        public IActionResult SearchResponsibility(OrganizationViewModel orgViewModel)
        {
            cachService.CacheItem(currentUser.Id, RESPONSIBILITIES, orgViewModel);
            return RedirectToAction(RESPONSIBILITIES, RESPONSIBILITY, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.OrganizationView) });
        }

        [HttpGet("ClearResponsibilitySearch")]
        public IActionResult ClearResponsibilitySearch()
        {
            new OrganizationRepository(serviceProvider, context).ClearResponsibilitySearch();
            return RedirectToAction(RESPONSIBILITIES, RESPONSIBILITY, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.OrganizationView) });
        }
    }
}
