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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using static GSCrm.CommonConsts;
using static GSCrm.Repository.OrganizationRepository;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(ORGANIZATION)]
    public class OrganizationController
        : MainController<Organization, OrganizationViewModel, OrganizationValidatior, OrganizationTransformer, OrganizationRepository>
    {
        public OrganizationController(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager)
            : base(context, viewsInfo, resManager, new OrganizationTransformer(context, resManager), new OrganizationRepository(context, viewsInfo, resManager))
        { }

        [HttpGet("ListOfOrganizations/{pageNumber}")]
        public ViewResult Organizations(int pageNumber)
        {
            User currentUser = context.Users.FirstOrDefault(n => n.UserName == User.Identity.Name);
            OrganizationsViewModel orgsViewModel = ModelCash<OrganizationsViewModel>.GetViewModel(currentUser.Id, ORGANIZATIONS);
            repository = new OrganizationRepository(context, viewsInfo, resManager, HttpContext);
            repository.SetViewInfo(currentUser.Id, ORGANIZATIONS, pageNumber);
            orgsViewModel.SearchName = orgsViewModel.SearchNameCash.GetValueOrDefault(currentUser.Id);
            repository.AttachOrganizations(ref orgsViewModel);
            return View(ORGANIZATIONS, orgsViewModel);
        }

        [HttpGet("{id}")]
        public ViewResult Organization(string id)
        {
            repository = new OrganizationRepository(context, viewsInfo, resManager, HttpContext);
            if (!repository.TryGetItemById(id, out Organization organization))
                return View("Error");

            OrganizationViewModel orgViewModel = transformer.DataToViewModel(organization);
            orgViewModel = new OrganizationTransformer(context, resManager, HttpContext).UpdateViewModelFromCash(orgViewModel);
            repository.AttachDivisions(orgViewModel);
            repository.AttachPositions(orgViewModel);
            repository.AttachEmployees(orgViewModel);
            //repository.AttachResponsibilities(orgViewModel);
            CurrentOrganization = orgViewModel;
            return View(ORGANIZATION, orgViewModel);
        }

        [HttpGet("BackToOrganization/{orgId}")]
        public IActionResult BackToOrganization(string orgId)
        {
            if (!string.IsNullOrEmpty(orgId) && Guid.TryParse(orgId, out Guid organizationId))
                return RedirectToAction(ORGANIZATION, ORGANIZATION, new { id = organizationId });
            return View("Error");
        }

        [HttpGet("GetAccounts/{accountPart}")]
        public IActionResult GetAccounts(string accountPart)
        {
            if (!string.IsNullOrEmpty(accountPart))
            {
                User currentUser = context.Users.FirstOrDefault(n => n.UserName == User.Identity.Name);
                /*if (currentUser.PrimaryOrganizationId != null)
                {
                    List<AccountViewModel> accountViewModels = context.GetOrgAccounts(currentUser.PrimaryOrganizationId)
                        .TransformToViewModels<Account, AccountViewModel, AccountTransformer>(
                            transformer: new AccountTransformer(context, resManager),
                            limitingFunc: acc => acc.Name.ToLower().Contains(accountPart.ToLower().TrimStartAndEnd()));
                    return Json(accountViewModels);
                }*/
                return Json("");
            }
            return Json("");
        }

        [HttpGet("GetEmployees/{orgId}/{employeePart}")]
        public IActionResult GetEmployees(string orgId, string employeePart)
        {
            if (!string.IsNullOrEmpty(orgId) && !string.IsNullOrEmpty(employeePart) && Guid.TryParse(orgId, out Guid organizationId))
                return Json(repository.GetOrgEmployeeViewModels(organizationId, employeePart));
            return Json("");
        }

        [HttpGet("GetEmployees/{employeePart}")]
        public IActionResult GetEmployees(string employeePart)
        {
            if (!string.IsNullOrEmpty(employeePart))
            {
                User currentUser = context.Users.FirstOrDefault(n => n.UserName == User.Identity.Name);
                if (currentUser.PrimaryOrganizationId != null)
                    return Json(repository.GetOrgEmployeeViewModels(currentUser.PrimaryOrganizationId, employeePart));
                return Json("");
            }
            return Json("");
        }

        [HttpPost("Create")]
        public override IActionResult Create(OrganizationViewModel orgViewModel)
        {
            ModelStateDictionary modelState = ModelState;
            OrganizationRepository organizationRepository = new OrganizationRepository(context, viewsInfo, resManager, HttpContext);
            if (organizationRepository.TryCreate(ref orgViewModel, modelState))
                return Json(Url.Action(ORGANIZATION, new { id = organizationRepository.newRecord.Id.ToString() }));
            return BadRequest(modelState);
        }

        [HttpPost("Search")]
        public IActionResult Search(OrganizationsViewModel orgViewModels)
        {
            new OrganizationRepository(context, viewsInfo, resManager, HttpContext).Search(orgViewModels);
            return RedirectToAction(ORGANIZATIONS, ORGANIZATION, new { pageNumber = DEFAULT_MIN_PAGE_NUMBER });
        }

        [HttpGet("ClearSearch")]
        public IActionResult ClearSearch()
        {
            new OrganizationRepository(context, viewsInfo, resManager, HttpContext).ClearSearch();
            return RedirectToAction(ORGANIZATIONS, ORGANIZATION, new { pageNumber = DEFAULT_MIN_PAGE_NUMBER });
        }

        [HttpPost("SearchDivision")]
        public IActionResult SearchDivision(OrganizationViewModel orgViewModel)
        {
            new OrganizationRepository(context, viewsInfo, resManager, HttpContext).SearchDivision(orgViewModel);
            return RedirectToAction(ORGANIZATION, ORGANIZATION, new { id = CurrentOrganization.Id });
        }

        [HttpGet("ClearDivisionSearch")]
        public IActionResult ClearDivisionSearch()
        {
            new OrganizationRepository(context, viewsInfo, resManager, HttpContext).ClearDivisionSearch();
            return RedirectToAction(ORGANIZATION, ORGANIZATION, new { id = CurrentOrganization.Id });
        }

        [HttpPost("SearchPosition")]
        public IActionResult SearchPosition(OrganizationViewModel orgViewModel)
        {
            new OrganizationRepository(context, viewsInfo, resManager, HttpContext).SearchPosition(orgViewModel);
            return RedirectToAction(ORGANIZATION, ORGANIZATION, new { id = CurrentOrganization.Id });
        }

        [HttpGet("ClearPositionSearch")]
        public IActionResult ClearPositionSearch()
        {
            new OrganizationRepository(context, viewsInfo, resManager, HttpContext).ClearPositionSearch();
            return RedirectToAction(ORGANIZATION, ORGANIZATION, new { id = CurrentOrganization.Id });
        }

        [HttpPost("SearchEmployee")]
        public IActionResult SearchEmployee(OrganizationViewModel orgViewModel)
        {
            new OrganizationRepository(context, viewsInfo, resManager, HttpContext).SearchEmployee(orgViewModel);
            return RedirectToAction(ORGANIZATION, ORGANIZATION, new { id = CurrentOrganization.Id });
        }

        [HttpGet("ClearEmployeeSearch")]
        public IActionResult ClearEmployeeSearch()
        {
            new OrganizationRepository(context, viewsInfo, resManager, HttpContext).ClearEmployeeSearch();
            return RedirectToAction(ORGANIZATION, ORGANIZATION, new { id = CurrentOrganization.Id });
        }

        /*[HttpPost("SearchResponsibility")]
        public IActionResult SearchResponsibility(OrganizationViewModel orgViewModel)
        {
            new OrganizationRepository(context, viewsInfo, resManager, HttpContext).SearchResponsibility(orgViewModel);
            return RedirectToAction(ORGANIZATION, ORGANIZATION, new { id = CurrentOrganization.Id });
        }

        [HttpGet("ClearResponsibilitySearch")]
        public IActionResult ClearResponsibilitySearch()
        {
            new OrganizationRepository(context, viewsInfo, resManager, HttpContext).ClearResponsibilitySearch();
            return RedirectToAction(ORGANIZATION, ORGANIZATION, new { id = CurrentOrganization.Id });
        }*/
    }
}
