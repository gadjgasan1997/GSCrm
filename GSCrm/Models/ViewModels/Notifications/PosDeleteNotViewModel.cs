namespace GSCrm.Models.ViewModels.Notifications
{
    public class PosDeleteNotViewModel : UserNotificationViewModel
    {
        public Organization Organization { get; set; }
        public string RemovedPositionName { get; set; }
        public bool IsPrimary { get; set; }
    }
}
