namespace GSCrm.Models.ViewModels
{
    /// <summary>
    /// Модель авторизации
    /// </summary>
    public class UserViewModel : BaseViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string OldPassword { get; set; }
        public string AvatarPath { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
