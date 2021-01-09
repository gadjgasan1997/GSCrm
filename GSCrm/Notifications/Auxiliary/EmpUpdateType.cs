namespace GSCrm.Notifications.Auxiliary
{
    public enum EmpUpdateType
    {
        /// <summary>
        /// Изменение данных на карточке сотрудника
        /// </summary>
        BaseUpdate = 0,
        /// <summary>
        /// Перевод сотрудника в другое подразделение
        /// </summary>
        ChangeDivision = 1,
        /// <summary>
        /// Добавление контакта
        /// </summary>
        AddContact = 2,
        /// <summary>
        /// Изменение контакта
        /// </summary>
        UpdateContact = 3,
        /// <summary>
        /// Удаление контакта
        /// </summary>
        DeleteContact = 4,
        /// <summary>
        /// Изменения должностей сотрудника
        /// </summary>
        SyncPoss = 5,
        /// <summary>
        /// Изменения полномочий сотрудника
        /// </summary>
        SyncResps = 6
    }
}
