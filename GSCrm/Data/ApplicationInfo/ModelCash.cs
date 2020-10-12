using GSCrm.Models.ViewModels;
using System.Collections.Generic;

namespace GSCrm.Data.ApplicationInfo
{
    public static class ModelCash<TViewModel>
        where TViewModel : BaseViewModel, new()
    {
        private static Dictionary<string, Dictionary<string, TViewModel>> modelsCash = new Dictionary<string, Dictionary<string, TViewModel>>();

        public static TViewModel GetViewModel(string userId, string viewName)
        {
            if (!modelsCash.ContainsKey(userId))
            {
                modelsCash.Add(userId, new Dictionary<string, TViewModel>()
                {
                    { viewName, new TViewModel() }
                });
            }
            else if (!modelsCash[userId].ContainsKey(viewName))
                modelsCash[userId].Add(viewName, new TViewModel());
            return modelsCash[userId][viewName];
        }

        public static void SetViewModel(string userId, string viewName, TViewModel viewModel)
        {
            if (!modelsCash.ContainsKey(userId))
            {
                modelsCash.Add(userId, new Dictionary<string, TViewModel>()
                {
                    { viewName, viewModel }
                });
            }
            else if (!modelsCash[userId].ContainsKey(viewName))
                modelsCash[userId].Add(viewName, viewModel);
            else modelsCash[userId][viewName] = viewModel;
        }
    }
}
