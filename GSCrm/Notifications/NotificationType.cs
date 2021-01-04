namespace GSCrm.Notifications
{
    public enum NotificationType
    {
        None,
        /// <summary>
        /// Приглашение в организацию
        /// </summary>
        OrgInvite,
        /// <summary>
        /// Регистрация
        /// </summary>
        Register,
        /// <summary>
        /// Сброс пароля
        /// </summary>
        ResetPassword
    }
}
