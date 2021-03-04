using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GSCrm.CommonConsts;

namespace GSCrm.Data.Cash
{
    /*public class OrgCachService : CachService, IOrgCachService
    {
        public void CacheView(User user, OrganizationViewModel orgViewModel, string viewName)
            => AddOrUpdate(user, GetCachedItemKey(orgViewModel, viewName), orgViewModel);

        public bool TryGetCachedView<TViewModel>(User user, Guid organizationId, string viewName, out TViewModel viewModel)
            where TViewModel : BaseViewModel
        {
            if (TryGetEntityCache(user, out TViewModel entity, GetCachedItemKey()
        }

        private static string GetCachedItemKey(OrganizationViewModel orgViewModel, string viewName)
            => viewName switch
            {
                POSITIONS => $"{orgViewModel.Id}_{POSITIONS}",
                _ => string.Empty
            };
    }*/
}
