namespace GSCrm.Models.Enums
{
    /// <summary>
    /// Источник появления пользователя в организации
    /// </summary>
    public enum UserSource
    {
        None,
        /// <summary>
        /// По приглашению от органзации
        /// </summary>
        Invite,
        /// <summary>
        /// Аккаунт был создан организацией
        /// </summary>
        CreateByOrganization,
    }
}
