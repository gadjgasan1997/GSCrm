using GSCrm.Data;
using GSCrm.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GSCrm.Transactions
{
    public class Transaction : ITransaction
    {
        #region Declarations
        private readonly IServiceProvider serviceProvider;
        protected readonly ApplicationDbContext context;
        /// <summary>
        /// Список всех изменений, произошедшших во время проведения транзакции
        /// </summary>
        private readonly List<TransactionChange> _changes = new List<TransactionChange>();
        /// <summary>
        /// Список параметров, хранимых во время проведения транзакции
        /// </summary>
        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        #endregion

        public Transaction(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            this.serviceProvider = serviceProvider;
            this.context = context;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public OperationType OperationType { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
        public string UserId { get; set; }

        public void AddParameter(string parameterName, object parameterValue)
        {
            if (!_parameters.ContainsKey(parameterName))
                _parameters.Add(parameterName, parameterValue);
            _parameters[parameterName] = parameterValue;
        }

        public object GetParameterValue(string parameterName)
        {
            if (!_parameters.ContainsKey(parameterName))
                return null;
            return _parameters[parameterName];
        }

        public void AddChange(object entity, EntityState entityState)
            => _changes.Add(new TransactionChange()
            {
                Entity = entity,
                EntityState = entityState
            });

        public List<TransactionChange> GetChanges() => _changes;
    }
}
