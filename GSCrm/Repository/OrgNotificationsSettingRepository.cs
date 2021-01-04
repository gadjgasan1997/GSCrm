using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Mapping;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Transactions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;

namespace GSCrm.Repository
{
    public class OrgNotificationsSettingRepository : BaseRepository<OrgNotificationsSetting, OrgNotificationsSettingViewModel>
    {
        public OrgNotificationsSettingRepository(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        #region Override Methods
        protected override bool RespsIsCorrectOnUpdate(OrgNotificationsSettingViewModel entityToUpdate) => true;

        protected override bool TryUpdatePrepare(OrgNotificationsSettingViewModel settingViewModel)
        {
            if (context.UserOrganizations.AsNoTracking().FirstOrDefault(i => i.Id == settingViewModel.UserOrganizationId) == null)
            {
                errors.Add("RecordNotFound", resManager.GetString("RecordNotFound"));
                return false;
            }
            return true;
        }

        protected override void OnRecordNotFound(OrgNotificationsSettingViewModel notificationsSetting)
        {
            UserOrganization userOrganization = context.UserOrganizations.AsNoTracking()
                .Include(org => org.Organization)
                .FirstOrDefault(i => i.Id == notificationsSetting.UserOrganizationId);
            string errorText = resManager.GetString("NotSettingNotFound").Replace("{orgName}", userOrganization.Organization.Name);
            if (!errors.ContainsKey("NotSettingNotFound"))
                errors.Add("NotSettingNotFound", errorText);
            else errors["NotSettingNotFound"] = $"{errors["NotSettingNotFound"]}<br />{errorText}";
        }

        protected override void UpdateAddErrors(ModelStateDictionary modelState) { }
        #endregion

        #region Attach Settings
        public void AttachSettings(ref AllNotificationsSettingsViewModel settingsViewModel)
        {
            settingsViewModel.OrgNotificationsSettingViewModels = context.GetNotificationsSettings(currentUser)
                .MapToViewModels(map, GetLimitList);
        }

        private List<OrgNotificationsSetting> GetLimitList(List<OrgNotificationsSetting> settings)
        {
            List<OrgNotificationsSetting> limitedSettings = settings;
            LimitListByPageNumber(NOT_SETTINGS, ref limitedSettings);
            return limitedSettings;
        }
        #endregion

        #region Other
        /// <summary>
        /// Метод обновляет настройки уведомлений для организаций
        /// </summary>
        /// <param name="notificationsSettings"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public bool TryCommitOrgSettings(AllNotificationsSettingsViewModel notificationsSettings, ModelStateDictionary modelState)
        {
            if (notificationsSettings.OrgNotificationsSettingViewModels != null)
            {

                // Обновление настроек уведомлений у всех организаций в цикле
                foreach (OrgNotificationsSettingViewModel notificationsSetting in notificationsSettings.OrgNotificationsSettingViewModels)
                {
                    OrgNotificationsSettingViewModel setting = notificationsSetting;
                    if (!TryUpdate(ref setting, modelState)) continue;
                }
                if (!errors.Any()) return true;

                // Добавление ошибок
                foreach (KeyValuePair<string, string> error in errors)
                    modelState.AddModelError(error.Key, error.Value);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Метод устанавливает настройки по умолчанию для уведомлений
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public bool SetNotSettingsToDefault(ModelStateDictionary modelState)
        {
            transaction = transactionFactory.Create(currentUser.Id, OperationType.InitNotSetting);

            // Инициализация настроек уведомлений значениями по умолчанию
            context.GetNotificationsSettings(currentUser).ForEach(orgNotSetting =>
            {
                new OrgNotificationsSettingMap(serviceProvider, context).InitNotSetting(orgNotSetting);
                transaction.AddChange(orgNotSetting, EntityState.Modified);
            });

            // Попытка сделать коммит
            if (transactionFactory.TryCommit(transaction, errors))
            {
                transactionFactory.Close(transaction);
                return true;
            }

            // Добавление ошибок при неудаче
            foreach (KeyValuePair<string, string> error in errors)
                modelState.AddModelError(error.Key, error.Value);
            transactionFactory.Close(transaction, TransactionStatus.Error);
            return false;
        }
        #endregion
    }
}
