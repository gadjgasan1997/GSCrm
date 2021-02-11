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
using GSCrm.Data.ApplicationInfo;
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

        [HttpGet("HasNoPermissionsForSee")]
        public IActionResult HasNoPermissionsForSee()
            => View($"{ORG_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new OrganizationViewModel());

        [HttpGet("{id}")]
        public ViewResult Organization()
        {
            Organization organization = (Organization)cachService.GetMainEntity(currentUser, MainEntityType.OrganizationData);
            OrganizationViewModel orgViewModel = map.DataToViewModel(organization);
            orgViewModel = new OrganizationMap(serviceProvider, context).Refresh(orgViewModel, currentUser, OrgAllViewTypes);
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            organizationRepository.AttachDivisions(orgViewModel);
            organizationRepository.AttachPositions(orgViewModel);
            organizationRepository.AttachEmployees(orgViewModel);
            organizationRepository.AttachResponsibilities(orgViewModel);
            cachService.SetCurrentView(currentUser.Id, ORGANIZATION);
            cachService.CacheOrganization(currentUser, organization, orgViewModel);
            return View(ORGANIZATION, orgViewModel);
        }

        /// <summary>
        /// Получить продуктовую модель организации
        /// </summary>
        /// <returns></returns>
        [HttpGet("ProductCategories/{id}")]
        public ViewResult ProductCategories()
        {
            // Если к организации есть доступ, то установка ее как текущей, на которой находится пользовательs
            OrganizationMap organizationMap = new OrganizationMap(serviceProvider, context);
            OrganizationViewModel orgViewModel = (OrganizationViewModel)cachService.GetMainEntity(currentUser, MainEntityType.OrganizationView);
            orgViewModel = organizationMap.Refresh(orgViewModel, currentUser, OrgAllViewTypes);
            cachService.CacheItem(currentUser.Id, "CurrentOrganizationView", orgViewModel);

            // Простановка текущего представления
            ProductCategoryRepository productCategoryRepository = new ProductCategoryRepository(serviceProvider, context);
            ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, PROD_CATS);
            productCategoryRepository.SetViewInfo(PROD_CATS, viewInfo.CurrentPageNumber);
            ProductCategoriesViewModel productCategoriesViewModel = new ProductCategoriesViewModel() { OrganizationViewModel = orgViewModel };
            cachService.CacheItem(currentUser.Id, PROD_CATS, productCategoriesViewModel);
            return View($"{PROD_CAT_VIEWS_REL_PATH}{PROD_CATS}.cshtml", productCategoriesViewModel);
        }

        /// <summary>
        /// Получить список подразделений организации
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("Divisions/{pageNumber}")]
        public IActionResult Divisions(int pageNumber)
        {
            OrganizationViewModel orgViewModel = (OrganizationViewModel)cachService.GetMainEntity(currentUser, MainEntityType.OrganizationView);
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            organizationRepository.SetViewInfo(DIVISIONS, pageNumber);
            organizationRepository.AttachDivisions(orgViewModel);
            return View($"{ORG_VIEWS_REL_PATH}{ORGANIZATION}.cshtml", orgViewModel);
        }

        /// <summary>
        /// Получить список должностей организации
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("Positions/{pageNumber}")]
        public IActionResult Positions(int pageNumber)
        {
            OrganizationViewModel orgViewModel = (OrganizationViewModel)cachService.GetMainEntity(currentUser, MainEntityType.OrganizationView);
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            organizationRepository.SetViewInfo(POSITIONS, pageNumber);
            organizationRepository.AttachPositions(orgViewModel);
            return View($"{ORG_VIEWS_REL_PATH}{ORGANIZATION}.cshtml", orgViewModel);
        }

        /// <summary>
        /// Получить список сотрудников организации
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("Employees/{pageNumber}")]
        public IActionResult Employees(int pageNumber)
        {
            OrganizationViewModel orgViewModel = (OrganizationViewModel)cachService.GetMainEntity(currentUser, MainEntityType.OrganizationView);
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            organizationRepository.SetViewInfo(EMPLOYEES, pageNumber);
            organizationRepository.AttachEmployees(orgViewModel);
            return View($"{ORG_VIEWS_REL_PATH}{ORGANIZATION}.cshtml", orgViewModel);
        }

        /// <summary>
        /// Получить список полномочий организации
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("Responsibilities/{pageNumber}")]
        public IActionResult Responsibilities(int pageNumber)
        {
            OrganizationViewModel orgViewModel = (OrganizationViewModel)cachService.GetMainEntity(currentUser, MainEntityType.OrganizationView);
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            organizationRepository.SetViewInfo(RESPONSIBILITIES, pageNumber);
            organizationRepository.AttachResponsibilities(orgViewModel);
            return Json(orgViewModel.Responsibilities);
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
            return RedirectToAction(ORGANIZATIONS, "Root", new { pageNumber = DEFAULT_MIN_PAGE_NUMBER });
        }

        [HttpGet("ClearSearch")]
        public IActionResult ClearSearch()
        {
            new OrganizationRepository(serviceProvider, context).ClearSearch();
            return RedirectToAction(ORGANIZATIONS, "Root", new { pageNumber = DEFAULT_MIN_PAGE_NUMBER });
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
            return RedirectToAction(RESPONSIBILITIES, ORGANIZATION, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.OrganizationView) });
        }

        [HttpGet("ClearResponsibilitySearch")]
        public IActionResult ClearResponsibilitySearch()
        {
            new OrganizationRepository(serviceProvider, context).ClearResponsibilitySearch();
            return RedirectToAction(RESPONSIBILITIES, ORGANIZATION, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.OrganizationView) });
        }
    }
}
