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
        public void RemoveUserCache(string userId)
        {
            if (CacheData._cashItems.ContainsKey(userId))
                CacheData._cashItems.Remove(userId);
            if (CacheData._cashViews.ContainsKey(userId))
                CacheData._cashViews.Remove(userId);
            if (CacheData._currentViews.ContainsKey(userId))
                CacheData._currentViews.Remove(userId);
        }
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

        public bool TryGetValue(User user, string itemName, out bool itemValue)
        {
            InitCacheItems(user);
            if (CacheData._cashItems[user.Id].TryGetValue(itemName, out bool item))
            {
                itemValue = item;
                return true;
            }
            itemValue = false;
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

        public void AddOrUpdate(User user, string itemName, bool itemValue)
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
        public bool TryGetEntityCache<TEntity>(User user, out TEntity entity, string cachedEntityName = null)
            where TEntity : class, IMainEntity
        {
            if (TryGetValue(user, cachedEntityName ?? $"Current{typeof(TEntity).Name}", out object item))
            {
                entity = (TEntity)item;
                return entity != null;
            }
            entity = null;
            return false;
        }

        public bool TryGetEntitiesCache<TEntity>(User user, out List<TEntity> entities, string cachedEntitiesName = null)
            where TEntity : class, IMainEntity
        {
            if (TryGetValue(user, cachedEntitiesName ?? $"ListOf{typeof(TEntity).Name}", out object items))
            {
                entities = (List<TEntity>)items;
                return entities != null;
            }
            entities = null;
            return false;
        }

        public Guid GetEntityId<TEntity>(User user, string cachedEntityName = null)
            where TEntity : class, IMainEntity
        {
            if (TryGetEntityCache(user, out TEntity entity, cachedEntityName))
                return entity.Id;
            return Guid.Empty;
        }

        public void AddOrUpdateEntity<TEntity>(User user, TEntity entity, string entityName = null)
            where TEntity : IMainEntity
        {
            InitCacheItems(user);
            CacheData._cashItems[user.Id].Set(entityName ?? $"Current{typeof(TEntity).Name}", entity);
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

        public ViewInfo GetViewInfo(string userId, string viewName)
        {
            if (new[] { userId, viewName }.IsNullOrEmpty())
                return new ViewInfo(viewName);
            InitCacheViews(userId);
            if (!CacheData._cashViews[userId].ContainsKey(viewName))
                return new ViewInfo(viewName);
            return CacheData._cashViews[userId][viewName];
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
        private void InitCacheItems(User user)
        {
            if (!CacheData._cashItems.ContainsKey(user.Id))
                CacheData._cashItems.Add(user.Id, new MemoryCache(new MemoryCacheOptions()));
        }

        private void InitCacheListItems(User user)
        {
            if (!CacheData._cashListItems.ContainsKey(user.Id))
                CacheData._cashListItems.Add(user.Id, new MemoryCache(new MemoryCacheOptions()));
        }

        private void InitCacheViews(string userId)
        {
            if (!CacheData._cashViews.ContainsKey(userId))
                CacheData._cashViews.Add(userId, new Dictionary<string, ViewInfo>());
            if (!CacheData._currentViews.ContainsKey(userId))
                CacheData._currentViews.Add(userId, null);
        }
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
