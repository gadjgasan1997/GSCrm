namespace GSCrm.Models.ViewModels.Notifications.AccUpdate
{
    public class UpdateContactNotViewModel : AccUpdateNotViewModel
    {
        public AccountContact OldAccountContact { get; set; }
        public AccountContact NewAccountContact { get; set; }
    }
}
