namespace GSCrm.Models.ViewModels.Notifications
{
    public class PosUpdateNotViewModel : UserNotificationViewModel
    {
        public Organization Organization { get; set; }
        public Position ChangedPosition { get; set; }
        public bool DivisionChanged { get; set; }
        public bool IsPrimary { get; set; }
    }
}
