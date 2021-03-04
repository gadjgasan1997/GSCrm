using System;
using System.Collections.Generic;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Data.ApplicationInfo;
using Microsoft.Extensions.Caching.Memory;

namespace GSCrm.Data.Cash
{
    public class CachService : ICachService
    {
        #region Base
        public Dictionary<string, MemoryCache> GetCashItems()
            => CacheData._cashItems;

        public Dictionary<string, Dictionary<string, ViewInfo>> GetCashViews()
            => CacheData._cashViews;
        #endregion

        #region Objects
        public bool TryGetValue(User user, string itemName, out object itemValue)
        {
            InitCacheItems(user);
            if (CacheData._cashItems[user.Id].TryGetValue(itemName, out object item))
            {
                itemValue = item;
                return true;
            }
            itemValue = null;
            return false;
        }

        public bool TryGetValue(User user, string itemName, out int itemValue)
        {
            InitCacheItems(user);
            if (CacheData._cashItems[user.Id].TryGetValue(itemName, out int item))
            {
                itemValue = item;
                return true;
            }
            itemValue = default;
            return false;
        }

        public void AddOrUpdate(User user, string itemName, object itemValue)
        {
            InitCacheItems(user);
            CacheData._cashItems[user.Id].Set(itemName, itemValue);
        }

        public void AddOrUpdate(User user, string itemName, int itemValue)
        {
            InitCacheItems(user);
            CacheData._cashItems[user.Id].Set(itemName, itemValue);
        }
        #endregion
            
        #region Generic Entities
        public void CacheEntity<TEntity>(User user, TEntity entity, string entityName = null)
            where TEntity : IMainEntity
            => AddOrUpdate(user, entityName ?? GetCachedViewKey(entity.Id, typeof(TEntity).Name), entity);

        public void CacheCurrentEntity<TEntity>(User user, TEntity entity, string entityName = null)
            where TEntity : IMainEntity
            => AddOrUpdate(user, entityName ?? $"Current{typeof(TEntity).Name}", entity);

        public bool TryGetCachedEntity<TEntity>(User user, Guid recordId, out TEntity entity, string entityName = null)
            where TEntity : class, IMainEntity
        {
            if (TryGetValue(user, entityName ?? GetCachedViewKey(recordId, typeof(TEntity).Name), out object entityValue))
            {
                entity = (TEntity)entityValue;
                return true;
            }
            entity = null;
            return false;
        }

        public bool TryGetCachedEntity<TEntity>(User user, string recordId, out TEntity entity, string entityName = null)
            where TEntity : class, IMainEntity
        {
            if (Guid.TryParse(recordId, out Guid guid) && TryGetCachedEntity(user, guid, out TEntity entityValue, entityName))
            {
                entity = entityValue;
                return true;
            }
            entity = null;
            return false;
        }

        public TEntity GetCachedCurrentEntity<TEntity>(User user, string entityName = null)
            where TEntity : class, IMainEntity
        {
            if (TryGetValue(user, entityName ?? $"Current{typeof(TEntity).Name}", out object itemValue))
                return (TEntity)itemValue;
            return null;
        }
        #endregion

        #region ViewInfo
        public void SetViewInfo(string userId, string viewName, ViewInfo viewInfo)
        {
            if (new[] { userId, viewName }.IsNullOrEmpty()) return;
            InitCacheViews(userId);

            // Добавление информации о представлении в список представлений пользователя
            if (!CacheData._cashViews[userId].ContainsKey(viewName))
                CacheData._cashViews[userId].Add(viewName, viewInfo);
            else CacheData._cashViews[userId][viewName] = viewInfo;

            // Прсотавление назвакния текущего представления, на котором находится пользователь
            if (!CacheData._currentViews.ContainsKey(userId))
                CacheData._currentViews.Add(userId, viewInfo);
            else CacheData._currentViews[userId] = viewInfo;
        }

        public void SetViewInfo(string userId, Guid recordId, string viewName, ViewInfo viewInfo)
        {
            string cachedItemKey = GetCachedViewKey(recordId, viewName);
            SetViewInfo(userId, cachedItemKey, viewInfo);
        }

        public ViewInfo GetViewInfo(string userId, string viewName)
        {
            if (new[] { userId, viewName }.IsNullOrEmpty())
                return new ViewInfo(viewName);
            InitCacheViews(userId);
            if (!CacheData._cashViews[userId].ContainsKey(viewName))
                return new ViewInfo(viewName);
            return CacheData._cashViews[userId][viewName];
        }

        public ViewInfo GetViewInfo(string userId, Guid recordId, string viewName)
        {
            string cachedItemKey = GetCachedViewKey(recordId, viewName);
            if (new[] { userId, cachedItemKey }.IsNullOrEmpty())
                return new ViewInfo(cachedItemKey, viewName);
            InitCacheViews(userId);
            if (!CacheData._cashViews[userId].ContainsKey(cachedItemKey))
                return new ViewInfo(cachedItemKey, viewName);
            return CacheData._cashViews[userId][cachedItemKey];
        }

        public void SetCurrentView(string userId, string currentViewName)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                if (!CacheData._currentViews.ContainsKey(userId))
                    CacheData._currentViews.Add(userId, new ViewInfo(currentViewName));
                else CacheData._currentViews[userId] = new ViewInfo(currentViewName);
            }
        }

        public ViewInfo GetCurrentViewInfo(string userId)
        {
            if (string.IsNullOrEmpty(userId) || !CacheData._currentViews.ContainsKey(userId))
                return null;
            return CacheData._currentViews[userId];
        }
        #endregion

        #region Addition Methods
        private static void InitCacheItems(User user)
        {
            if (!CacheData._cashItems.ContainsKey(user.Id))
                CacheData._cashItems.Add(user.Id, new MemoryCache(new MemoryCacheOptions()));
        }

        private static void InitCacheViews(string userId)
        {
            if (!CacheData._cashViews.ContainsKey(userId))
                CacheData._cashViews.Add(userId, new Dictionary<string, ViewInfo>());
            if (!CacheData._currentViews.ContainsKey(userId))
                CacheData._currentViews.Add(userId, null);
        }

        /// <summary>
        /// Метод возвращает ключ, которым кешируется представление
        /// </summary>
        /// <param name="recordId">Id сущности</param>
        /// <param name="viewName">Название представления</param>
        /// <returns></returns>
        private static string GetCachedViewKey(Guid recordId, string viewName)
            => $"{recordId}_{viewName}";
        #endregion

        class CacheData
        {
            public static readonly Dictionary<string, MemoryCache> _cashItems
                = new Dictionary<string, MemoryCache>();
            public static readonly Dictionary<string, MemoryCache> _cashListItems
                = new Dictionary<string, MemoryCache>();
            public static readonly Dictionary<string, Dictionary<string, ViewInfo>> _cashViews
                = new Dictionary<string, Dictionary<string, ViewInfo>>();
            public static readonly Dictionary<string, ViewInfo> _currentViews
                = new Dictionary<string, ViewInfo>();
        }
    }
}
