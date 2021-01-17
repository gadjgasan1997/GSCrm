using GSCrm.Data.ApplicationInfo;
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
        /// Метод устанавливает название текущего представлления, на котором находится пользователь
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currentViewName"></param>
        void SetCurrentViewName(string userId, string currentViewName);
        /// <summary>
        /// Метод возвращает название текущего представления, на котором находится пользователь
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        string GetCurrentViewName(string userId);
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
        /// <summary>
        /// Метод кеширует текущую организацию, на которой находится пользователь 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="organization"></param>
        /// <param name="orgViewModel"></param>
        void CacheOrganization(User currentUser, Organization organization, OrganizationViewModel orgViewModel);
        /// <summary>
        /// Метод получает из кеша данные текущей организации, на которой находится пользователь
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="organization"></param>
        /// <param name="orgViewModel"></param>
        void GetCachedOrganization(User currentUser, out Organization organization, out OrganizationViewModel orgViewModel);
        /// <summary>
        /// Метод кеширует текущую должность, на которой находится пользователь 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="position"></param>
        /// <param name="posViewModel"></param>
        void CachePosition(User currentUser, Position position, PositionViewModel posViewModel);
        /// <summary>
        /// Метод кеширует текущего сотрудника, на котором находится пользователь 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="employee"></param>
        /// <param name="empViewModel"></param>
        void CacheEmployee(User currentUser, Employee employee, EmployeeViewModel empViewModel);
        /// <summary>
        /// Метод кеширует текущее полномочие, на котором находится пользователь 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="responsibility"></param>
        /// <param name="respViewModel"></param>
        void CacheResponsibility(User currentUser, Responsibility responsibility, ResponsibilityViewModel respViewModel);
        /// <summary>
        /// Метод кеширует текущего клиента, на котором находится пользователь 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="account"></param>
        /// <param name="accViewModel"></param>
        void CacheAccount(User currentUser, Account account, AccountViewModel accViewModel);
    }
}
