using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using GSCrm.Data;
using GSCrm.Data.Cash;
using GSCrm.Models;
using GSCrm.Mapping;
using GSCrm.Helpers;
using GSCrm.Factories;
using GSCrm.Transactions;
using GSCrm.Localization;
using GSCrm.Models.ViewModels;
using GSCrm.Data.ApplicationInfo;
using GSCrm.Routing.Middleware.AccessibilityMiddleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using static GSCrm.CommonConsts;

namespace GSCrm.Repository
{
    public class BaseRepository<TDataModel, TViewModel> : IRepository<TDataModel, TViewModel>
        where TDataModel : BaseDataModel, new()
        where TViewModel : BaseViewModel, new()
    {
        #region Declarations
        /// <summary>
        /// Контекст приложения
        /// </summary>
        protected readonly ApplicationDbContext context;
        /// <summary>
        /// Http context
        /// </summary>
        protected readonly HttpContext httpContext;
        /// <summary>
        /// Текущий пользователь
        /// </summary>
        protected User currentUser;
        protected readonly DbSet<TDataModel> dbSet;
        protected readonly IServiceProvider serviceProvider;
        protected readonly ITFFactory TFFactory;
        /// <summary>
        /// Хелпер для работы с урлами
        /// </summary>
        protected readonly IUrlHelper urlHelper;
        /// <summary>
        /// Кеш сервис
        /// </summary>
        protected readonly ICachService cachService;
        /// <summary>
        /// Сервис транзакций
        /// </summary>
        protected readonly ITransactionFactory<TViewModel> viewModelsTF;
        /// <summary>
        /// Сервис транзакций
        /// </summary>
        protected readonly ITransactionFactory<TDataModel> dataModelsTF;
        protected ITransaction transaction;
        /// <summary>
        /// Менеджер ресурсов для доступа к переводам
        /// </summary>
        protected readonly IResManager resManager;
        /// <summary>
        /// Преобразователь сущностей при маппинге
        /// </summary>
        protected readonly IMap<TDataModel, TViewModel> map;
        /// <summary>
        /// Новая созданная запись, проставляется после маппинга и перед коммитом в базу в методе <see cref="TryCreate(ref TViewModel, ModelStateDictionary, User)"/>
        /// </summary>
        public TDataModel NewRecord { get; protected set; }
        /// <summary>
        /// Измененная запись, проставляется после маппинга и перед коммитом в базу в методе <see cref="TryUpdate(ref TViewModel, ModelStateDictionary, User)"/>
        /// </summary>
        public TDataModel ChangedRecord { get; protected set; }
        /// <summary>
        /// Удаляемая запись, проставляется перед проверкой полномочий на удаление в методе <see cref="TryDelete(string, ModelStateDictionary, User)"/>
        /// </summary>
        public TDataModel RecordToRemove { get; protected set; }
        /// <summary>
        /// Список с ошибками
        /// </summary>
        protected Dictionary<string, string> errors;
        #endregion

        #region Constructs
        public BaseRepository(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            // Вспомогательные сервисы
            IUserContextFactory userContextServices = serviceProvider.GetService<IUserContextFactory>();
            IUrlHelperFactory urlHelperFactory = serviceProvider.GetService<IUrlHelperFactory>();
            IActionContextAccessor actionAccessor = serviceProvider.GetService<IActionContextAccessor>();
            IMapFactory mapFactory = serviceProvider.GetService<IMapFactory>();

            // Фабрики
            TFFactory = serviceProvider.GetService<ITFFactory>();
            viewModelsTF = TFFactory.GetTransactionFactory<TViewModel>(serviceProvider, context);
            dataModelsTF = TFFactory.GetTransactionFactory<TDataModel>(serviceProvider, context);

            /// ActionContext может быть null, так как этот конструктор вызывается из <see cref="AccessibilityMiddleware"/>
            if (actionAccessor.ActionContext != null)
                urlHelper = urlHelperFactory.GetUrlHelper(actionAccessor.ActionContext);

            // Прочее
            this.serviceProvider = serviceProvider;
            this.context = context;
            httpContext = userContextServices.HttpContext;
            currentUser = httpContext.GetCurrentUser(context);
            map = mapFactory.GetMap<TDataModel, TViewModel>(serviceProvider, context);
            cachService = serviceProvider.GetService<ICachService>();
            resManager = serviceProvider.GetService<IResManager>();
            errors = new Dictionary<string, string>();
            dbSet = context.Set<TDataModel>();
        }
        #endregion

        #region Watch Items
        public virtual bool HasPermissionsForSeeItem(TDataModel dataModel) => false;
        #endregion

        #region Create Record
        public bool TryCreate(ref TViewModel entityToCreate, ModelStateDictionary modelState, User currentUser = null)
        {
            this.currentUser = currentUser ?? this.currentUser;

            // Создание и открытие транзакции
            transaction = viewModelsTF.Create(this.currentUser.Id, OperationType.Create, entityToCreate);

            // Проверка прав пользователя на совершение действия
            if (!RespsIsCorrectOnCreate(entityToCreate))
                HasNotPermissionsForCreate();
            else
            {
                // Подготовки записи к ее созданию
                if (TryCreatePrepare(entityToCreate))
                {
                    // Преобразование записи и ее добавление
                    TDataModel dataModel = map.OnModelCreate(entityToCreate);
                    transaction.AddChange(dataModel, EntityState.Added);
                    NewRecord = dataModel;
                    transaction.AddParameter("NewRecord", dataModel);
                    if (viewModelsTF.TryCommit(transaction, errors))
                    {
                        viewModelsTF.Close(transaction);
                        return true;
                    }
                }
            }

            // Добавление всех ошибок
            foreach (KeyValuePair<string, string> error in errors)
                modelState.AddModelError(error.Key, error.Value);

            // Закрытие транзакции и выход
            viewModelsTF.Close(transaction, TransactionStatus.Error);
            return false;
        }

        /// <summary>
        /// Метод проверяет возожность у сотрудника на создание записи
        /// </summary>
        /// <param name="entityToCreate"></param>
        /// <returns></returns>
        protected virtual bool RespsIsCorrectOnCreate(TViewModel entityToCreate) => false;

        /// <summary>
        /// Метод, обрабатывающий случай, когда у сотрудника недостаточно полномочий для создания записи
        /// </summary>
        /// <param name="modelState"></param>
        protected virtual void HasNotPermissionsForCreate() => AddHasNoPermissionsError(OperationType.Create);

        /// <summary>
        /// Попытка подготовить модель
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        protected virtual bool TryCreatePrepare(TViewModel viewModel) => true;
        #endregion

        #region Update Record
        public bool TryUpdate(ref TViewModel entityToUpdate, ModelStateDictionary modelState, User currentUser = null)
        {
            this.currentUser = currentUser ?? this.currentUser;

            // Создание и открытие транзакции
            transaction = viewModelsTF.Create(this.currentUser.Id, OperationType.Update, entityToUpdate);

            // Поиск записи
            Guid entityToUpdateId = entityToUpdate.Id;
            TDataModel recordToChange = dbSet.AsNoTracking().FirstOrDefault(i => i.Id == entityToUpdateId);
            if (recordToChange == null)
                OnRecordNotFound(entityToUpdate);
            else
            {
                // Проверка прав пользователя на совершение действия
                transaction.AddParameter("RecordToChange", recordToChange);
                if (RespsIsCorrectOnUpdate(entityToUpdate))
                {
                    // Попытка обновления записи
                    if (TryUpdatePrepare(entityToUpdate))
                    {
                        // Преобразование записи при обновлении(получение ее из бд и изменение значений полей)
                        TDataModel dataModel = map.OnModelUpdate(entityToUpdate);
                        try
                        {
                            // Обновление записи
                            transaction.AddChange(dataModel, EntityState.Modified);
                            ChangedRecord = dataModel;
                            transaction.AddParameter("ChangedRecord", dataModel);
                            if (viewModelsTF.TryCommit(transaction, errors))
                            {
                                // Закрытие транзакции
                                viewModelsTF.Close(transaction);
                                // Получение из бд обновленной записи, преобразование ее в модель отображения
                                entityToUpdate = map.DataToViewModel(dataModel);
                                return true;
                            }
                        }
                        catch (Exception ex)
                        {
                            errors.Add(resManager.GetString("UnhandledException"), ex.Message);
                        }
                    }
                }
                else HasNotPermissionsForUpdate();
            }

            // Добавление ошибок
            UpdateAddErrors(modelState);

            // Закрытие транзакции
            viewModelsTF.Close(transaction, TransactionStatus.Error);
            FailureUpdateHandler(entityToUpdate);
            return false;
        }

        /// <summary>
        /// Метод обрабатывает ошибку отсутствия записи
        /// </summary>
        /// <param name="entityToUpdate"></param>
        protected virtual void OnRecordNotFound(TViewModel entityToUpdate)
            => errors.Add("RecordNotFound", resManager.GetString("RecordNotFound"));

        /// <summary>
        /// Метод проверяет возожность у сотрудника на обновления записи
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        protected virtual bool RespsIsCorrectOnUpdate(TViewModel entityToUpdate) => false;

        /// <summary>
        /// Метод, обрабатывающий случай, когда у сотрудника недостаточно полномочий для обновления записи
        /// </summary>
        protected virtual void HasNotPermissionsForUpdate() => AddHasNoPermissionsError(OperationType.Update);

        /// <summary>
        /// Метод осуществляет проверку модели представления при обновлении записи
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        protected virtual bool TryUpdatePrepare(TViewModel viewModel) => true;

        protected virtual void FailureUpdateHandler(TViewModel viewModel) { }

        /// <summary>
        /// Метод выполняется при добавлении ошибок в модель
        /// </summary>
        /// <param name="modelState"></param>
        protected virtual void UpdateAddErrors(ModelStateDictionary modelState)
        {
            foreach (KeyValuePair<string, string> error in errors)
                modelState.AddModelError(error.Key, error.Value);
        }
        #endregion

        #region Delete Record
        public bool TryDelete(string id, ModelStateDictionary modelState, User currentUser = null)
        {
            // Создание транзакции
            this.currentUser = currentUser ?? this.currentUser;
            transaction = viewModelsTF.Create(this.currentUser.Id, OperationType.Delete, id);

            // Попытка распарсить id
            if (TryParseId(id, out Guid guid))
            {
                // Получение удаляемой сущности
                TDataModel entityToDelete = dbSet.FirstOrDefault(i => i.Id == guid);
                if (entityToDelete == null)
                    errors.Add("RecordNotFound", resManager.GetString("RecordNotFound"));
                else
                {
                    RecordToRemove = entityToDelete;
                    transaction.AddParameter("RecordToRemove", RecordToRemove);

                    // Обновление кеша в случае необходимости
                    UpdateCacheOnDelete(entityToDelete);

                    // Проверка полномочий
                    if (RespsIsCorrectOnDelete(entityToDelete))
                    {
                        // Выполнение подготовительных действий
                        if (TryDeletePrepare(entityToDelete))
                        {
                            // Попытка закоммитить
                            transaction.AddChange(entityToDelete, EntityState.Deleted);
                            if (viewModelsTF.TryCommit(transaction, errors))
                            {
                                viewModelsTF.Close(transaction);
                                return true;
                            }
                        }
                    }

                    // Обработка отсутствующия полномочий
                    else HasNotPermissionsForDelete();
                }
            }

            // Добавление ошибок
            foreach (KeyValuePair<string, string> error in errors)
                modelState.AddModelError(error.Key, error.Value);

            // Закрытие транзакции
            viewModelsTF.Close(transaction, TransactionStatus.Error);
            return false;
        }

        /// <summary>
        /// Метод удаляет уже найденную запись без проверки ее на существование и наличие полномочий у пользователя
        /// Вызывается из внешних источников
        /// </summary>
        /// <param name="entityToDelete"></param>
        /// <returns></returns>
        public bool TryDelete(TDataModel entityToDelete)
        {
            transaction = dataModelsTF.Create(currentUser.Id, OperationType.Delete, entityToDelete);

            // Выполнение подготовительных действий
            if (TryDeletePrepare(entityToDelete))
            {
                transaction.AddChange(entityToDelete, EntityState.Deleted);
                if (dataModelsTF.TryCommit(transaction, errors))
                {
                    dataModelsTF.Close(transaction);
                    return true;
                }
            }

            // Закрытие транзакции
            dataModelsTF.Close(transaction, TransactionStatus.Error);
            return false;
        }

        /// <summary>
        /// Метод вызывается перед проверкой полномочий на удаление методом <see cref="HasNotPermissionsForDelete"/> и после получения сущности, подлежащей удалению
        /// Можно переопределить, если необходимо обновить кеш перед операцией удаления сущности
        /// </summary>
        /// <param name="dataModel"></param>
        protected virtual void UpdateCacheOnDelete(TDataModel dataModel) { }

        /// <summary>
        /// Метод проверяет возожность у сотрудника на удаление записи
        /// </summary>
        /// <param name="entityToDelete"></param>
        /// <returns></returns>
        protected virtual bool RespsIsCorrectOnDelete(TDataModel entityToDelete) => false;

        /// <summary>
        /// Метод, обрабатывающий случай, когда у сотрудника недостаточно полномочий для удаления записи
        /// </summary>
        /// <param name="modelState"></param>
        protected virtual void HasNotPermissionsForDelete() => AddHasNoPermissionsError(OperationType.Delete);

        /// <summary>
        /// Метод осуществляет проверку модели при удалении записи
        /// </summary>
        /// <param name="dataModel"></param>
        /// <returns></returns>
        protected virtual bool TryDeletePrepare(TDataModel dataModel) => true;
        #endregion

        #region Other Methods
        public void SetViewInfo(string viewName, int pageNumber)
        {
            ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, viewName);
            viewInfo.CurrentPageNumber = pageNumber <= DEFAULT_MIN_PAGE_NUMBER ? DEFAULT_MIN_PAGE_NUMBER : pageNumber;
            viewInfo.SkipSteps = viewInfo.CurrentPageNumber - DEFAULT_PAGE_STEP;
            cachService.SetViewInfo(currentUser.Id, viewName, viewInfo);
        }

