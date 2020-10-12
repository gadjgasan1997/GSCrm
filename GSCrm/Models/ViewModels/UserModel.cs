namespace GSCrm.Models.ViewModels
{
    /// <summary>
    /// Модель авторизации
    /// </summary>
    public class UserModel : BaseDataModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string OldPassword { get; set; }
    }
}
