using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using System.Linq;
using GSCrm.Data;
using Microsoft.EntityFrameworkCore;

namespace GSCrm.Mapping
{
    public class EmployeePositionMap : BaseMap<EmployeePosition, EmployeePositionViewModel>
    {
        public EmployeePositionMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        public override EmployeePositionViewModel DataToViewModel(EmployeePosition employeePosition)
        {
            Employee employee = context.Employees.AsNoTracking().FirstOrDefault(i => i.Id == employeePosition.EmployeeId);
            Position position = context.Positions.AsNoTracking().FirstOrDefault(i => i.Id == employeePosition.PositionId);
            Position parentPosition = context.Positions.AsNoTracking().FirstOrDefault(i => i.Id == position.ParentPositionId);
            return new EmployeePositionViewModel()
            {
                Id = employeePosition.Id,
                PositionId = position.Id,
                PositionName = position.Name,
                ParentPositionId = parentPosition?.Id,
                ParentPositionName = parentPosition?.Name,
                IsPrimary = employee.PrimaryPositionId == position.Id
            };
        }
    }
}
