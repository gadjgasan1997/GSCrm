using GSCrm.Data;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GSCrm.DataTransformers
{
    public abstract class BaseTransformer<TDataModel, TViewModel>
        where TDataModel : BaseDataModel, new()
        where TViewModel : BaseViewModel, new()
    {
        protected readonly ApplicationDbContext context;
        protected readonly DbSet<TDataModel> dbSet;
        protected readonly ResManager resManager;
        public BaseTransformer(ApplicationDbContext context, ResManager resManager)
        {
            this.context = context;
            dbSet = context.Set<TDataModel>();
            this.resManager = resManager;
        }

        /// <summary>
        /// Преобразует модель данных в модель представления при создании записи
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public virtual TDataModel OnModelCreate(TViewModel viewModel) => new TDataModel();

        /// <summary>
        /// Преобразует модель данных в модель представления при обновлении записи
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public virtual TDataModel OnModelUpdate(TViewModel viewModel) => dbSet.Find(viewModel.Id);

        /// <summary>
        /// Преобразует модель представления в модель данных
        /// </summary>
        /// <param name="dataModel"></param>
        /// <returns></returns>
        public virtual TViewModel DataToViewModel(TDataModel dataModel) => new TViewModel();

        /// <summary>
        /// Метод получает на вход модель представления и обновляет значение ее полей из кеша
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public virtual TViewModel UpdateViewModelFromCash(TViewModel viewModel) => viewModel;
    }
}
