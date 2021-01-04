using System.Collections.Generic;

namespace GSCrm.Models.ViewModels
{
    public class OrganizationViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string OwnerId { get; set; }
        public string SearchDivName { get; set; }
        public string SearchParentDivName { get; set; }
        public string SearchPosName { get; set; }
        public string SeacrhPositionDivName { get; set; }
        public string SearchParentPosName { get; set; }
        public string SearchPrimaryEmployeeName { get; set; }
        public string SearchEmployeeName { get; set; }
        public string SearchEmployeePrimaryPosName { get; set; }
        public string SeacrhEmployeeDivName { get; set; }
        public string SeacrhResponsibilityName { get; set; }
        public bool Accepted { get; set; }
        public IEnumerable<DivisionViewModel> Divisions { get; set; }
        public IEnumerable<PositionViewModel> Positions { get; set; }
        public IEnumerable<EmployeeViewModel> Employees { get; set; }
        public IEnumerable<ResponsibilityViewModel> Responsibilities { get; set; }
    }
}
