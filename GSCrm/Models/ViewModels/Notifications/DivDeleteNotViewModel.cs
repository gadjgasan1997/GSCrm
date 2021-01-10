namespace GSCrm.Models.ViewModels.Notifications
{
    public class DivDeleteNotViewModel : UserNotificationViewModel
    {
        public Organization Organization { get; set; }
        public string RemovedDivisionName { get; set; }
    }
}
