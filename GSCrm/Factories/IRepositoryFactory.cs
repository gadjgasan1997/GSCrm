using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using System;

namespace GSCrm.Factories
{
    public interface IRepositoryFactory
    {
        IRepository<TDataModel, TViewModel> GetRepository<TDataModel, TViewModel>(IServiceProvider serviceProvider, ApplicationDbContext context)
            where TDataModel : BaseDataModel, new()
            where TViewModel : BaseViewModel, new();
    }
}
