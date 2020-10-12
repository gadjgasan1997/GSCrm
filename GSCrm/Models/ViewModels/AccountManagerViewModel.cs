using System;

namespace GSCrm.Models.ViewModels
{
    public class AccountManagerViewModel : BaseViewModel
    {
        public Guid EmployeeId { get; set; }
        public string InitialName { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsLock { get; set; }
        public string PositionName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
