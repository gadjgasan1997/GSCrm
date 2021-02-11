using GSCrm.Mapping;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using GSCrm.Data;
using GSCrm.Models.Enums;
using static GSCrm.CommonConsts;
using static GSCrm.Repository.EmployeeRepository;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(EMPLOYEE)]
    public class EmployeeController : MainController<Employee, EmployeeViewModel>
    {
        public EmployeeController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }

        [HttpGet("HasNoPermissionsForSee")]
        public IActionResult HasNoPermissionsForSee()
            => View($"{EMP_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new EmployeeViewModel());

        [HttpGet("{id}")]
        public ViewResult Employee()
        {
            Employee employee = cachService.GetMainEntity(currentUser, MainEntityType.EmployeeData) as Employee;
            EmployeeMap employeeMap = new EmployeeMap(serviceProvider, context);
            EmployeeViewModel empViewModel = employeeMap.DataToViewModel(employee);
            empViewModel = new EmployeeMap(serviceProvider, context).Refresh(empViewModel, currentUser, EmpBaseViewTypes);
            if (employee.EmployeeStatus == EmployeeStatus.Active)
            {
                EmployeeRepository employeeRepository = new EmployeeRepository(serviceProvider, context);
                employeeRepository.AttachPositions(empViewModel);
                employeeRepository.AttachContacts(empViewModel);
                employeeRepository.AttachSubordinates(empViewModel);
            }
            cachService.SetCurrentView(currentUser.Id, EMPLOYEE);
            cachService.CacheEmployee(currentUser, employee, empViewModel);
            return View(EMPLOYEE, empViewModel);
        }

        /// <summary>
        /// Получение списка подчиненных сотрудника
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("Subordinates/{pageNumber}")]
        public IActionResult Subordinates(int pageNumber)
        {
            EmployeeViewModel employeeViewModel = (EmployeeViewModel)cachService.GetMainEntity(currentUser, MainEntityType.EmployeeView);
            EmployeeRepository employeeRepository = new EmployeeRepository(serviceProvider, context);
            employeeRepository.SetViewInfo(EMP_SUBS, pageNumber);
            employeeRepository.AttachSubordinates(employeeViewModel);
            return View($"{EMP_VIEWS_REL_PATH}{EMPLOYEE}.cshtml", employeeViewModel);
        }

        /// <summary>
        /// Получение списка контактов сотрудника
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("Contacts/{pageNumber}")]
        public IActionResult Contacts(int pageNumber)
        {
            EmployeeViewModel employeeViewModel = (EmployeeViewModel)cachService.GetMainEntity(currentUser, MainEntityType.EmployeeView);
            EmployeeRepository employeeRepository = new EmployeeRepository(serviceProvider, context);
            employeeRepository.SetViewInfo(EMP_CONTACTS, pageNumber);
            employeeRepository.AttachContacts(employeeViewModel);
            return View($"{EMP_VIEWS_REL_PATH}{EMPLOYEE}.cshtml", employeeViewModel);
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