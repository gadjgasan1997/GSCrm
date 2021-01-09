namespace GSCrm.Notifications
{
    public enum NotificationType
    {
        None = 0,
        /// <summary>
        /// Приглашение в организацию
        /// </summary>
        OrgInvite = 1,
        /// <summary>
        /// Регистрация
        /// </summary>
        Register = 2,
        /// <summary>
        /// Сброс пароля
        /// </summary>
        ResetPassword = 3,
        /// <summary>
        /// Удаление подразделения
        /// </summary>
        DivDelete = 4,
        /// <summary>
        /// Удаление должности
        /// </summary>
        PosDelete = 5
    }
}
