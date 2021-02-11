﻿using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("{id}")]
        public IActionResult Contact(string id)
        {
            if (!repository.TryGetItemById(id, out EmployeeContact employeeContact))
                return View("Error");
            return Json(map.DataToViewModel(employeeContact));
        }
    }
}
