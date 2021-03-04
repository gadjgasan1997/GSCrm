using System;
using System.Collections.Generic;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Repository;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using GSCrm.Data.ApplicationInfo;
using static GSCrm.CommonConsts;

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

        [HttpGet("{id}")]
        public IActionResult Organization(string id) => GetOrganization(id);

        #region Child Entities
        /// <summary>
        /// Получить список подразделений организации
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("{id}/Divisions/{pageNumber}")]
        public IActionResult Divisions(string id, int pageNumber)
        {
            repository.SetViewInfo(id, DIVISIONS, pageNumber);
            return GetOrganization(id);
        }

        /// <summary>
        /// Получить список должностей организации
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("{id}/Positions/{pageNumber}")]
        public IActionResult Positions(string id, int pageNumber)
        {
            repository.SetViewInfo(id, POSITIONS, pageNumber);
            return GetOrganization(id);
        }

        /// <summary>
        /// Получить список сотрудников организации
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("{id}/Employees/{pageNumber}")]
        public IActionResult Employees(string id, int pageNumber)
        {
            repository.SetViewInfo(id, EMPLOYEES, pageNumber);
            return GetOrganization(id);
        }

        /// <summary>
        /// Получить список полномочий организации
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("{id}/Responsibilities/{pageNumber}")]
        public IActionResult Responsibilities(string id, int pageNumber)
        {
            repository.SetViewInfo(id, RESPONSIBILITIES, pageNumber);
            return GetResponsibilities();
        }

        /// <summary>
        /// Метод загружает и возвращает список полномочий
        /// Должен быть публичным, так как дергается из <see cref="ResponsibilityController"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/GetResponsibilities")]
        public IActionResult GetResponsibilities()
        {
            Organization organization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
            OrganizationViewModel orgViewModel = repository.LoadView(organization);
            return Json(orgViewModel.Responsibilities);
        }

        /// <summary>
        /// Получить продуктовую модель организации
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/ProductCategories")]
        public ViewResult ProductCategories()
            => View($"{PROD_CAT_VIEWS_REL_PATH}{PROD_CATS}.cshtml", cachService.GetCachedCurrentEntity<ProductCategoriesViewModel>(currentUser));
        #endregion

        #region Searching
        [HttpPost("SearchDivision")]
        public IActionResult SearchDivision(OrganizationViewModel orgViewModel)
        {
            new OrganizationRepository(serviceProvider, context).SearchDivision(orgViewModel);
            return RedirectToAction(ORGANIZATION, ORGANIZATION, new { id = orgViewModel.Id });
        }

        [HttpGet("{id}/ClearDivisionSearch")]
        public IActionResult ClearDivisionSearch(string id)
        {
            new OrganizationRepository(serviceProvider, context).ClearDivisionSearch();
            return RedirectToAction(ORGANIZATION, ORGANIZATION, new { id });
        }

        [HttpPost("SearchPosition")]
        public IActionResult SearchPosition(OrganizationViewModel orgViewModel)
        {
            new OrganizationRepository(serviceProvider, context).SearchPosition(orgViewModel);
            return RedirectToAction(ORGANIZATION, ORGANIZATION, new { id = orgViewModel.Id });
        }

        [HttpGet("{id}/ClearPositionSearch")]
        public IActionResult ClearPositionSearch(string id)
        {
            new OrganizationRepository(serviceProvider, context).ClearPositionSearch();
            return RedirectToAction(ORGANIZATION, ORGANIZATION, new { id });
        }

        [HttpPost("SearchEmployee")]
        public IActionResult SearchEmployee(OrganizationViewModel orgViewModel)
        {
            new OrganizationRepository(serviceProvider, context).SearchEmployee(orgViewModel);
            return RedirectToAction(ORGANIZATION, ORGANIZATION, new { id = orgViewModel.Id });
        }

        [HttpGet("{id}/ClearEmployeeSearch")]
        public IActionResult ClearEmployeeSearch(string id)
        {
            new OrganizationRepository(serviceProvider, context).ClearEmployeeSearch();
            return RedirectToAction(ORGANIZATION, ORGANIZATION, new { id });
        }

        [HttpPost("SearchResponsibility")]
        public IActionResult SearchResponsibility(OrganizationViewModel orgViewModel)
        {
            new OrganizationRepository(serviceProvider, context).SearchResponsibility(orgViewModel);
            return Redirect($"/{ORGANIZATION}/{orgViewModel.Id}/{RESPONSIBILITIES}/0/");
        }

        [HttpGet("{id}/ClearResponsibilitySearch")]
        public IActionResult ClearResponsibilitySearch(string id)
        {
            new OrganizationRepository(serviceProvider, context).ClearResponsibilitySearch();
            return Redirect($"/{ORGANIZATION}/{id}/{RESPONSIBILITIES}/0/");
        }

        [HttpPost("SearchProductCategories")]
        public IActionResult SearchProductCategories(ProductCategoriesViewModel prodCatsViewModel)
        {
            new ProductCategoryRepository(serviceProvider, context).Search(prodCatsViewModel);
            return Redirect($"/{ORGANIZATION}/{prodCatsViewModel.OrganizationId}/GetProductCategoriesData/0/");
        }

        [HttpGet("{id}/ProductCategoriesClearSearch")]
        public IActionResult ProductCategoriesClearSearch(string id)
        {
            new ProductCategoryRepository(serviceProvider, context).ClearSearch();
            return Redirect($"/{ORGANIZATION}/{id}/GetProductCategoriesData/0/");
        }
        #endregion

        #region Actions
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

        [HttpGet("{id}/Leave")]
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

        [HttpGet("{id}/AcceptInvite")]
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

        [HttpGet("{id}/RejectInvite")]
        public IActionResult RejectInvite(string id)
        {
            new OrganizationRepository(serviceProvider, context).RejectInvite(id);
            return Json(typeof(OrganizationViewModel).Name.GetReturnUrl(Url));
        }

        [HttpGet("{id}/GetProductCategoriesData/{pageNumber}")]
        public IActionResult GetProductCategoriesData(string id, int pageNumber)
        {
            ProductCategoriesViewModel prodCatsCached = cachService.GetCachedCurrentEntity<ProductCategoriesViewModel>(currentUser);
            ProductCategoryRepository productCategoryRepository = new ProductCategoryRepository(serviceProvider, context);
            productCategoryRepository.SetViewInfo(id, PROD_CATS, pageNumber);
            productCategoryRepository.AttachProductCategories(prodCatsCached);
            return Json(prodCatsCached, serializerSettings);
        }
        #endregion

        #region Addition Methods
        /// <summary>
        /// Метод загружает и возвращает организацию
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        private IActionResult GetOrganization(string organizationId)
        {
            if (cachService.TryGetCachedEntity(currentUser, organizationId, out Organization organization))
                return View(ORGANIZATION, repository.LoadView(organization));
            return View("Error");
        }
        #endregion
    }
}
