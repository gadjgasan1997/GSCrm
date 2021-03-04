using GSCrm.Models;
using GSCrm.Models.ViewModels;

namespace GSCrm.Mapping
{
    public interface IMap<TDataModel, TViewModel>
        where TDataModel : BaseDataModel, new()
        where TViewModel : BaseViewModel, new()
    {
        /// <summary>
        /// Преобразует модель данных в модель представления при создании записи
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        TDataModel OnModelCreate(TViewModel viewModel);

        /// <summary>
        /// Преобразует модель данных в модель представления при обновлении записи
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        TDataModel OnModelUpdate(TViewModel viewModel);

        /// <summary>
        /// Преобразует модель данных в модель представления
        /// </summary>
        /// <param name="dataModel"></param>
        /// <returns></returns>
        TViewModel DataToViewModel(TDataModel dataModel);
    }
}
