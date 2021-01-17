using System;
using System.Collections.Generic;
using GSCrm.Data.ApplicationInfo;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;

namespace GSCrm.Data.Cash
{
    public class CachService : ICachService
    {
        public string GetCachedItem(string userId, string itemName)
        {
            InitCacheItem(userId, itemName, string.Empty);
            return CachMainData._cashItems[userId][itemName];
        }

        public void CacheItem(string userId, string itemName, string itemValue)
        {
            InitCacheItem(userId, itemName, itemValue);
            CachMainData._cashItems[userId][itemName] = itemValue;
        }

        /// <summary>
        /// Метод инициализирует элемент кеша, не изменяя его при наличии
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="userId"></param>
        /// <param name="itemName"></param>
        /// <param name="itemValue"></param>
        private void InitCacheItem(string userId, string itemName, string itemValue)
        {
            if (!CachMainData._cashItems.ContainsKey(userId))
            {
                CachMainData._cashItems.Add(userId, new Dictionary<string, string>()
                {
                    { itemName, itemValue }
                });
            }
            else if (!CachMainData._cashItems[userId].ContainsKey(itemName))
                CachMainData._cashItems[userId].Add(itemName, itemValue);
        }

        public TEntity GetCachedItem<TEntity>(string userId, string entityName)
            where TEntity : IMainEntity, new()
        {
            InitCacheItem(userId, entityName, new TEntity());
            return CachEntitiesData<TEntity>._cashItems[userId][entityName];
        }

        public void CacheItem<TEntity>(string userId, string entityName, TEntity entity)
            where TEntity : IMainEntity
        {
            InitCacheItem(userId, entityName, entity);
            CachEntitiesData<TEntity>._cashItems[userId][entityName] = entity;
        }

        public List<TEntity> GetCachedItems<TEntity>(string userId, string entityName)
            where TEntity : IMainEntity, new()
        {
            InitCacheItems(userId, entityName, new List<TEntity>());
            return CachEntitiesData<TEntity>._cashListedItems[userId][entityName];
        }

        public void CacheItems<TEntity>(string userId, string entityName, List<TEntity> entities)
            where TEntity : IMainEntity
        {
            InitCacheItems(userId, entityName, entities);
            CachEntitiesData<TEntity>._cashListedItems[userId][entityName] = entities;
        }

        /// <summary>
        /// Метод инициализирует элемент кеша, не изменяя его при наличии
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="userId"></param>
        /// <param name="entityName"></param>
        /// <param name="entity"></param>
        private void InitCacheItem<TEntity>(string userId, string entityName, TEntity entity)
            where TEntity : IMainEntity
        {
            if (!CachEntitiesData<TEntity>._cashItems.ContainsKey(userId))
            {
                CachEntitiesData<TEntity>._cashItems.Add(userId, new Dictionary<string, TEntity>()
                {
                    { entityName, entity }
                });
            }
            else if (!CachEntitiesData<TEntity>._cashItems[userId].ContainsKey(entityName))
                CachEntitiesData<TEntity>._cashItems[userId].Add(entityName, entity);
        }

        /// <summary>
        /// Метод инициализирует элемент кеша со списком элементов, не изменяя его при наличии
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="userId"></param>
        /// <param name="entityName"></param>
        /// <param name="entity"></param>
        private void InitCacheItems<TEntity>(string userId, string entityName, List<TEntity> entity)
            where TEntity : IMainEntity
        {
            if (!CachEntitiesData<TEntity>._cashListedItems.ContainsKey(userId))
            {
                CachEntitiesData<TEntity>._cashListedItems.Add(userId, new Dictionary<string, List<TEntity>>()
                {
                    { entityName, entity }
                });
            }
            else if (!CachEntitiesData<TEntity>._cashListedItems[userId].ContainsKey(entityName))
                CachEntitiesData<TEntity>._cashListedItems[userId].Add(entityName, entity);
        }

        public void SetViewInfo(string userId, string viewName, ViewInfo viewInfo)
        {
            if (new[] { userId, viewName }.IsNullOrEmpty()) return;

            // Добавление информации о представлении в список представлений пользователя
            if (!CashViewData._cashItems.ContainsKey(userId))
            {
                CashViewData._cashItems.Add(userId, new Dictionary<string, ViewInfo>()
                {
                    { viewName, viewInfo }
                });
            }
            else if (!CashViewData._cashItems[userId].ContainsKey(viewName))
                CashViewData._cashItems[userId].Add(viewName, viewInfo);
            else CashViewData._cashItems[userId][viewName] = viewInfo;

            // Прсотавление назвакния текущего представления, на котором находится пользователь
            if (!CashViewData._currentViewNames.ContainsKey(userId))
                CashViewData._currentViewNames.Add(userId, viewName);
            else CashViewData._currentViewNames[userId] = viewName;
        }

        public ViewInfo GetViewInfo(string userId, string viewName)
        {
            if (new[] { userId, viewName }.IsNullOrEmpty()) return new ViewInfo(viewName);
            if (!CashViewData._cashItems.ContainsKey(userId)) return new ViewInfo(viewName);
            if (!CashViewData._cashItems[userId].ContainsKey(viewName)) return new ViewInfo(viewName);
            return CashViewData._cashItems[userId][viewName];
        }

