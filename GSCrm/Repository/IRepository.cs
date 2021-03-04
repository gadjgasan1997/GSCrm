using System;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GSCrm.Repository
{
    public interface IRepository<TDataModel, TViewModel>
        where TDataModel : BaseDataModel, new()
        where TViewModel : BaseViewModel, new()
    {
        TDataModel NewRecord { get; }
        TDataModel ChangedRecord { get; }
        TDataModel RecordToRemove { get; }
        /// <summary>
        /// Метод проверяет, имеет ли текущий пользователь право на просмотр выбранного элемента
        /// </summary>
        /// <param name="dataModel"></param>
        /// <returns></returns>
        bool HasPermissionsForSeeItem(TDataModel dataModel);
        /// <summary>
        /// Метод выполняет попытку создания записи и, в случае неуспеха, записывает в модель состояния ошибки
        /// </summary>
        /// <param name="entityToCreate">Запись для создания</param>
        /// <param name="modelState">Модель состояния из контроллера</param>
        /// <returns></returns>
        bool TryCreate(ref TViewModel entityToCreate, ModelStateDictionary modelState, User currentUser = null);
        /// <summary>
        /// Метод выполняет попытку обновления записи и, в случае неуспеха, записывает в модель состояния ошибки
        /// </summary>
        /// <param name="entityToUpdate">Запись для обновления</param>
        /// <param name="modelState">Модель состояния из контроллера</param>
        /// <returns></returns>
        bool TryUpdate(ref TViewModel entityToUpdate, ModelStateDictionary modelState, User currentUser = null);
        /// <summary>
        /// Метод выполняет попытку удаления записи и, в случае неуспеха, записывает в модель состояния ошибки
        /// </summary>
        /// <param name="id">Id удаляемой записи</param>
        /// <param name="modelState">Модель состояния из контроллера</param>
        /// <returns></returns>
        bool TryDelete(string id, ModelStateDictionary modelState, User currentUser = null);
        /// <summary>
        /// Метод устанавливает информацию о представлении
        /// </summary>
        /// <param name="viewName">Название представления</param>
        /// <param name="pageNumber">Номер страницы</param>
        void SetViewInfo(string viewName, int pageNumber);
        /// <summary>
        /// Метод устанавливает информацию о частичном представлении
        /// </summary>
        /// <param name="mainRecordId">Id записи основной сущности</param>
        /// <param name="partialViewName">Название частичного представления</param>
        /// <param name="pageNumber">Номер страницы</param>
        void SetViewInfo(Guid mainRecordId, string partialViewName, int pageNumber);
        /// <summary>
        /// Метод устанавливает информацию о частичном представлении
        /// </summary>
        /// <param name="mainRecordId">Id записи основной сущности</param>
        /// <param name="partialViewName">Название частичного представления</param>
        /// <param name="pageNumber">Номер страницы</param>
        void SetViewInfo(string mainRecordId, string partialViewName, int pageNumber);
        /// <summary>
        /// Метод выполняет все необходимые подготовление и загружает представление
        /// </summary>
        /// <param name="dataModel"></param>
        /// <returns></returns>
        TViewModel LoadView(TDataModel dataModel);
        /// <summary>
        /// Методы пытаются найти сущность и, в случае успеха, возвращают ее
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modelState"></param>
        /// <param name="dataModel"></param>
        /// <returns></returns>
        bool TryGetItemById(string id, ModelStateDictionary modelState, out TDataModel dataModel);
        bool TryGetItemById(string id, out TDataModel dataModel);
        bool TryGetItemById(Guid? id, ModelStateDictionary modelState, out TDataModel dataModel);
        bool TryGetItemById(Guid? id, out TDataModel dataModel);
    }
}
