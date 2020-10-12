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
        public Dictionary<string, string> SearchDivNameCash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SearchParentDivNameCash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SearchPosNameCash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SeacrhPositionDivNameCash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SearchParentPosNameCash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SearchPrimaryEmployeeNameCash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SearchEmployeeNameCash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SearchEmployeePrimaryPosNameCash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SeacrhEmployeeDivNameCash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SeacrhResponsibilityNameCash { get; set; } = new Dictionary<string, string>();
        public IEnumerable<DivisionViewModel> Divisions { get; set; }
        public IEnumerable<PositionViewModel> Positions { get; set; }
        public IEnumerable<EmployeeViewModel> Employees { get; set; }
        public IEnumerable<ResponsibilityViewModel> Responsibilities { get; set; }
    }
}
