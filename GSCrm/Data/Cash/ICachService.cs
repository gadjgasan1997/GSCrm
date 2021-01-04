using GSCrm.Models;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using System;
using System.Collections.Generic;

namespace GSCrm.Data.Cash
{
    public interface ICachService
    {
        string GetCachedItem(string userId, string itemName);
        void CacheItem(string userId, string itemName, string item);
        TEntity GetCachedItem<TEntity>(string userId, string entityName) where TEntity : IMainEntity, new();
        void CacheItem<TEntity>(string userId, string entityName, TEntity entity) where TEntity : IMainEntity;
        List<TEntity> GetCachedItems<TEntity>(string userId, string entityName) where TEntity : IMainEntity, new();
        void CacheItems<TEntity>(string userId, string entityName, List<TEntity> entity) where TEntity : IMainEntity;
        /// <summary>
        /// Метод возвращает основную сущность, на который находится пользователь по ее типу
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="mainEntityType"></param>
        /// <returns></returns>
        IMainEntity GetMainEntity(User currentUser, MainEntityType mainEntityType);
        /// <summary>
        /// Метод возвращает id основной сущности, на который находится пользователь по ключу
        /// </summary>
        /// <param name="currentUser">Пользователь, для которого необходимо получить id сущности, на которой он находится</param>
        /// <param name="mainEntityType">Тип сущности</param>
        /// <returns></returns>
        Guid GetMainEntityId(User currentUser, MainEntityType mainEntityType);
    }
}
