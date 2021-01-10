namespace GSCrm.Models.ViewModels.Notifications
{
    public class EmpDeleteNotViewModel : UserNotificationViewModel
    {
        public Organization Organization { get; set; }
        public string RemovedEmployeeName { get; set; }
    }
}
