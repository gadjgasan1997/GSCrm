namespace GSCrm.Models.ViewModels.Notifications.AccUpdate
{
    public class UpdateAddressNotViewModel : AccUpdateNotViewModel
    {
        public AccountAddress OldAccountAddress { get; set; }
        public AccountAddress NewAccountAddress { get; set; }
    }
}