        public void SetCurrentViewName(string userId, string currentViewName)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                if (!CashViewData._currentViewNames.ContainsKey(userId))
                    CashViewData._currentViewNames.Add(userId, currentViewName);
                else CashViewData._currentViewNames[userId] = currentViewName;
            }
        }

        public string GetCurrentViewName(string userId)
        {
            if (string.IsNullOrEmpty(userId) || !CashViewData._currentViewNames.ContainsKey(userId))
                return null;
            return CashViewData._currentViewNames[userId];
        }

        public IMainEntity GetMainEntity(User currentUser, MainEntityType mainEntityType)
            => mainEntityType switch
            {
                MainEntityType.AccountData => GetCachedItem<Account>(currentUser.Id, "CurrentAccountData"),
                MainEntityType.OrganizationData => GetCachedItem<Organization>(currentUser.Id, "CurrentOrganizationData"),
                MainEntityType.EmployeeData => GetCachedItem<Employee>(currentUser.Id, "CurrentEmployeeData"),
                MainEntityType.PositionData => GetCachedItem<Position>(currentUser.Id, "CurrentPositionData"),
                MainEntityType.ResponsibilityData => GetCachedItem<Responsibility>(currentUser.Id, "CurrentResponsibilityData"),
                MainEntityType.NotificationData => GetCachedItem<Notification>(currentUser.Id, "CurrentNotificationData"),
                MainEntityType.AccountView => GetCachedItem<AccountViewModel>(currentUser.Id, "CurrentAccountView"),
                MainEntityType.OrganizationView => GetCachedItem<OrganizationViewModel>(currentUser.Id, "CurrentOrganizationView"),
                MainEntityType.EmployeeView => GetCachedItem<EmployeeViewModel>(currentUser.Id, "CurrentEmployeeView"),
                MainEntityType.PositionView => GetCachedItem<PositionViewModel>(currentUser.Id, "CurrentPositionView"),
                MainEntityType.ResponsibilityView => GetCachedItem<ResponsibilityViewModel>(currentUser.Id, "CurrentResponsibilityView"),
                MainEntityType.NotificationView => GetCachedItem<UserNotificationViewModel>(currentUser.Id, "CurrentNotificationView"),
                _ => null
            };

        public Guid GetMainEntityId(User currentUser, MainEntityType mainEntityType) => GetMainEntity(currentUser, mainEntityType).Id;

        public void CacheOrganization(User currentUser, Organization organization, OrganizationViewModel orgViewModel)
        {
            CacheItem(currentUser.Id, "CurrentOrganizationData", organization);
            CacheItem(currentUser.Id, "CurrentOrganizationView", orgViewModel);
        }

        public void GetCachedOrganization(User currentUser, out Organization organization, out OrganizationViewModel orgViewModel)
        {
            organization = GetCachedItem<Organization>(currentUser.Id, "CurrentOrganizationData");
            orgViewModel = GetCachedItem<OrganizationViewModel>(currentUser.Id, "CurrentOrganizationView");
        }

        public void CachePosition(User currentUser, Position position, PositionViewModel posViewModel)
        {
            CacheItem(currentUser.Id, "CurrentPositionData", position);
            CacheItem(currentUser.Id, "CurrentPositionView", posViewModel);
        }

        public void CacheEmployee(User currentUser, Employee employee, EmployeeViewModel empViewModel)
        {
            CacheItem(currentUser.Id, "CurrentEmployeeData", employee);
            CacheItem(currentUser.Id, "CurrentEmployeeView", empViewModel);
        }

        public void CacheResponsibility(User currentUser, Responsibility responsibility, ResponsibilityViewModel respViewModel)
        {
            CacheItem(currentUser.Id, "CurrentResponsibilityData", responsibility);
            CacheItem(currentUser.Id, "CurrentResponsibilityView", respViewModel);
        }

        public void CacheAccount(User currentUser, Account account, AccountViewModel accViewModel)
        {
            CacheItem(currentUser.Id, "CurrentAccountData", account);
            CacheItem(currentUser.Id, "CurrentAccountView", accViewModel);
        }

        class CachMainData
        {
            public static readonly Dictionary<string, Dictionary<string, string>> _cashItems = new Dictionary<string, Dictionary<string, string>>();
        }

        class CashViewData
        {
            public static Dictionary<string, string> _currentViewNames { get; set; } = new Dictionary<string, string>();
            public static Dictionary<string, Dictionary<string, ViewInfo>> _cashItems { get; set; } = new Dictionary<string, Dictionary<string, ViewInfo>>();
        }

        class CachEntitiesData<TEntity>
            where TEntity : IMainEntity
        {
            public static readonly Dictionary<string, Dictionary<string, TEntity>> _cashItems = new Dictionary<string, Dictionary<string, TEntity>>();
            public static readonly Dictionary<string, Dictionary<string, List<TEntity>>> _cashListedItems = new Dictionary<string, Dictionary<string, List<TEntity>>>();
        }
    }
}