        public void SetViewInfo(string recordId, string viewName, int pageNumber)
            => SetViewInfo(Guid.Parse(recordId), viewName, pageNumber);

        public void SetViewInfo(Guid recordId, string viewName, int pageNumber)
        {
            ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, recordId, viewName);
            viewInfo.CurrentPageNumber = pageNumber <= DEFAULT_MIN_PAGE_NUMBER ? DEFAULT_MIN_PAGE_NUMBER : pageNumber;
            viewInfo.SkipSteps = viewInfo.CurrentPageNumber - DEFAULT_PAGE_STEP;
            cachService.SetViewInfo(currentUser.Id, recordId, viewName, viewInfo);
        }

        /// <summary>
        /// Метод ограничивает список элементов представления
        /// </summary>
        /// <typeparam name="TItemsListType"></typeparam>
        /// <param name="viewName">Название представления</param>
        /// <param name="itemsToLimit">Список элементов представления для ограничения</param>
        /// <returns></returns>
        protected void LimitViewItemsByPageNumber<TItemsListType>(string viewName, ref List<TItemsListType> itemsToLimit)
            where TItemsListType : class
        {
            ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, viewName);
            LimitListByPageNumber(viewInfo, ref itemsToLimit);
        }

        /// <summary>
        /// Метод ограничивает список элементов представления
        /// </summary>
        /// <typeparam name="TPartialViewItemType"></typeparam>
        /// <param name="recordId">Id записи</param>
        /// <param name="viewName">Название представления</param>
        /// <param name="itemsToLimit">Список элементов представления для ограничения</param>
        protected void LimitViewItemsByPageNumber<TPartialViewItemType>(Guid recordId, string viewName, ref List<TPartialViewItemType> itemsToLimit)
            where TPartialViewItemType : class
        {
            ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, recordId, viewName);
            LimitListByPageNumber(viewInfo, ref itemsToLimit);
        }

        private void LimitListByPageNumber<TItemsListType>(ViewInfo viewInfo, ref List<TItemsListType> itemsToLimit)
            where TItemsListType : class
        {
            List<TItemsListType> limitedItems = itemsToLimit.Skip(viewInfo.SkipSteps * viewInfo.ItemsCount).Take(viewInfo.ItemsCount).ToList();
            if (limitedItems.Count == 0)
            {
                int newSkipItemsCount = (viewInfo.SkipSteps - DEFAULT_PAGE_STEP) * viewInfo.ItemsCount;
                limitedItems = itemsToLimit.Skip(newSkipItemsCount).ToList();
                viewInfo.CurrentPageNumber--;
                viewInfo.SkipSteps -= DEFAULT_PAGE_STEP;
            }
            itemsToLimit = limitedItems;
        }

        public virtual TViewModel LoadView(TDataModel dataModel) => map.DataToViewModel(dataModel);

        /// <summary>
        /// Метод пытается преобразовать строковое проедставление id в Guid и найти запись
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public bool TryParseId(string id, out Guid guid)
        {
            guid = default;
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid guidId))
            {
                errors.Add("IdIsWrong", resManager.GetString("IdIsWrong"));
                return false;
            }
            if (dbSet.Find(guidId) == null)
            {
                errors.Add("RecordNotFound", resManager.GetString("RecordNotFound"));
                return false;
            }
            guid = guidId;
            return true;
        }

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
            return TryGetItemById(guid, out dataModel);
        }

        public bool TryGetRefItemById(string id, ref TDataModel dataModel)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid guid)) return false;
            return TryGetItemById(guid, out dataModel);
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

        protected void AddHasNoPermissionsError(OperationType operationType)
        {
            (string, string) errorInfo = GetErrorNotPermissionsInfo(operationType);
            errors.Add(errorInfo.Item1, errorInfo.Item2);
        }

        /// <summary>
        /// Метод возвращает кортеж из ключа и значения для стандартной обработки ошибки отсутствия полномочий
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        private (string, string) GetErrorNotPermissionsInfo(OperationType operationType)
            => (typeof(TViewModel).Name, operationType) switch
            {
                ("DivisionViewModel", OperationType.Create) => ("DivCreateNoRes", resManager.GetString("DivCreateNoRes")),
                ("DivisionViewModel", OperationType.Update) => ("DivUpdateNoRes", resManager.GetString("DivUpdateNoRes")),
                ("DivisionViewModel", OperationType.Delete) => ("DivDeleteNoRes", resManager.GetString("DivDeleteNoRes")),
                ("PositionViewModel", OperationType.Create) => ("PosCreateNoRes", resManager.GetString("PosCreateNoRes")),
                ("PositionViewModel", OperationType.Update) => ("PosUpdateNoRes", resManager.GetString("PosUpdateNoRes")),
                ("PositionViewModel", OperationType.Delete) => ("PosDeleteNoRes", resManager.GetString("PosDeleteNoRes")),
                ("PositionViewModel", OperationType.ChangePositionDivision) => ("ChangePositionDivisionNoRes", resManager.GetString("ChangePositionDivisionNoRes")),
                ("PositionViewModel", OperationType.UnlockPosition) => ("UnlockPositionNoRes", resManager.GetString("UnlockPositionNoRes")),
                ("EmployeeViewModel", OperationType.Create) => ("EmpCreateNoRes", resManager.GetString("EmpCreateNoRes")),
                ("EmployeeViewModel", OperationType.Update) => ("EmpUpdateNoRes", resManager.GetString("EmpUpdateNoRes")),
                ("EmployeeViewModel", OperationType.Delete) => ("EmpDeleteNoRes", resManager.GetString("EmpDeleteNoRes")),
                ("EmployeeViewModel", OperationType.ChangeEmployeeDivision) => ("ChangeEmployeeDivisionNoRes", resManager.GetString("ChangeEmployeeDivisionNoRes")),
                ("EmployeeViewModel", OperationType.UnlockEmployee) => ("UnlockEmployeeNoRes", resManager.GetString("UnlockEmployeeNoRes")),
                ("EmployeeContactViewModel", OperationType.Create) => ("EmpContactCreateNoRes", resManager.GetString("EmpContactCreateNoRes")),
                ("EmployeeContactViewModel", OperationType.Update) => ("EmpContactUpdateNoRes", resManager.GetString("EmpContactUpdateNoRes")),
                ("EmployeeContactViewModel", OperationType.Delete) => ("EmpContactDeleteNoRes", resManager.GetString("EmpContactDeleteNoRes")),
                ("EmployeePositionViewModel", OperationType.EmployeePositionsManagement) => ("EmpPossManagementNoRes", resManager.GetString("EmpPossManagementNoRes")),
                ("EmployeeResponsibilityViewModel", OperationType.EmployeeResponsibilitiesManagement) => ("EmpRespsManagementNoRes", resManager.GetString("EmpRespsManagementNoRes")),
                ("ResponsibilityViewModel", OperationType.Create) => ("RespCreateNoRes", resManager.GetString("RespCreateNoRes")),
                ("ResponsibilityViewModel", OperationType.Update) => ("RespUpdateNoRes", resManager.GetString("RespUpdateNoRes")),
                ("ResponsibilityViewModel", OperationType.Delete) => ("RespDeleteNoRes", resManager.GetString("RespDeleteNoRes")),
                ("AccountViewModel", OperationType.Create) => ("AccCreateNoRes", resManager.GetString("AccCreateNoRes")),
                ("AccountViewModel", OperationType.Update) => ("AccUpdateNoRes", resManager.GetString("AccUpdateNoRes")),
                ("AccountViewModel", OperationType.Delete) => ("AccDeleteNoRes", resManager.GetString("AccDeleteNoRes")),
                ("AccountViewModel", OperationType.ChangeAccountPrimaryContact) => ("AccUpdateNoRes", resManager.GetString("AccUpdateNoRes")),
                ("AccountViewModel", OperationType.ChangeAccountType) => ("AccChangeTypeNoRes", resManager.GetString("AccChangeTypeNoRes")),
                ("AccountViewModel", OperationType.UnlockAccount) => ("AccUnlockNoRes", resManager.GetString("AccUnlockNoRes")),
                ("AccountManagerViewModel", OperationType.AccountTeamManagement) => ("AccTeamManagementNoRes", resManager.GetString("AccTeamManagementNoRes")),
                ("AccountAddressViewModel", OperationType.ChangeAccountLegalAddress) => ("AccUpdateNoRes", resManager.GetString("AccUpdateNoRes")),
                ("AccountAddressViewModel", OperationType.Create) => ("AccAddressCreateNoRes", resManager.GetString("AccAddressCreateNoRes")),
                ("AccountAddressViewModel", OperationType.Update) => ("AccAddressUpdateNoRes", resManager.GetString("AccAddressUpdateNoRes")),
                ("AccountAddressViewModel", OperationType.Delete) => ("AccAddressDeleteNoRes", resManager.GetString("AccAddressDeleteNoRes")),
                ("AccountContactViewModel", OperationType.Create) => ("AccContactCreateNoRes", resManager.GetString("AccContactCreateNoRes")),
                ("AccountContactViewModel", OperationType.Update) => ("AccContactUpdateNoRes", resManager.GetString("AccContactUpdateNoRes")),
                ("AccountContactViewModel", OperationType.Delete) => ("AccContactDeleteNoRes", resManager.GetString("AccContactDeleteNoRes")),
                ("AccountInvoiceViewModel", OperationType.Create) => ("AccInvoiceCreateNoRes", resManager.GetString("AccInvoiceCreateNoRes")),
                ("AccountInvoiceViewModel", OperationType.Update) => ("AccInvoiceUpdateNoRes", resManager.GetString("AccInvoiceUpdateNoRes")),
                ("AccountInvoiceViewModel", OperationType.Delete) => ("AccInvoiceDeleteNoRes", resManager.GetString("AccInvoiceDeleteNoRes")),
                _ => ("NoRes", resManager.GetString("HasNoPermissions"))
            };
        #endregion
    }
}
