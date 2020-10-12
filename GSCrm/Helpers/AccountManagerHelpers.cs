using GSCrm.Data;
using GSCrm.DataTransformers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System.Collections.Generic;

namespace GSCrm.Helpers
{
    public static class AccountManagerHelpers
    {

        /// <summary>
        /// Метод преобразует список менеджеров клиентов из уровня данных в уровень отображения
        /// </summary>
        /// <param name="accountManagers"></param>
        /// <param name="resManager"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<AccountManagerViewModel> GetViewModelsFromData(this List<AccountManager> accountManagers, ResManager resManager, ApplicationDbContext context)
        {
            List<AccountManagerViewModel> accountManagerViewModels = new List<AccountManagerViewModel>();
            AccountManagerTransformer accountManagerTransformer = new AccountManagerTransformer(context, resManager);
            if (accountManagers.Count > 0)
            {
                accountManagers?.ForEach(accountManager =>
                {
                    accountManagerViewModels.Add(accountManagerTransformer.DataToViewModel(accountManager));
                });
            }
            return accountManagerViewModels;
        }
    }
}
