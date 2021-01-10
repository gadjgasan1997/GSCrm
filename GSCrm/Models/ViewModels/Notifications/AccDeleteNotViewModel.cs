namespace GSCrm.Models.ViewModels.Notifications
{
    public class AccDeleteNotViewModel : UserNotificationViewModel
    {
        public Organization Organization { get; set; }
        public string RemovedAccountName { get; set; }
    }
}
