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
    [Route(EMP_SUB)]
    public class EmployeeSubordinateController
        : MainController<Employee, EmployeeViewModel>
    {
        public EmployeeSubordinateController(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(context, serviceProvider)
        { }

        [HttpGet("ListOfSubordinates/{pageNumber}")]
        public IActionResult Subordinates(int pageNumber)
        {
            User currentUser = context.Users.FirstOrDefault(n => n.UserName == User.Identity.Name);
            EmployeeViewModel employeeViewModel = (EmployeeViewModel)cachService.GetMainEntity(currentUser, MainEntityType.EmployeeView);
            EmployeeRepository employeeRepository = new EmployeeRepository(serviceProvider, context);
            employeeRepository.SetViewInfo(EMP_SUBS, pageNumber);
            employeeRepository.AttachSubordinates(employeeViewModel);
            return View($"{EMP_VIEWS_REL_PATH}{EMPLOYEE}.cshtml", employeeViewModel);
        }
    }
}
