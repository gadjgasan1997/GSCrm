namespace GSCrm.Models.Enums
{
    /// <summary>
    /// Указание, каким именно образом необходимо прервать запрос
    /// </summary>
    public enum RequestBreakType
    {
        /// <summary>
        /// Возврат ошибки 
        /// </summary>
        Error = 0,
        /// <summary>
        /// Перенаправление на страницу с ошибкой
        /// </summary>
        Redirect = 1
    }
}
