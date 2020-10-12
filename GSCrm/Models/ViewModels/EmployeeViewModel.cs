using System;
using System.Collections.Generic;

namespace GSCrm.Models.ViewModels
{
    public class EmployeeViewModel : BaseViewModel
    {
        public string UserId { get; set; }
        public bool UserAccountExists { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FullName { get; set; }
        public string FullInitialName { get; set; }
        public string SupervisorId { get; set; }
        public string SupervisorInitialName { get; set; }
        public Guid DivisionId { get; set; }
        public string DivisionName { get; set; }
        public Guid? PrimaryPositionId { get; set; }
        public string PrimaryPositionName { get; set; }
        public string EmployeeStatus { get; set; } = "None";
        public string SearchPosName { get; set; }
        public string SearchParentPosName { get; set; }
        public string SearchContactType { get; set; } = string.Empty;
        public string SearchContactPhone { get; set; }
        public string SearchContactEmail { get; set; }
        public string SearchAllPosName { get; set; }
        public string SearchAllParentPosName { get; set; }
        public string SearchSelectedPosName { get; set; }
        public string SearchSelectedParentPosName { get; set; }
        public string SearchSubordinateFullName { get; set; }
        public Dictionary<string, Guid> OrganizationIdCash { get; set; } = new Dictionary<string, Guid>();
        public Dictionary<string, Guid> DivisionIdCash { get; set; } = new Dictionary<string, Guid>();
        public Dictionary<string, string> SearchPosNameCash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SearchParentPosNameCash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SearchContactTypeCash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SearchContactPhoneCash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SearchContactEmailCash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SearchAllPosNameCash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SearchAllParentPosNameCash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SearchSelectedPosNameCash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SearchSelectedParentPosNameCash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SearchSubordinateFullNameCash { get; set; } = new Dictionary<string, string>();
        public List<EmployeeViewModel> SubordinatesViewModels { get; set; } = new List<EmployeeViewModel>();
        public List<EmployeePositionViewModel> EmployeePositionViewModels { get; set; } = new List<EmployeePositionViewModel>();
        public List<EmployeeContactViewModel> EmployeeContactViewModels { get; set; } = new List<EmployeeContactViewModel>();
    }
}
