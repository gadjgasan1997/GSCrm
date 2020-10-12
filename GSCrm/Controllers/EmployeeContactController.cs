using GSCrm.Data;
using GSCrm.Data.ApplicationInfo;
using GSCrm.DataTransformers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using GSCrm.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using static GSCrm.CommonConsts;
using static GSCrm.Repository.EmployeeRepository;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(EMP_CONTACT)]
    public class EmployeeContactController 
        : MainController<EmployeeContact, EmployeeContactViewModel, EmployeeContactValidator, EmployeeContactTransformer, EmployeeContactRepository>
    {
        public EmployeeContactController(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager)
            : base(context, viewsInfo, resManager, new EmployeeContactTransformer(context, resManager), new EmployeeContactRepository(context, viewsInfo, resManager))
        { }

        [HttpGet("ListOfContacts/{pageNumber}")]
        public IActionResult Contacts(int pageNumber)
        {
            User currentUser = context.Users.FirstOrDefault(n => n.UserName == User.Identity.Name);
            EmployeeViewModel employeeViewModel = CurrentEmployee;
            EmployeeRepository employeeRepository = new EmployeeRepository(context, viewsInfo, resManager, HttpContext);
            employeeRepository.SetViewInfo(currentUser.Id, EMP_CONTACTS, pageNumber);
            employeeRepository.AttachContacts(employeeViewModel);
            return View($"{EMP_VIEWS_REL_PATH}{EMPLOYEE}.cshtml", employeeViewModel);
        }

        [HttpGet(CONTACT)]
        public IActionResult Contact(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid Id))
                return View("Error");

            EmployeeContact employeeContact = context.EmployeeContacts.FirstOrDefault(i => i.Id == Id);
            if (employeeContact == null)
                return View("Error");

            return Json(transformer.DataToViewModel(employeeContact));
        }
    }
}
