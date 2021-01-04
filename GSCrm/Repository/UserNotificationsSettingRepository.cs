using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using GSCrm.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GSCrm.Mapping;
using GSCrm.Transactions;

namespace GSCrm.Repository
{
    public class UserNotificationsSettingRepository : BaseRepository<UserNotificationsSetting, UserNotificationsSettingViewModel>
    {
        public UserNotificationsSettingRepository(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        protected override bool RespsIsCorrectOnUpdate(UserNotificationsSettingViewModel entityToUpdate) => true;

        #region Other Methods
        /// <summary>
        /// Метод устанавливает настройки по умолчанию для уведомлений
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public bool SetNotSettingsToDefault(ModelStateDictionary modelState)
        {
            transaction = viewModelsTransactionFactory.Create(currentUser.Id, OperationType.InitNotSetting);

            // Инициализация настроек уведомлений значениями по умолчанию
            UserNotificationsSetting userNotSetting = context.UserNotificationsSettings.AsNoTracking().FirstOrDefault(i => i.UserId == currentUser.Id);
            new UserNotificationsSettingMap(serviceProvider, context).InitNotSetting(userNotSetting);
            transaction.AddChange(userNotSetting, EntityState.Modified);

            // Попытка сделать коммит
            if (viewModelsTransactionFactory.TryCommit(transaction, errors))
            {
                viewModelsTransactionFactory.Close(transaction);
                return true;
            }

            // Добавление ошибок при неудаче
            foreach (KeyValuePair<string, string> error in errors)
                modelState.AddModelError(error.Key, error.Value);
            viewModelsTransactionFactory.Close(transaction, TransactionStatus.Error);
            return false;
        }
        #endregion
    }
}
