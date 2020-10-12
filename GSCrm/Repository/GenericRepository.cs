using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using GSCrm.Data;
using GSCrm.Data.ApplicationInfo;
using GSCrm.DataTransformers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Validators;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using static GSCrm.CommonConsts;

namespace GSCrm.Repository
{
    public class GenericRepository<TDataModel, TViewModel, TValidator, TTransformer>
        where TDataModel : BaseDataModel, new()
        where TViewModel : BaseViewModel, new()
        where TValidator : BaseValidator<TViewModel>
        where TTransformer: BaseTransformer<TDataModel, TViewModel>
    {
        protected ApplicationDbContext context;
        protected DbSet<TDataModel> dbSet;
        protected IViewsInfo viewsInfo;
        protected ResManager resManager;
        protected TValidator validator;
        protected TTransformer transformer;
        protected TDataModel changedRecord;
        public TDataModel newRecord;
        public GenericRepository(ApplicationDbContext context)
        {
            this.context = context;
            dbSet = context.Set<TDataModel>();
        }

        public GenericRepository(ApplicationDbContext context, ResManager resManager)
        {
            this.context = context;
            dbSet = context.Set<TDataModel>();
            this.resManager = resManager;
        }

        public GenericRepository(ApplicationDbContext context, ResManager resManager, TValidator validator, TTransformer transformer)
        {
            this.context = context;
            dbSet = context.Set<TDataModel>();
            this.resManager = resManager;
            this.validator = validator;
            this.transformer = transformer;
        }

        public GenericRepository(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager, TValidator validator, TTransformer transformer)
        {
            this.context = context;
            dbSet = context.Set<TDataModel>();
            this.viewsInfo = viewsInfo;
            this.resManager = resManager;
            this.validator = validator;
            this.transformer = transformer;
        }

        /// <summary>
        /// Метод выполняет попытку создания записи и, в случае неуспеха, записывает в модель состояния ошибки
        /// </summary>
        /// <param name="entityToCreate">Запись для создания</param>
        /// <param name="modelState">Модель состояния из контроллера</param>
        /// <returns></returns>
        public bool TryCreate(ref TViewModel entityToCreate, ModelStateDictionary modelState)
        {
            // Валидация модели при создании
            if (TryCreatePrepare(entityToCreate, modelState))
            {
                try
                {
                    // Преобразование записи и ее добавление
                    TDataModel dataModel = transformer.OnModelCreate(entityToCreate);
                    context.Entry(dataModel).State = EntityState.Added;
                    dbSet.Add(dataModel);
                    context.SaveChanges();
                    newRecord = dataModel;
                    return true;
                }
                catch(Exception ex)
                {
                    modelState.AddModelError(resManager.GetString("UnhandledException"), ex.Message);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Метод осуществляет проверку модели представления перед созданием записи
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public virtual bool TryCreatePrepare(TViewModel viewModel, ModelStateDictionary modelState)
        {
            Dictionary<string, string> creationErrors = validator.CreationCheck(viewModel);
            if (creationErrors.Any())
            {
                foreach (KeyValuePair<string, string> error in creationErrors)
                    modelState.AddModelError(error.Key, error.Value);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Метод выполняет попытку обновления записи и, в случае неуспеха, записывает в модель состояния ошибки
        /// </summary>
        /// <param name="entityToUpdate">Запись для обновления</param>
        /// <param name="modelState">Модель состояния из контроллера</param>
        /// <returns></returns>
        public bool TryUpdate(ref TViewModel entityToUpdate, ModelStateDictionary modelState)
        {
            // Поиск записи
            if (dbSet.Find(entityToUpdate.Id) == null)
                modelState.AddModelError("RecordNotFound", resManager.GetString("RecordNotFound"));
            // Валидация модели при обновлении
            else if (TryUpdatePrepare(entityToUpdate, modelState))
            {
                try
                {
                    // Преобразование записи при обновлении(получение ее из бд и изменение значений полей)
                    TDataModel dataModel = transformer.OnModelUpdate(entityToUpdate);

                    // Обновление записи
                    context.Entry(dataModel).State = EntityState.Modified;
                    dbSet.Update(dataModel);
                    context.SaveChanges();
                    changedRecord = dataModel;

                    // Получение из бд обновленной записи и преобразование ее в модель отображения
                    entityToUpdate = transformer.DataToViewModel(dataModel);
                    return true;
                }
                catch (Exception ex)
                {
                    modelState.AddModelError(resManager.GetString("UnhandledException"), ex.Message);
                    FailureUpdateHandler(entityToUpdate);
                    return false;
                }
            }
            FailureUpdateHandler(entityToUpdate);
            return false;
        }

        /// <summary>
        /// Метод осуществляет проверку модели представления при обновлении записи
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public virtual bool TryUpdatePrepare(TViewModel viewModel, ModelStateDictionary modelState)
        {
            Dictionary<string, string> updationErrors = validator.UpdateCheck(viewModel);
            if (updationErrors.Any())
            {
                foreach (KeyValuePair<string, string> error in updationErrors.ToList())
                    modelState.AddModelError(error.Key, error.Value);
                return false;
            }
            return true;
        }

        public virtual void FailureUpdateHandler(TViewModel viewModel, Action<TViewModel> handler = null) => handler?.Invoke(viewModel);

        /// <summary>
        /// Метод выполняет попытку удаления записи и, в случае неуспеха, записывает в модель состояния ошибки
        /// </summary>
        /// <param name="id">Id удаляемой записи</param>
        /// <param name="modelState">Модель состояния из контроллера</param>
        /// <returns></returns>
        public bool TryDelete(string id, ModelStateDictionary modelState)
        {
            /*if (TryParseId(id, modelState, out Guid guid))
            {
                TDataModel entityToDelete = dbSet.Find(guid);
                if (TryDeletePrepare(guid, entityToDelete, modelState))
                {
                    if (entityToDelete == null)
                        modelState.AddModelError("RecordNotFound", resManager.GetString("RecordNotFound"));
                    else
                    {
                        try
                        {
                            context.Entry(entityToDelete).State = EntityState.Deleted;
                            dbSet.Remove(entityToDelete);
                            context.SaveChanges();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            modelState.AddModelError(resManager.GetString("UnhandledException"), ex.Message);
                            return false;
                        }
                    }
                }
            }*/
            return false;
        }

        /// <summary>
        /// Метод осуществляет проверку модели представления при удалении записи
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dataModel"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public virtual bool TryDeletePrepare(Guid id, TDataModel dataModel, ModelStateDictionary modelState)
        {
            Dictionary<string, string> deleteErrors = validator.DeleteCheck(id);
            if (deleteErrors.Any())
            {
                foreach (KeyValuePair<string, string> error in deleteErrors)
                    modelState.AddModelError(error.Key, error.Value);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Устанавливает константы, отвечающие за номер страницы(currentPageNumber), 
        /// количество отображдаемых элементов(skipItemsCount)
        /// и количество шагов для пропуска элементов(skipSteps)
        /// </summary>
        /// <param name="pageNumber"></param>
        public virtual void SetViewInfo(string userId, string viewName, int pageNumber, int itemsCount = DEFAULT_ITEMS_COUNT, int pageStep = DEFAULT_PAGE_STEP)
        {
            ViewInfo viewInfo = viewsInfo.Get(userId, viewName);
            viewInfo.CurrentPageNumber = pageNumber <= DEFAULT_MIN_PAGE_NUMBER ? DEFAULT_MIN_PAGE_NUMBER : pageNumber;
            viewInfo.SkipSteps = viewInfo.CurrentPageNumber - pageStep;
            //if (viewInfo.CurrentPageNumber != DEFAULT_MIN_PAGE_NUMBER)
            viewInfo.SkipItemsCount = viewInfo.SkipSteps * itemsCount;
            viewsInfo.Set(userId, viewName, viewInfo);
        }

        /// <summary>
        /// Метод возвращает ограниченный список данных для отображения
        /// </summary>
        /// <typeparam name="TItemsListType"></typeparam>
        /// <param name="viewName"></param>
        /// <param name="itemsToLimit"></param>
        /// <param name="itemsCount"></param>
        /// <returns></returns>
        protected void LimitListByPageNumber<TItemsListType>(string userId, string viewName, ref List<TItemsListType> itemsToLimit, int itemsCount = DEFAULT_ITEMS_COUNT, int pageStep = DEFAULT_PAGE_STEP)
            where TItemsListType : class
        {
            ViewInfo viewInfo = viewsInfo.Get(userId, viewName);
            if (viewInfo != null)
            {
                List<TItemsListType> limitedItems = itemsToLimit.Skip(viewInfo.SkipItemsCount).Take(itemsCount).ToList();
                if (limitedItems.Count == 0)
                {
                    int newSkipItemsCount = (viewInfo.SkipSteps - pageStep) * itemsCount;
                    limitedItems = itemsToLimit.Skip(newSkipItemsCount).ToList();
                    viewInfo.CurrentPageNumber--;
                    viewInfo.SkipSteps -= pageStep;
                    viewInfo.SkipItemsCount -= itemsCount;
                }
                itemsToLimit = limitedItems;
            }
        }

        /// <summary>
        /// Метод пытается преобразовать строковое проедставление id в Guid и найти запись
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        protected bool TryParseId(string id, ModelStateDictionary modelState, out Guid guid)
        {
            guid = default;
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid guidId))
            {
                modelState.TryAddModelError("IdIsWrong", resManager.GetString("IdIsWrong"));
                return false;
            }
            if (dbSet.Find(guidId) == null)
            {
                modelState.TryAddModelError("RecordNotFound", resManager.GetString("RecordNotFound"));
                return false;
            }
            guid = guidId;
            return true;
        }

        /// <summary>
        /// Методы пытаются найти сущность и, в случае успеха, возвращают ее
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modelState"></param>
        /// <param name="dataModel"></param>
        /// <returns></returns>
        public bool TryGetItemById(string id, ModelStateDictionary modelState, out TDataModel dataModel)
        {
            dataModel = null;
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid guid))
            {
                modelState.TryAddModelError("IdIsWrong", resManager.GetString("IdIsWrong"));
                return false;
            }

            if (!TryGetItemById(guid, modelState, out dataModel)) return false;
            return true;
        }

        public bool TryGetItemById(string id, out TDataModel dataModel)
        {
            dataModel = null;
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid guid)) return false;

            if (!TryGetItemById(guid, out dataModel)) return false;
            return true;
        }

        public bool TryGetItemById(Guid? id, ModelStateDictionary modelState, out TDataModel dataModel)
        {
            dataModel = null;
            if (id == null)
            {
                modelState.TryAddModelError("RecordNotFound", resManager.GetString("RecordNotFound"));
                return false;
            }
            dataModel = dbSet.FirstOrDefault(i => i.Id == id);
            if (dataModel == null)
            {
                modelState.TryAddModelError("RecordNotFound", resManager.GetString("RecordNotFound"));
                return false;
            }
            return true;
        }

        public bool TryGetItemById(Guid? id, out TDataModel dataModel)
        {
            dataModel = null;
            if (id == null) return false;
            dataModel = dbSet.FirstOrDefault(i => i.Id == id);
            if (dataModel == null) return false;
            return true;
        }
    }
}
