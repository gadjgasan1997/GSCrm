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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;
using static GSCrm.Repository.OrganizationRepository;
using static GSCrm.Repository.EmployeeRepository;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(EMPLOYEE)]
    public class EmployeeController 
        : MainController<Employee, EmployeeViewModel, EmployeeValidator, EmployeeTransformer, EmployeeRepository>
    {
        private readonly UserManager<User> userManager;
        public EmployeeController(UserManager<User> userManager, ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager)
            : base(context, viewsInfo, resManager, new EmployeeTransformer(context, resManager), new EmployeeRepository(context, viewsInfo, resManager, null, userManager))
        {
            this.userManager = userManager;
        }

        [HttpGet("ListOfEmployees/{pageNumber}")]
        public IActionResult Employees(int pageNumber)
        {
            OrganizationViewModel orgViewModel = CurrentOrganization;
            User currentUser = context.Users.FirstOrDefault(n => n.UserName == User.Identity.Name);
            OrganizationRepository organizationRepository = new OrganizationRepository(context, viewsInfo, resManager, HttpContext);
            organizationRepository.SetViewInfo(currentUser.Id, EMPLOYEES, pageNumber);
            organizationRepository.AttachEmployees(orgViewModel);
            return View($"{ORG_VIEWS_REL_PATH}{ORGANIZATION}.cshtml", orgViewModel);
        }

        [HttpGet("{id}")]
        public ViewResult Employee(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid Id))
                return View("Error");

            Employee employee = context.Employees.FirstOrDefault(i => i.Id == Id);
            if (employee == null)
                return View("Error");

            EmployeeViewModel empViewModel = transformer.DataToViewModel(employee);
            empViewModel = new EmployeeTransformer(context, resManager, HttpContext).UpdateViewModelFromCash(empViewModel);
            repository = new EmployeeRepository(context, viewsInfo, resManager, HttpContext);
            repository.AttachPositions(empViewModel);
            repository.AttachContacts(empViewModel);
            repository.AttachSubordinates(empViewModel);
            CurrentEmployee = empViewModel;
            return View(EMPLOYEE, empViewModel);
        }

        [HttpGet("GetEmployees/{orgId}/{divNamePart}/{employeePart}")]
        public IActionResult GetEmployees(string orgId, string divNamePart, string employeePart)
        {
            if (!string.IsNullOrEmpty(divNamePart) && !string.IsNullOrEmpty(employeePart) && Guid.TryParse(orgId, out Guid guid))
            {
                Division selectedDivision = context.GetOrgDivisions(guid).FirstOrDefault(n => n.Name == divNamePart);
                if (selectedDivision == null) return Json("");

                List<EmployeeViewModel> employeeViewModels = selectedDivision.GetEmployees(context)
                    .TransformToViewModels<Employee, EmployeeViewModel, EmployeeTransformer>(
                        transformer: transformer,
                        limitingFunc: emp => emp.GetFullName().ToLower().Contains(employeePart));
                return Json(employeeViewModels);
            }
            return Json("");
        }

        [HttpPost("Create")]
        public override IActionResult Create(EmployeeViewModel employeeViewModel)
        {
            ModelStateDictionary modelState = ModelState;
            EmployeeRepository employeeRepository = new EmployeeRepository(
                context: context,
                viewsInfo: viewsInfo,
                resManager: resManager,
                httpContext: HttpContext,
                userManager: userManager);
            if (employeeRepository.TryCreate(ref employeeViewModel, modelState))
                return Json(Url.Action(EMPLOYEE, new { id = employeeRepository.newRecord.Id.ToString() }));
            return BadRequest(modelState);
        }

        [HttpPost("Update")]
        public override IActionResult Update(EmployeeViewModel employeeViewModel)
        {
            ModelStateDictionary modelState = ModelState;
            if (new EmployeeRepository(context, viewsInfo, resManager, HttpContext).TryUpdate(ref employeeViewModel, modelState))
                return Json(Url.Action(EMPLOYEE, EMPLOYEE, new { id = CurrentEmployee.Id }));
            return BadRequest(modelState);
        }

        [HttpPost("ChangeDivision")]
        public IActionResult ChangeDivision(EmployeeViewModel employeeViewModel)
        {
            ModelStateDictionary modelState = ModelState;
            if (new EmployeeRepository(context, viewsInfo, resManager, HttpContext).TryChangeDivision(employeeViewModel, modelState))
                return Json("");
            return BadRequest(modelState);
        }

        [HttpPost("Unlock")]
        public IActionResult Unlock(EmployeeViewModel employeeViewModel)
        {
            ModelStateDictionary modelState = ModelState;
            if (new EmployeeRepository(context, viewsInfo, resManager, HttpContext).TryUnlock(ref employeeViewModel, modelState))
                return View(EMPLOYEE, employeeViewModel);
            return View(EMPLOYEE, employeeViewModel);
        }

        [HttpPost("SearchPosition")]
        public IActionResult SearchPosition(EmployeeViewModel employeeViewModel)
        {
            new EmployeeRepository(context, viewsInfo, resManager, HttpContext).SearchPosition(employeeViewModel);
            return RedirectToAction(EMPLOYEE, EMPLOYEE, new { id = CurrentEmployee.Id });
        }

        [HttpGet("ClearPositionSearch")]
        public IActionResult ClearPositionSearch()
        {
            new EmployeeRepository(context, viewsInfo, resManager, HttpContext).ClearPositionSearch();
            return RedirectToAction(EMPLOYEE, EMPLOYEE, new { id = CurrentEmployee.Id });
        }

        [HttpPost("SearchContact")]
        public IActionResult SearchContact(EmployeeViewModel employeeViewModel)
        {
            new EmployeeRepository(context, viewsInfo, resManager, HttpContext).SearchContact(employeeViewModel);
            return RedirectToAction(EMPLOYEE, EMPLOYEE, new { id = CurrentEmployee.Id });
        }

        [HttpGet("ClearContactSearch")]
        public IActionResult ClearContactSearch()
        {
            new EmployeeRepository(context, viewsInfo, resManager, HttpContext).ClearContactSearch();
            return RedirectToAction(EMPLOYEE, EMPLOYEE, new { id = CurrentEmployee.Id });
        }

        [HttpPost("SearchSubordinate")]
        public IActionResult SearchSubordinate(EmployeeViewModel employeeViewModel)
        {
            new EmployeeRepository(context, viewsInfo, resManager, HttpContext).SearchSubordinate(employeeViewModel);
            return RedirectToAction(EMPLOYEE, EMPLOYEE, new { id = CurrentEmployee.Id });
        }

        [HttpGet("ClearSubordinateSearch")]
        public IActionResult ClearSubordinateSearch()
        {
            new EmployeeRepository(context, viewsInfo, resManager, HttpContext).ClearSubordinateSearch();
            return RedirectToAction(EMPLOYEE, EMPLOYEE, new { id = CurrentEmployee.Id });
        }
    }
}