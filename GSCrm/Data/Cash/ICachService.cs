using System;
using System.Collections.Generic;
using GSCrm.Models;
using GSCrm.Data.ApplicationInfo;
using Microsoft.Extensions.Caching.Memory;

namespace GSCrm.Data.Cash
{
    public interface ICachService
    {
        #region Base
        Dictionary<string, MemoryCache> GetCashItems();
        Dictionary<string, Dictionary<string, ViewInfo>> GetCashViews();
        #endregion

        #region Objects
        bool TryGetValue(User user, string itemName, out object itemValue);
        bool TryGetValue(User user, string itemName, out int itemValue);
        void AddOrUpdate(User user, string itemName, object itemValue);
        void AddOrUpdate(User user, string itemName, int itemValue);
        #endregion

        #region Generic Entities
        /// <summary>
        /// Метод кеширует модель по ее названию
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="user"></param>
        /// <param name="entity">Модель</param>
        /// <param name="entityName">Название представления</param>
        void CacheEntity<TEntity>(User user, TEntity entity, string entityName = null) where TEntity : IMainEntity;
        /// <summary>
        /// Метод кеширует модель как текущую, на которой находится пользователь по ее названию
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="user"></param>
        /// <param name="entity">Модель</param>
        /// <param name="entityName">Название представления</param>
        void CacheCurrentEntity<TEntity>(User user, TEntity entity, string entityName = null) where TEntity : IMainEntity;
        /// <summary>
        /// Метод пытается вернуть закешированную модель по id записи и ее названию
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="user"></param>
        /// <param name="recordId">Id записи</param>
        /// <param name="entity">Модель</param>
        /// <param name="entityName">Название представления</param>
        /// <returns></returns>
        bool TryGetCachedEntity<TEntity>(User user, Guid recordId, out TEntity entity, string entityName = null) where TEntity : class, IMainEntity;
        /// <summary>
        /// Метод пытается вернуть закешированную модель по id записи и ее названию
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="user"></param>
        /// <param name="recordId">Id записи</param>
        /// <param name="entity">Модель</param>
        /// <param name="entityName">Название представления</param>
        /// <returns></returns>
        bool TryGetCachedEntity<TEntity>(User user, string recordId, out TEntity entity, string entityName = null) where TEntity : class, IMainEntity;
        /// <summary>
        /// Метод возвращает закешированную модель сущности, на которой в данный момент находится пользователь по ее названию
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="user"></param>
        /// <param name="entityName">Название представления</param>
        /// <returns></returns>
        TEntity GetCachedCurrentEntity<TEntity>(User user, string entityName = null) where TEntity : class, IMainEntity;
        #endregion

        #region ViewInfo
        /// <summary>
        /// Метод устанавливает информацию о представлении
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="viewName"></param>
        /// <param name="viewInfo"></param>
        void SetViewInfo(string userId, string viewName, ViewInfo viewInfo);
        /// <summary>
        /// Метод устанавливает информацию о представлении, кешируя его по составному ключу из id записи и названия представления
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="recordId">Id записи</param>
        /// <param name="viewName">Название представления</param>
        /// <param name="viewInfo"></param>
        void SetViewInfo(string userId, Guid recordId, string viewName, ViewInfo viewInfo);
        /// <summary>
        /// Метод возвращает информацию о представлении
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="viewName"></param>
        /// <returns></returns>
        ViewInfo GetViewInfo(string userId, string viewName);
        /// <summary>
        /// Метод возвращает информацию о представлении по составному ключу из id записи и названия представления
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="recordId">Id записи</param>
        /// <param name="viewName">Название представления</param>
        /// <returns></returns>
        ViewInfo GetViewInfo(string userId, Guid recordId, string viewName);
        /// <summary>
        /// Метод устанавливает текущее представление, на котором находится пользователь
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currentViewName"></param>
        void SetCurrentView(string userId, string currentViewName);
        /// <summary>
        /// Метод возвращает текущее представление, на котором находится пользователь
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        ViewInfo GetCurrentViewInfo(string userId);
        #endregion
    }
}
