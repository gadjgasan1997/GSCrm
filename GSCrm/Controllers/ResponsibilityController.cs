using GSCrm.Data;
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
        /// <summary>
        /// Количество одновременно отоброжаемых полномочий организации
        /// Уровень доступа public, чтобы можно бьыло обратиться из представления
        /// </summary>
        public const int RESPONSIBILITIES_COUNT = 5;

        public ResponsibilityController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }

        [HttpGet("Responsibilities/{pageNumber}")]
        public IActionResult Responsibilities(int pageNumber)
        {
            OrganizationViewModel orgViewModel = (OrganizationViewModel)cachService.GetMainEntity(currentUser, MainEntityType.OrganizationView);
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            organizationRepository.SetViewInfo(RESPONSIBILITIES, pageNumber);
            organizationRepository.AttachResponsibilities(orgViewModel);
            return Json(orgViewModel.Responsibilities);
        }

        [HttpGet("{id}")]
        public ViewResult Responsibility(string id)
        {
            // Проверки на наличие полномочия и доступа к нему у пользователя
            ResponsibilityRepository responsibilityRepository = new ResponsibilityRepository(serviceProvider, context);
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            if (!responsibilityRepository.TryGetItemById(id, out Responsibility responsibility))
                return View($"{RESP_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new ResponsibilityViewModel());
            if (!organizationRepository.HasPermissionsForSeeItem(responsibility.GetOrganization(context)))
                return View($"{ORG_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new OrganizationViewModel());
            if (!responsibilityRepository.HasPermissionsForSeeItem(responsibility))
                return View($"{RESP_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new ResponsibilityViewModel());

            // Если полномочие и доступ имеются
            ResponsibilityViewModel respViewModel = new ResponsibilityMap(serviceProvider, context).DataToViewModel(responsibility);
            cachService.SetCurrentViewName(currentUser.Id, RESPONSIBILITY);
            cachService.CacheResponsibility(currentUser, responsibility, respViewModel);
            return View(RESPONSIBILITY, respViewModel);
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
