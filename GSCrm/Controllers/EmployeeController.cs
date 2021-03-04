using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using GSCrm.Data;
using static GSCrm.CommonConsts;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(EMPLOYEE)]
    public class EmployeeController : MainController<Employee, EmployeeViewModel>
    {
        public EmployeeController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }

        [HttpGet("{id}")]
        public IActionResult Employee(string id) => GetEmployee(id);

        #region Child Entities
        /// <summary>
        /// Получение списка подчиненных сотрудника
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("{id}/Subordinates/{pageNumber}")]
        public IActionResult Subordinates(string id, int pageNumber)
        {
            repository.SetViewInfo(id, EMP_SUBS, pageNumber);
            return GetEmployee(id);
        }

        /// <summary>
        /// Получение списка контактов сотрудника
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("{id}/Contacts/{pageNumber}")]
        public IActionResult Contacts(string id, int pageNumber)
        {
            repository.SetViewInfo(id, EMP_CONTACTS, pageNumber);
            return GetEmployee(id);
        }
        #endregion

        #region Actions
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
        #endregion

        #region Searching
        [HttpPost("SearchContact")]
        public IActionResult SearchContact(EmployeeViewModel employeeViewModel)
        {
            new EmployeeRepository(serviceProvider, context).SearchContact(employeeViewModel);
            return RedirectToAction(EMPLOYEE, EMPLOYEE, new { id = employeeViewModel.Id });
        }

        [HttpGet("{id}/ClearContactSearch")]
        public IActionResult ClearContactSearch(string id)
        {
            new EmployeeRepository(serviceProvider, context).ClearContactSearch();
            return RedirectToAction(EMPLOYEE, EMPLOYEE, new { id });
        }

        [HttpPost("SearchSubordinate")]
        public IActionResult SearchSubordinate(EmployeeViewModel employeeViewModel)
        {
            new EmployeeRepository(serviceProvider, context).SearchSubordinate(employeeViewModel);
            return RedirectToAction(EMPLOYEE, EMPLOYEE, new { id = employeeViewModel.Id });
        }

        [HttpGet("{id}/ClearSubordinateSearch")]
        public IActionResult ClearSubordinateSearch(string id)
        {
            new EmployeeRepository(serviceProvider, context).ClearSubordinateSearch();
            return RedirectToAction(EMPLOYEE, EMPLOYEE, new { id });
        }
        #endregion

        #region Addition Methods
        private IActionResult GetEmployee(string employeeId)
        {
            if (cachService.TryGetCachedEntity(currentUser, employeeId, out Employee employee))
                return View(EMPLOYEE, repository.LoadView(employee));
            return View("Error");
        }
        #endregion
    }
}