using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Transactions;
using System;

namespace GSCrm.Factories
{
    public interface ITFFactory
    {
        ITransactionFactory<TEntity> GetTransactionFactory<TEntity>(IServiceProvider serviceProvider, ApplicationDbContext context)
            where TEntity : IMainEntity;
    }
}
