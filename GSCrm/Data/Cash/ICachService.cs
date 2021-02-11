using System;
using System.Collections;
using System.Collections.Generic;
using GSCrm.Data.ApplicationInfo;
using GSCrm.Models;

namespace GSCrm.Data.Cash
{
    public interface ICachService
    {
        #region Base
        /// <summary>
        /// Метод чистит кеш пользователя
        /// </summary>
        /// <param name="userId"></param>
        void RemoveUserCache(string userId);
        #endregion

        #region Objects
        bool TryGetValue(User user, string itemName, out object itemValue);
        bool TryGetValue(User user, string itemName, out bool itemValue);
        bool TryGetValue(User user, string itemName, out int itemValue);
        void AddOrUpdate(User user, string itemName, object itemValue);
        void AddOrUpdate(User user, string itemName, bool itemValue);
        void AddOrUpdate(User user, string itemName, int itemValue);
        #endregion

        #region Generic Entities
        /// <summary>
        /// Метод возвращает кеш сущности
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="user"></param>
        /// <param name="entityCache">Кеш сущности</param>
        /// <param name="cachedEntityName">Необязательное название, которым кешировалась сущность</param>
        /// <returns></returns>
        bool TryGetEntityCache<TEntity>(User user, out TEntity entityCache, string cachedEntityName = null)
            where TEntity : class, IMainEntity;
        /// <summary>
        /// Метод возвращает кеш списка сущностей
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="user"></param>
        /// <param name="entitiesCache">Кеш сущностей</param>
        /// <param name="cachedEntityName">Необязательное название, которым кешировался список сущностей</param>
        /// <returns></returns>
        bool TryGetEntitiesCache<TEntity>(User user, out List<TEntity> entitiesCache, string cachedEntityName = null)
            where TEntity : class, IMainEntity;
        /// <summary>
        /// Метод возвращает id закешированной сущности
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Guid GetEntityId<TEntity>(User user, string cachedEntityName = null) where TEntity : class, IMainEntity;
        /// <summary>
        /// Метод кеширует сущность
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="user"></param>
        /// <param name="entity">Сущность, которую необходимо кешировать</param>
        /// <param name="entityName">Необязательное название</param>
        void AddOrUpdateEntity<TEntity>(User user, TEntity entity, string entityName = null) where TEntity : IMainEntity;
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
        /// Метод возвращает информацию о представлении
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="viewName"></param>
        /// <returns></returns>
        ViewInfo GetViewInfo(string userId, string viewName);
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
