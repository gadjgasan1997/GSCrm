namespace GSCrm.Notifications.Params
{
    public class UserRegisterParams : INotificationParams
    {
        public string Token { get; set; }
        public string ConfirmEmailUrl { get; set; }
    }
}
