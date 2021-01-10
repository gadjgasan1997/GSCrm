namespace GSCrm.Transactions
{
    public enum OperationType
    {
        Create = 0,
        Update = 1,
        Delete = 2,
        /// <summary>
        /// Изменение подразделения сотрудника
        /// </summary>
        ChangeEmployeeDivision = 3,
        /// <summary>
        /// Разблокировка сотрудника
        /// </summary>
        UnlockEmployee = 4,
        /// <summary>
        /// Управление должностями сотрудника
        /// </summary>
        EmployeePositionsManagement = 5,
        /// <summary>
        /// Управление полномочиями сотрудника
        /// </summary>
        EmployeeResponsibilitiesManagement = 6,
        /// <summary>
        /// Изменение подразделения должности
        /// </summary>
        ChangePositionDivision = 7,
        /// <summary>
        /// Изменение юридического адреса клиента
        /// </summary>
        ChangeAccountLegalAddress = 8,
        /// <summary>
        /// Изменение основного контакта клиента
        /// </summary>
        ChangeAccountPrimaryContact = 9,
        /// <summary>
        /// Изменение типа клиента
        /// </summary>
        ChangeAccountType = 10,
        /// <summary>
        /// Управление командой по клиенту
        /// </summary>
        AccountTeamManagement = 11,
        /// <summary>
        /// Разблокировка клиента
        /// </summary>
        UnlockAccount = 12,
        /// <summary>
        /// Изменение основной организации
        /// </summary>
        ChangePrimaryOrganization = 13,
        /// <summary>
        /// Выход из организации
        /// </summary>
        LeaveOrganization = 14,
        /// <summary>
        /// Отправка уведомления
        /// </summary>
        SendNotification = 15,
        /// <summary>
        /// Инициализация настроек уведомлений значениями по умолчанию
        /// </summary>
        InitNotSetting = 16,
        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        Register = 17,
        /// <summary>
        /// Авторизация пользователя
        /// </summary>
        Login = 18,
        /// <summary>
        /// Указание почты для сброса пароля
        /// </summary>
        ResetPasswordSpecifyEmail = 19,
        /// <summary>
        /// Сброс пароля
        /// </summary>
        ResetPassword = 20,
        /// <summary>
        /// Принятие приглашения в организацию
        /// </summary>
        AcceptInvite = 21,
        /// <summary>
        /// Отказ принять приглашение в организацию
        /// </summary>
        RejectInvite = 22,
        /// <summary>
        /// Разблокировка должности
        /// </summary>
        UnlockPosition = 23
    }
}
