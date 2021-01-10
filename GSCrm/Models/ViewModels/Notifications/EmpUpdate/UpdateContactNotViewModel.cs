namespace GSCrm.Models.ViewModels.Notifications.EmpUpdate
{
    public class UpdateContactNotViewModel : EmpUpdateNotViewModel
    {
        public EmployeeContact OldEmployeeContact { get; set; }
        public EmployeeContact NewEmployeeContact { get; set; }
    }
}
