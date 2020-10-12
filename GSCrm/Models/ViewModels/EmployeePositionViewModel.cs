using System;

namespace GSCrm.Models.ViewModels
{
    public class EmployeePositionViewModel : BaseViewModel
    {
        public Guid PositionId { get; set; }
        public string PositionName { get; set; }
        public Guid? ParentPositionId { get; set; }
        public string ParentPositionName { get; set; }
        public bool IsPrimary { get; set; }
    }
}
