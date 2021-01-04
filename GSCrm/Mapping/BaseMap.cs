using GSCrm.Data;
using GSCrm.Data.Cash;
using GSCrm.Helpers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Transactions;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using GSCrm.Factories;
using System.Linq;

namespace GSCrm.Mapping
{
    public class BaseMap<TDataModel, TViewModel> : IMap<TDataModel, TViewModel>
        where TDataModel : BaseDataModel, new()
        where TViewModel : BaseViewModel, new()
    {
        #region Declarations
        /// <summary>
        /// Контекст приложения
        /// </summary>
        protected readonly ApplicationDbContext context;
        protected readonly IServiceProvider serviceProvider;
        protected readonly DbSet<TDataModel> dbSet;
        protected readonly IResManager resManager;
        /// <summary>
        /// Текущий пользователь
        /// </summary>
        protected readonly User currentUser;
        /// <summary>
        /// Кеш сервис
        /// </summary>
        protected readonly ICachService cachService;
        /// <summary>
        /// Сервис транзакций
        /// </summary>
        protected readonly ITransactionFactory<TViewModel> transactionFactory;
        protected ITransaction transaction;
        #endregion

        #region Construts
        public BaseMap(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            this.serviceProvider = serviceProvider;
            this.context = context;

            ITFFactory TFFactory = serviceProvider.GetService(typeof(ITFFactory)) as ITFFactory;
            transactionFactory = TFFactory.GetTransactionFactory<TViewModel>(serviceProvider, context);
            resManager = serviceProvider.GetService(typeof(IResManager)) as IResManager;
            cachService = serviceProvider.GetService(typeof(ICachService)) as ICachService;

            IUserContextFactory userContextServices = serviceProvider.GetService(typeof(IUserContextFactory)) as IUserContextFactory;
            currentUser = userContextServices.HttpContext.GetCurrentUser(context);
            dbSet = context.Set<TDataModel>();
        }
        #endregion

        public virtual TDataModel OnModelCreate(TViewModel viewModel)
        {
            SetTransaction(OperationType.Create);
            return new TDataModel();
        }

        public virtual TDataModel OnModelUpdate(TViewModel viewModel)
        {
            SetTransaction(OperationType.Update);
            return dbSet.AsNoTracking().FirstOrDefault(i => i.Id == viewModel.Id);
        }

        public virtual TViewModel DataToViewModel(TDataModel dataModel) => new TViewModel();

        protected void SetTransaction(OperationType operationType)
            => transaction = transactionFactory.GetTransaction(currentUser.Id, operationType);
    }
}
