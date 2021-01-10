namespace GSCrm.Notifications.Auxiliary
{
    public enum AccUpdateType
    {
        /// <summary>
        /// Изменение данных на карточке клиента
        /// </summary>
        BaseUpdate = 0,
        /// <summary>
        /// Добавление контакта
        /// </summary>
        AddContact = 1,
        /// <summary>
        /// Изменение контакта
        /// </summary>
        UpdateContact = 2,
        /// <summary>
        /// Удаление контакта
        /// </summary>
        DeleteContact = 3,
        /// <summary>
        /// Добавление адреса
        /// </summary>
        AddAddress = 4,
        /// <summary>
        /// Изменение адреса
        /// </summary>
        UpdateAddress = 5,
        /// <summary>
        /// Удаление адреса
        /// </summary>
        DeleteAddress = 6,
        /// <summary>
        /// Добавление банковских реквизитов
        /// </summary>
        AddInvoice = 7,
        /// <summary>
        /// Изменение банковских реквизитов
        /// </summary>
        UpdateInvoice = 8,
        /// <summary>
        /// Удаление банковских реквизитов
        /// </summary>
        DeleteInvoice = 9
    }
}
