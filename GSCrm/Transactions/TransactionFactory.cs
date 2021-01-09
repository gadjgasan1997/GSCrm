using GSCrm.Data;
using GSCrm.Data.Cash;
using GSCrm.Factories;
using GSCrm.Helpers;
using GSCrm.Localization;
using GSCrm.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;

namespace GSCrm.Transactions
{
    public class TransactionFactory<TEntity> : ITransactionFactory<TEntity>
        where TEntity : IMainEntity
    {
        #region Declarations
        protected readonly IServiceProvider serviceProvider;
        protected readonly ICachService cachService;
        protected ITransaction transaction;
        protected readonly IResManager resManager;
        protected readonly User currentUser;
        protected readonly ApplicationDbContext context;
        /// <summary>
        /// Http context
        /// </summary>
        protected readonly HttpContext httpContext;
        /// <summary>
        /// Хелпер для работы с урлами
        /// </summary>
        protected readonly IUrlHelper urlHelper;
        /// <summary>
        /// Массив из базовых типов операций
        /// </summary>
        protected OperationType[] baseOperationTypes = new OperationType[] { OperationType.Create, OperationType.Update };
        /// <summary>
        /// Словарь со всеми транзакциями, в качестве ключа выступает id пользователя
        /// </summary>
        private static readonly Dictionary<string, List<ITransaction>> _transactions = new Dictionary<string, List<ITransaction>>();
        #endregion

        #region Constructs
        public TransactionFactory(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            this.serviceProvider = serviceProvider;
            resManager = serviceProvider.GetService(typeof(IResManager)) as IResManager;
            cachService = serviceProvider.GetService(typeof(ICachService)) as ICachService;
            this.context = context;
            IUserContextFactory userContextServices = serviceProvider.GetService(typeof(IUserContextFactory)) as IUserContextFactory;
            httpContext = userContextServices.HttpContext;
            currentUser = httpContext.GetCurrentUser(context);
            urlHelper = serviceProvider.GetService(typeof(IUrlHelper)) as IUrlHelper;
        }
        #endregion

        #region Create
        public ITransaction Create(string userId, OperationType operationType, TEntity entity)
        {
            CreateTransaction(operationType, userId);
            CreateHandler(operationType, entity);
            return transaction;
        }

        public ITransaction Create(string userId, OperationType operationType, string recordId)
        {
            CreateTransaction(operationType, userId);
            CreateHandler(operationType, recordId);
            return transaction;
        }

        public ITransaction Create(string userId, OperationType operationType)
        {
            CreateTransaction(operationType, userId);
            CreateHandler(operationType);
            return transaction;
        }

        public ITransaction Create(OperationType operationType, TEntity entity)
        {
            CreateTransaction(operationType);
            CreateHandler(operationType);
            return transaction;
        }

        private void CreateTransaction(OperationType operationType, string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = Guid.NewGuid().ToString();
            transaction = new Transaction(serviceProvider, context);
            transaction.UserId = userId;
            transaction.Name = GetTransactionName(operationType);
            transaction.OperationType = operationType;
            transaction.TransactionStatus = TransactionStatus.Success;
            if (!_transactions.ContainsKey(userId))
                _transactions.Add(userId, new List<ITransaction>() { transaction });
            else if (_transactions[userId].FirstOrDefault(i => i.Name == transaction.Name) == null)
                _transactions[userId].Add(transaction);
        }

        protected virtual void CreateHandler(OperationType operationType, TEntity entity) { }

        protected virtual void CreateHandler(OperationType operationType, string recordId) { }

        protected virtual void CreateHandler(OperationType operationType) { }
        #endregion

        #region Close
        public void Close(ITransaction transaction, TransactionStatus transactionStatus = TransactionStatus.None)
        {
            if (!_transactions.ContainsKey(transaction.UserId)) return;
            ITransaction transation = _transactions[transaction.UserId].FirstOrDefault(i => i.Name == transaction.Name);
            if (transation != null)
            {
                // В случае, если не был подан никакой статус, то транзакция закрывается с текущим
                transactionStatus = transactionStatus == TransactionStatus.None ? transaction.TransactionStatus : transactionStatus;
                CloseHandler(transactionStatus, transation.OperationType);
                _transactions.GetValueOrDefault(transaction.UserId).Remove(transation);
                return;
            }
        }

        protected virtual void CloseHandler(TransactionStatus transactionStatus, OperationType operationType) { }
        #endregion

        #region Other
        public virtual bool TryCommit(ITransaction transaction, Dictionary<string, string> errors)
        {
            BeforeCommit(transaction.OperationType);
            using IDbContextTransaction efTransaction = context.Database.BeginTransaction();
            try
            {
                foreach (TransactionChange change in transaction.GetChanges())
                    context.Entry(change.Entity).State = change.EntityState;
                context.SaveChanges();
                efTransaction.Commit();
                transaction.TransactionStatus = TransactionStatus.Success;
                return true;
            }
            catch (Exception ex)
            {
#if DEBUG
                errors.Add("UnhandledException", ex.InnerException?.Message ?? ex.Message);
#else
                errors.Add("UnhandledException", resManager.GetString("UnhandledException"));
#endif
                efTransaction.Rollback();
                transaction.TransactionStatus = TransactionStatus.Error;
                return false;
            }
        }

        protected virtual void BeforeCommit(OperationType operationType) { }

        public ITransaction GetTransaction(string userId, OperationType operationType)
        {
            if (!_transactions.ContainsKey(userId)) return null;
            return _transactions[userId].FirstOrDefault(n => n.Name == GetTransactionName(operationType));
        }

        /// <summary>
        /// Возвращает название транзакции исходя из типа сущности, для которой она создается и типа текущей операции
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        private string GetTransactionName(OperationType operationType)
            => (typeof(TEntity).Name, operationType) switch
            {
                ("OrganizationViewModel", OperationType.ChangePrimaryOrganization) => "ChangePrimaryOrganization",
                ("OrganizationViewModel", OperationType.LeaveOrganization) => "LeaveOrganization",
                ("DivisionViewModel", OperationType.Create) => "DivisionCreate",
                ("DivisionViewModel", OperationType.Update) => "DivisionUpdate",
                ("DivisionViewModel", OperationType.Delete) => "DivisionDelete",
                ("PositionViewModel", OperationType.Create) => "PositionCreate",
                ("PositionViewModel", OperationType.Update) => "PositionUpdate",
                ("PositionViewModel", OperationType.Delete) => "PositionDelete",
                ("PositionViewModel", OperationType.ChangePositionDivision) => "ChangePositionDivision",
                ("PositionViewModel", OperationType.UnlockPosition) => "UnlockPosition",
                ("EmployeeViewModel", OperationType.Create) => "EmployeeCreate",
                ("EmployeeViewModel", OperationType.Update) => "EmployeeUpdate",
                ("EmployeeViewModel", OperationType.Delete) => "EmployeeDelete",
                ("EmployeeViewModel", OperationType.ChangeEmployeeDivision) => "ChangeEmployeeDivision",
                ("EmployeeViewModel", OperationType.UnlockEmployee) => "UnlockEmployee",
                ("EmployeeContactViewModel", OperationType.Create) => "EmployeeContactCreate",
                ("EmployeeContactViewModel", OperationType.Update) => "EmployeeContactUpdate",
                ("EmployeeContactViewModel", OperationType.Delete) => "EmployeeContactDelete",
                ("EmployeePositionViewModel", OperationType.EmployeePositionsManagement) => "PositionEmployeePositionsManagement",
                ("EmployeeResponsibilityViewModel", OperationType.EmployeeResponsibilitiesManagement) => "EmployeeResponsibilitiesManagement",
                ("ResponsibilityViewModel", OperationType.Create) => "ResponsibilityCreate",
                ("ResponsibilityViewModel", OperationType.Update) => "ResponsibilityUpdate",
                ("ResponsibilityViewModel", OperationType.Delete) => "ResponsibilityDelete",
                ("AccountViewModel", OperationType.Create) => "AccountCreate",
                ("AccountViewModel", OperationType.Update) => "AccountUpdate",
                ("AccountViewModel", OperationType.Delete) => "AccountDelete",
                ("AccountViewModel", OperationType.ChangeAccountLegalAddress) => "ChangeAccountLegalAddress",
                ("AccountViewModel", OperationType.ChangeAccountPrimaryContact) => "ChangeAccountPrimaryContact",
                ("AccountViewModel", OperationType.ChangeAccountType) => "ChangeAccountType",
                ("AccountViewModel", OperationType.UnlockAccount) => "UnlockAccount",
                ("AccountManagerViewModel", OperationType.AccountTeamManagement) => "AccountTeamManagement",
                ("AccountAddressViewModel", OperationType.Create) => "AccountAddressCreate",
                ("AccountAddressViewModel", OperationType.Update) => "AccountAddressUpdate",
                ("AccountAddressViewModel", OperationType.Delete) => "AccountAddressDelete",
                ("AccountContactViewModel", OperationType.Create) => "AccountContactCreate",
                ("AccountContactViewModel", OperationType.Update) => "AccountContactUpdate",
                ("AccountContactViewModel", OperationType.Delete) => "AccountContactDelete",
                ("AccountInvoiceViewModel", OperationType.Create) => "AccountInvoiceCreate",
                ("AccountInvoiceViewModel", OperationType.Update) => "AccountInvoiceUpdate",
                ("AccountInvoiceViewModel", OperationType.Delete) => "AccountInvoiceDelete",
                ("UserNotificationsSettingViewModel", OperationType.Update) => "UserNotificationsSettingUpdate",
                ("OrgNotificationsSettingViewModel", OperationType.Update) => "OrgNotificationsSettingUpdate",
                ("OrgNotificationsSettingViewModel", OperationType.InitNotSetting) => "OrgNotificationsSettingUpdate",
                ("Notification", OperationType.SendNotification) => "SendNotification",
                ("UserNotification", OperationType.Update) => "UpdateUserNotification",
                ("UserNotificationViewModel", OperationType.Delete) => "DeleteUserNotification",
                ("UserViewModel", OperationType.Register) => "RegisterUser",
                ("UserViewModel", OperationType.Login) => "LoginUser",
                ("UserViewModel", OperationType.ResetPasswordSpecifyEmail) => "ResetUserPasswordSpecifyEmail",
                ("UserViewModel", OperationType.ResetPassword) => "ResetUserPassword",
                _ => string.Empty
            };
        #endregion
    }
}
