using GSCrm.Mapping;
using GSCrm.Helpers;
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
using GSCrm.Data;
using GSCrm.Models.Enums;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(DIVISION)]
    public class DivisionController
        : MainController<Division, DivisionViewModel>
    {
        public DivisionController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }

        [HttpGet("ListOfDivisions/{pageNumber}")]
        public IActionResult Divisions(int pageNumber)
        {
            OrganizationViewModel orgViewModel = (OrganizationViewModel)cachService.GetMainEntity(currentUser, MainEntityType.OrganizationView);
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            organizationRepository.SetViewInfo(DIVISIONS, pageNumber);
            organizationRepository.AttachDivisions(orgViewModel);
            return View($"{ORG_VIEWS_REL_PATH}{ORGANIZATION}.cshtml", orgViewModel);
        }

        [HttpGet("{id}")]
        public ViewResult Division(string id)
        {
            if (!repository.TryGetItemById(id, out Division division))
                return View("Error");
            return View($"{DIV_VIEWS_REL_PATH}{DIVISION}.cshtml", map.DataToViewModel(division));
        }
    }
}
