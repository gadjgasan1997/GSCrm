using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace GSCrm.Transactions
{
    public interface ITransaction
    {
        Guid Id { get; set; }
        string Name { get; set; }
        OperationType OperationType { get; set; }
        TransactionStatus TransactionStatus { get; set; }
        string UserId { get; set; }
        /// <summary>
        /// Установка параметра по ключу
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        void AddParameter(string parameterName, object parameterValue);
        /// <summary>
        /// Получение параметра по ключу
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        object GetParameterValue(string parameterName);
        /// <summary>
        /// Добавляет изменение к транзакции с установкой сущности и ее состояния
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityState"></param>
        void AddChange(object entity, EntityState entityState);
        /// <summary>
        /// Возвращает список изменений пользователя для текущей транзакции
        /// </summary>
        /// <returns></returns>
        List<TransactionChange> GetChanges();
    }
}
