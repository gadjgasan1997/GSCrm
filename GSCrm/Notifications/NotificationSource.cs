namespace GSCrm.Notifications
{
    /// <summary>
    /// Источник уведомления
    /// </summary>
    public enum NotificationSource
    {
        /// <summary>
        /// Уведомления от лица самой системы
        /// </summary>
        GSCrm = 0,
        /// <summary>
        /// Уведомления от лица организации
        /// </summary>
        Organization = 1,
        /// <summary>
        /// Уведомления от других пользователей
        /// </summary>
        User = 2
    }
}
