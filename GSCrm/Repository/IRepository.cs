using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using static GSCrm.CommonConsts;

namespace GSCrm.Repository
{
    public interface IRepository<TDataModel, TViewModel>
        where TDataModel : BaseDataModel, new()
        where TViewModel : BaseViewModel, new()
    {
        /// <summary>
        /// Новая созданная запись
        /// </summary>
        TDataModel NewRecord { get; }
        /// <summary>
        /// Измененная запись
        /// </summary>
        TDataModel ChangedRecord { get; }
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
        /// Устанавливает константы, отвечающие за номер страницы(currentPageNumber), 
        /// количество отображдаемых элементов(skipItemsCount)
        /// и количество шагов для пропуска элементов(skipSteps)
        /// <param name="userId"></param>
        /// <param name="viewName"></param>
        /// <param name="pageNumber"></param>
        /// <param name="itemsCount"></param>
        /// <param name="pageStep"></param>
        void SetViewInfo(string userId, string viewName, int pageNumber, int itemsCount = DEFAULT_ITEMS_COUNT, int pageStep = DEFAULT_PAGE_STEP);
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
