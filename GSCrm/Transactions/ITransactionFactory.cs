using GSCrm.Models;
using System;
using System.Collections.Generic;

namespace GSCrm.Transactions
{
    public interface ITransactionFactory<TEntity>
        where TEntity : IMainEntity
    {
        /// <summary>
        /// Создает транзакцию и вызывает обработчик CreateHandler
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="operationType"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        ITransaction Create(string userId, OperationType operationType, TEntity entity);
        ITransaction Create(string userId, OperationType operationType, string recordId);
        ITransaction Create(string userId, OperationType operationType);
        /// <summary>
        /// Создает транзакцию и вызывает обработчик CreateHandler для создания транзакции неавторизованного пользователя
        /// </summary>
        /// <param name="operationType"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        ITransaction Create(OperationType operationType, TEntity entity);
        /// <summary>
        /// Метод пытается выполнить коммит в бд, в случае неудачи возвращает ошибки
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        bool TryCommit(ITransaction transaction, Dictionary<string, string> errors);
        /// <summary>
        /// Закрывает транзакцию и вызывает обработчик CloseHandler
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="transactionStatus"></param>
        void Close(ITransaction transaction, TransactionStatus transactionStatus = TransactionStatus.None);
        /// <summary>
        /// Получает транзакцию по типу и id пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="operationType"></param>
        /// <returns></returns>
        ITransaction GetTransaction(string userId, OperationType operationType);
    }
}
