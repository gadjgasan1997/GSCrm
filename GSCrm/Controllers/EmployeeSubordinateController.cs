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
    [Route(EMP_SUB)]
    public class EmployeeSubordinateController
        : MainController<Employee, EmployeeViewModel, EmployeeValidator, EmployeeTransformer, EmployeeRepository>
    {
        public EmployeeSubordinateController(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager)
            : base(context, viewsInfo, resManager, new EmployeeTransformer(context, resManager), new EmployeeRepository(context, viewsInfo, resManager))
        { }

        [HttpGet("ListOfSubordinates/{pageNumber}")]
        public IActionResult Subordinates(int pageNumber)
        {
            User currentUser = context.Users.FirstOrDefault(n => n.UserName == User.Identity.Name);
            EmployeeViewModel employeeViewModel = CurrentEmployee;
            repository = new EmployeeRepository(context, viewsInfo, resManager, HttpContext);
            repository.SetViewInfo(currentUser.Id, EMP_SUBS, pageNumber);
            repository.AttachSubordinates(employeeViewModel);
            return View($"{EMP_VIEWS_REL_PATH}{EMPLOYEE}.cshtml", employeeViewModel);
        }
    }
}
