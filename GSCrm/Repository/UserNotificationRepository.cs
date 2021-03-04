﻿using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Transactions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;

namespace GSCrm.Repository
{
    public class UserNotificationRepository : BaseRepository<UserNotification, UserNotificationViewModel>
    {
        public UserNotificationRepository(IServiceProvider serviceProvider, ApplicationDbContext context) : base (serviceProvider, context)
        { }

        #region Attach Notifications
        public void AttachNotifications(UserNotificationsViewModel userNotsViewModel)
            => userNotsViewModel.UserNotificationViewModels = context.GetUserNotifications(currentUser).MapToViewModels(map, GetLimitedList);

        private List<UserNotification> GetLimitedList(List<UserNotification> userNots)
        {
            List<UserNotification> limitesUserNots = userNots;
            LimitViewItemsByPageNumber(USER_NOTS, ref limitesUserNots);
            return limitesUserNots;
        }
        #endregion

        #region Override
        public UserNotificationsViewModel LoadNotificationsView()
        {
            UserNotificationsViewModel userNotsViewModel = cachService.GetCachedCurrentEntity<UserNotificationsViewModel>(currentUser);

            // Прикрепление всех сущностей
            AttachNotifications(userNotsViewModel);
            UserNotificationsSetting userNotSetting = context.UserNotificationsSettings.AsNoTracking().FirstOrDefault(u => u.UserId == currentUser.Id);
            userNotsViewModel.UserNotificationsSettingId = userNotSetting.Id.ToString();

            // Кеширование модели
            cachService.SetCurrentView(currentUser.Id, USER_NOTS);
            cachService.CacheEntity(currentUser, userNotsViewModel);
            cachService.CacheCurrentEntity(currentUser, userNotsViewModel);
            return userNotsViewModel;
        }

        protected override bool RespsIsCorrectOnDelete(UserNotification entityToDelete) => true;

        protected override bool TryDeletePrepare(UserNotification userNot)
        {
            return true;
        }
        #endregion

        #region HasReedSign
        /// <summary>
        /// Метод устанавливает признак прочитанного сообщения
        /// </summary>
        /// <param name="userNotId"></param>
        /// <param name="hasReed"></param>
        public void SetHasReedFlag(string userNotId, bool hasReed)
        {
            // Поиск уведомления пользователя по id
            List<UserNotification> userNots = context.GetUserNotifications(currentUser);
            UserNotification userNot = userNots.FirstOrDefault(i => i.Id == Guid.Parse(userNotId));
            SetUserNotHasReedFlag(userNot, hasReed);
        }

        /// <summary>
        /// Метод устанавливает признак прочитанного сообщения для конкретного уведомления
        /// </summary>
        /// <param name="userNot"></param>
        /// <param name="hasReed"></param>
        private void SetUserNotHasReedFlag(UserNotification userNot, bool hasReed)
        {
            if (userNot != null)
            {
                // Создание транзакции и изменние признака прочитанного уведомления
                transaction = viewModelsTF.Create(currentUser.Id, OperationType.Update);
                userNot.HasRead = hasReed;
                transaction.AddChange(userNot, EntityState.Modified);

                // Попытка сделать коммит
                if (viewModelsTF.TryCommit(transaction, errors))
                {
                    // Если успешно, закрытие транзакции и изменение счетчика уведомлений пользователя
                    viewModelsTF.Close(transaction);
                    if (cachService.TryGetValue(currentUser, "NotsCount", out int notsCount))
                    {
                        if (hasReed) notsCount--;
                        else notsCount++;
                        cachService.AddOrUpdate(currentUser, "NotsCount", notsCount);
                    }
                }
                else viewModelsTF.Close(transaction, TransactionStatus.Error);
            }
        }

        /// <summary>
        /// Метод отмечает прочитанными все уведомления пользователя
        /// </summary>
        public void ReadAll()
            => context.GetUserNotifications(currentUser).ForEach(userNot => SetUserNotHasReedFlag(userNot, true));
        #endregion

        #region Other Methods
        /// <summary>
        /// Обработчик, вызываемый после добавления уведомления пользователю
        /// </summary>
        /// <param name="targetUser"></param>
        public void OnUserNotAdded(User targetUser)
        {
            // Необходимо инкрементировать счетчик непрочитанных сообщений
            if (cachService.TryGetValue(targetUser, "NotsCount", out int notsCount))
            {
                notsCount++;
                cachService.AddOrUpdate(targetUser, "NotsCount", notsCount);
            }
        }

        /// <summary>
        /// Обработчик, вызываемый после удаления уведомления пользователя
        /// </summary>
        /// <param name="userNot"></param>
        public void OnUserNotRemoved(UserNotification userNot)
        {
            // Необходимо уменьшить счетчик непрочитанных сообщений, если сообщение не было прочитано перед удалением
            if (!userNot.HasRead && cachService.TryGetValue(currentUser, "NotsCount", out int notsCount))
            {
                notsCount--;
                cachService.AddOrUpdate(currentUser, "NotsCount", notsCount);
            }
        }
        #endregion
    }
}
