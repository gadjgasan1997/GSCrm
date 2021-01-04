namespace GSCrm.Models.Enums
{
    public enum EmployeeStatus
    {
        None = 0,
        /// <summary>
        /// Заблокированный
        /// </summary>
        Lock = 1,
        /// <summary>
        /// Активный
        /// </summary>
        Active = 2,
        /// <summary>
        /// Ожидает согласия на высланное приглашение
        /// </summary>
        AwaitingInvitationAcceptance = 3
    }
}
