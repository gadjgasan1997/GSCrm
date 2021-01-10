namespace GSCrm.Notifications.Auxiliary
{
    public enum AccTeamManagementNotType
    {
        /// <summary>
        /// Уведомление при добавлении в команду нового созданного клиента
        /// </summary>
        AddedToNew = 0,
        /// <summary>
        /// Уведомление при добавлении в команду существующего клиента
        /// </summary>
        AddedToExists = 1,
        /// <summary>
        /// Удаление из команды по клиенту
        /// </summary>
        Removed = 2,
        /// <summary>
        /// Назначение основным менеджером
        /// </summary>
        SetToPrimary = 3
    }
}
