using GSCrm.Mapping;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using static GSCrm.CommonConsts;
using static GSCrm.Repository.EmployeeRepository;
using GSCrm.Data;
using GSCrm.Models.Enums;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(EMPLOYEE)]
    public class EmployeeController : MainController<Employee, EmployeeViewModel>
    {
        public EmployeeController(UserManager<User> userManager, IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }

        [HttpGet("ListOfEmployees/{pageNumber}")]
        public IActionResult Employees(int pageNumber)
        {
            OrganizationViewModel orgViewModel = (OrganizationViewModel)cachService.GetMainEntity(currentUser, MainEntityType.OrganizationView);
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            organizationRepository.SetViewInfo(EMPLOYEES, pageNumber);
            organizationRepository.AttachEmployees(orgViewModel);
            return View($"{ORG_VIEWS_REL_PATH}{ORGANIZATION}.cshtml", orgViewModel);
        }

        [HttpGet("{id}")]
        public ViewResult Employee(string id)
        {
            // Проверки на наличие сотрудника и доступа к нему у пользователя
            EmployeeRepository employeeRepository = new EmployeeRepository(serviceProvider, context);
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            if (!employeeRepository.TryGetItemById(id, out Employee employee))
                return View($"{EMP_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new EmployeeViewModel());
            if (!organizationRepository.HasPermissionsForSeeItem(employee.GetOrganization(context)))
                return View($"{ORG_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new OrganizationViewModel());
            if (!employeeRepository.HasPermissionsForSeeItem(employee))
                return View($"{EMP_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new EmployeeViewModel());

            // Если сотрудник и доступ имеются
            EmployeeMap employeeMap = new EmployeeMap(serviceProvider, context);
            EmployeeViewModel empViewModel = employeeMap.DataToViewModel(employee);
            empViewModel = new EmployeeMap(serviceProvider, context).Refresh(empViewModel, currentUser, EmpBaseViewTypes);
            if (employee.EmployeeStatus == EmployeeStatus.Active)
            {
                employeeRepository.AttachPositions(empViewModel);
                employeeRepository.AttachContacts(empViewModel);
                employeeRepository.AttachSubordinates(empViewModel);
            }
            cachService.SetCurrentView(currentUser.Id, EMPLOYEE);
            cachService.CacheEmployee(currentUser, employee, empViewModel);
            return View(EMPLOYEE, empViewModel);
        }

        [HttpPost("ChangeDivision")]
        public IActionResult ChangeDivision(EmployeeViewModel employeeViewModel)
        {
            ModelStateDictionary modelState = ModelState;
            if (!new EmployeeRepository(serviceProvider, context).TryChangeDivision(employeeViewModel, out Dictionary<string, string> errors))
            {
                AddErrorsToModel(modelState, errors);
                return BadRequest(modelState);
            }
            return Json("");
        }

        [HttpPost("Unlock")]
        public IActionResult Unlock(EmployeeViewModel employeeViewModel)
        {
            ModelStateDictionary modelState = ModelState;
            if (!new EmployeeRepository(serviceProvider, context).TryUnlock(ref employeeViewModel, out Dictionary<string, string> errors))
            {
                AddErrorsToModel(modelState, errors);
                return BadRequest(modelState);
            }
            return Json("");
        }

        [HttpPost("SearchPosition")]
        public IActionResult SearchPosition(EmployeeViewModel employeeViewModel)
        {
            cachService.CacheItem(currentUser.Id, EMP_POSITIONS, employeeViewModel);
            return RedirectToAction(EMPLOYEE, EMPLOYEE, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.EmployeeView) });
        }

        [HttpGet("ClearPositionSearch")]
        public IActionResult ClearPositionSearch()
        {
            new EmployeeRepository(serviceProvider, context).ClearPositionSearch();
            return RedirectToAction(EMPLOYEE, EMPLOYEE, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.EmployeeView) });
        }

        [HttpPost("SearchContact")]
        public IActionResult SearchContact(EmployeeViewModel employeeViewModel)
        {
            cachService.CacheItem(currentUser.Id, EMP_CONTACTS, employeeViewModel);
            return RedirectToAction(EMPLOYEE, EMPLOYEE, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.EmployeeView) });
        }

        [HttpGet("ClearContactSearch")]
        public IActionResult ClearContactSearch()
        {
            new EmployeeRepository(serviceProvider, context).ClearContactSearch();
            return RedirectToAction(EMPLOYEE, EMPLOYEE, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.EmployeeView) });
        }

        [HttpPost("SearchSubordinate")]
        public IActionResult SearchSubordinate(EmployeeViewModel employeeViewModel)
        {
            cachService.CacheItem(currentUser.Id, EMP_SUBS, employeeViewModel);
            return RedirectToAction(EMPLOYEE, EMPLOYEE, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.EmployeeView) });
        }

        [HttpGet("ClearSubordinateSearch")]
        public IActionResult ClearSubordinateSearch()
        {
            new EmployeeRepository(serviceProvider, context).ClearSubordinateSearch();
            return RedirectToAction(EMPLOYEE, EMPLOYEE, new { id = cachService.GetMainEntityId(currentUser, MainEntityType.EmployeeView) });
        }
    }
}