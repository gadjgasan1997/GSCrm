namespace GSCrm.Models.Enums
{
    public enum EmployeeLockReason
    {
        None = 0,
        /// <summary>
        /// Отсутствует подразделение
        /// </summary>
        DivisionAbsent = 1,
        /// <summary>
        /// Отсутствует основная должность
        /// </summary>
        PrimaryPositionAbsent = 2,
        /// <summary>
        /// Сотрудник вышел из компании
        /// </summary>
        EmployeeLeftOrganization = 3,
        /// <summary>
        /// Увольнение
        /// </summary>
        Dismissal = 4,
        /// <summary>
        /// Пользователь не принял приглашение в организацию
        /// </summary>
        RejectInvite = 5
    }
}
