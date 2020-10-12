using System;

namespace GSCrm.Models.ViewModels
{
    public class DivisionViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid? ParentDivisionId { get; set; }
        public string ParentDivisionName { get; set; }
    }
}
