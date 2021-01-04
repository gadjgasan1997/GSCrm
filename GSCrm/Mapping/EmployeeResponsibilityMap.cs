using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using GSCrm.Data;

namespace GSCrm.Mapping
{
    public class EmployeeResponsibilityMap : BaseMap<EmployeeResponsibility, EmployeeResponsibilityViewModel>
    {
        public EmployeeResponsibilityMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }
    }
}
