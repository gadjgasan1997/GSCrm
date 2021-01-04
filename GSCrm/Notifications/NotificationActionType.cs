using System;

namespace GSCrm.Models.Enums
{
    [Flags]
    public enum NotificationActionType
    {
        /// <summary>
        /// Скрытие уведомления
        /// </summary>
        Hide = 0b_0000_0000,
        /// <summary>
        /// Принятие предложения уведомления
        /// </summary>
        Allow = 0b_0000_0001,
        /// <summary>
        /// Отказ от предложения в уведомлении
        /// </summary>
        Deniy = 0b_0000_0010,
        /// <summary>
        /// Принятие или отказ
        /// </summary>
        AllowDeniy = Allow | Deniy,
        /// <summary>
        /// Принятие или отказ или скрыть
        /// </summary>
        AllowDeniyHide = Allow | Deniy | Hide
    }
}
