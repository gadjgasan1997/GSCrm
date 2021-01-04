using GSCrm.Models;
using GSCrm.Mapping;
using GSCrm.Models.ViewModels;
using System;
using GSCrm.Data;

namespace GSCrm.Factories
{
    public interface IMapFactory
    {
        IMap<TDataModel, TViewModel> GetMap<TDataModel, TViewModel>(IServiceProvider serviceProvider, ApplicationDbContext context)
            where TDataModel : BaseDataModel, new()
            where TViewModel : BaseViewModel, new();
    }
}
