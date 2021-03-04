using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Mapping;
using GSCrm.Helpers;
using GSCrm.Data.Cash;
using GSCrm.Factories;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace GSCrm.Repository
{
    public class AllNotificationsSettingRepository
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ICachService cachService;
        private readonly ApplicationDbContext context;
        private readonly User currentUser;

        public AllNotificationsSettingRepository(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            IUserContextFactory userContextServices = serviceProvider.GetService<IUserContextFactory>();
            cachService = serviceProvider.GetService<ICachService>();
            this.serviceProvider = serviceProvider;
            this.context = context;
            HttpContext httpContext = userContextServices.HttpContext;
            currentUser = httpContext.GetCurrentUser(context);
        }

        public AllNotificationsSettingsViewModel LoadView()
        {
            UserNotificationsSettingViewModel userNotSettingViewModel = cachService.GetCachedCurrentEntity<UserNotificationsSettingViewModel>(currentUser);
            AllNotificationsSettingsViewModel allSettingsViewModel = new AllNotificationsSettingsViewModel
            {
                UserNotificationsSettingViewModel = userNotSettingViewModel
            };
            AttachSettings(allSettingsViewModel);
            return allSettingsViewModel;
        }

        private void AttachSettings(AllNotificationsSettingsViewModel settingsViewModel)
            => settingsViewModel.OrgNotificationsSettingViewModels = context.GetNotificationsSettings(currentUser)
                .MapToViewModels(
                    map: new OrgNotificationsSettingMap(serviceProvider, context),
                    limitingFunc: new OrgNotificationsSettingRepository(serviceProvider, context).GetLimitList);
    }
}
