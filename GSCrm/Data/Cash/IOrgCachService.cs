using GSCrm.Models;
using GSCrm.Models.ViewModels;

namespace GSCrm.Data.Cash
{
    public interface IOrgCachService : ICachService
    {
        void CacheView(User user, OrganizationViewModel orgViewModel, string viewName);
        bool TryGetCachedView<TViewModel>(User user, string viewName, out TViewModel viewModel) where TViewModel : IMainEntity;
    }
}
