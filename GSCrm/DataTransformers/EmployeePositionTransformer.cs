using GSCrm.Data;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System.Linq;

namespace GSCrm.DataTransformers
{
    public class EmployeePositionTransformer : BaseTransformer<EmployeePosition, EmployeePositionViewModel>
    {
        public EmployeePositionTransformer(ApplicationDbContext context, ResManager resManager) : base(context, resManager) { }

        public override EmployeePositionViewModel DataToViewModel(EmployeePosition employeePosition)
        {
            Employee employee = context.Employees.FirstOrDefault(i => i.Id == employeePosition.EmployeeId);
            Position position = context.Positions.FirstOrDefault(i => i.Id == employeePosition.PositionId);
            Position parentPosition = context.Positions.FirstOrDefault(i => i.Id == position.ParentPositionId);
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
