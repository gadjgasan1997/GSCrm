using GSCrm.Data;
using GSCrm.Mapping;
using GSCrm.Models;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using GSCrm.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using static GSCrm.CommonConsts;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(EMP_CONTACT)]
    public class EmployeeContactController 
        : MainController<EmployeeContact, EmployeeContactViewModel>
    {
        public EmployeeContactController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }

        [HttpGet("ListOfContacts/{pageNumber}")]
        public IActionResult Contacts(int pageNumber)
        {
            EmployeeViewModel employeeViewModel = (EmployeeViewModel)cachService.GetMainEntity(currentUser, MainEntityType.EmployeeView);
            EmployeeRepository employeeRepository = new EmployeeRepository(serviceProvider, context);
            employeeRepository.SetViewInfo(EMP_CONTACTS, pageNumber);
            employeeRepository.AttachContacts(employeeViewModel);
            return View($"{EMP_VIEWS_REL_PATH}{EMPLOYEE}.cshtml", employeeViewModel);
        }

        [HttpGet(CONTACT)]
        public IActionResult Contact(string id)
        {
            if (!repository.TryGetItemById(id, out EmployeeContact employeeContact))
                return View("Error");
            return Json(map.DataToViewModel(employeeContact));
        }
    }
}
