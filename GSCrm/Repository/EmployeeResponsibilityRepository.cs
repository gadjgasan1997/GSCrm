using GSCrm.Mapping;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using static GSCrm.CommonConsts;
using GSCrm.Data;
using GSCrm.Transactions;
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Repository
{
    public class EmployeeResponsibilityRepository
        : BaseRepository<EmployeeResponsibility, EmployeeResponsibilityViewModel>
    {
        #region Declarations
        /// <summary>
        /// Ошибки, возникшие при синхронизации списка полномочий
        /// </summary>
        private readonly Dictionary<string, string> syncErrors = new Dictionary<string, string>();
        /// <summary>
        /// Полномочия, которые пользователь выбрал для добавления в список полномочий сотрудника
        /// </summary>
        private readonly List<EmployeeResponsibility> respsToAdd = new List<EmployeeResponsibility>();
        /// <summary>
        /// Полномочия сотрудника, которые пользователь хочет удалить из его списка полномочий
        /// </summary>
        private readonly List<EmployeeResponsibility> respsToRemove = new List<EmployeeResponsibility>();
        /// <summary>
        /// Количество одновременно отоброжаемых полномочий из списка всех полномочий организации
        /// </summary>
        private const int ALL_EMP_RESPS_COUNT = 5;
        /// <summary>
        /// Количество одновременно отоброжаемых полномочий из списка всех полномочий сотрудника
        /// </summary>
        private const int SELECTED_EMP_RESPS_COUNT = 5;
        /// <summary>
        /// Транзакция для синхронизации должностей
        /// </summary>
        private ITransaction syncRespsTransaction;
        private readonly ITransactionFactory<SyncRespsViewModel> syncRespsTransactionFactory;
        #endregion

        #region Constructs
        public EmployeeResponsibilityRepository(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(serviceProvider, context)
        {
            syncRespsTransactionFactory = TFFactory.GetTransactionFactory<SyncRespsViewModel>(serviceProvider, context);
        }
        #endregion

        #region Override 
        #endregion

        #region Searching
        /// <summary>
        /// Метод очищает поиск по всем полномочиям
        /// </summary>
        public void ClearAllResponsibilitiesSearch()
        {
            EmployeeViewModel employeeViewModelCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, ALL_EMP_RESPS);
            employeeViewModelCash.SearchAllRespName = default;
            cachService.CacheItem(currentUser.Id, ALL_EMP_RESPS, employeeViewModelCash);
        }

        /// <summary>
        /// Метод очищает поиск по выбранным полномочиям
        /// </summary>
        public void ClearSelectedResponsibilitiesSearch()
        {
            EmployeeViewModel employeeViewModelCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, SELECTED_EMP_RESPS);
            employeeViewModelCash.SearchSelectedRespName = default;
            cachService.CacheItem(currentUser.Id, SELECTED_EMP_RESPS, employeeViewModelCash);
        }
        #endregion

        #region Attaching All Responsibilities
        /// <summary>
        /// Метод возвращает список всех полномочий организации
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public List<Responsibility> AttachAllResponsibilities(Guid employeeId, int pageNumber = DEFAULT_MIN_PAGE_NUMBER)
        {
            SetViewInfo(currentUser.Id, ALL_EMP_RESPS, pageNumber, ALL_EMP_RESPS_COUNT);

            // Получение списка всех полномочий организации за исключением тех, которые уже присутствуют у сотрудника
            Employee employee = context.Employees.FirstOrDefault(i => i.Id == employeeId);
            // Выбранные полномочия сотрудника
            List<Responsibility> selectedResponsibilities = context.EmployeeResponsibilities
                .AsNoTracking()
                .Include(resp => resp.Responsibility)
                .Where(emp => emp.EmployeeId == employeeId)
                .Select(resp => resp.Responsibility).ToList();

            // Все полномочия сотрудника
            List<Responsibility> allResponsibilities = context.Responsibilities
                .AsNoTracking()
                .Where(org => org.OrganizationId == employee.GetOrganization(context).Id).ToList()
                .Except(selectedResponsibilities, new ResponsibilityComparer()).ToList();

            // Ограничение по фильтрам и номеру страницы
            EmployeeViewModel employeeViewModelCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, ALL_EMP_RESPS);
            LimitAllRespsByName(employeeViewModelCash, ref allResponsibilities);
            LimitListByPageNumber(ALL_EMP_RESPS, ref allResponsibilities, ALL_EMP_RESPS_COUNT);
            return allResponsibilities;
        }


        /// <summary>
        /// Ограничение списка всех полномочий организации по названию
        /// </summary>
        /// <param name="employeeViewModelCash"></param>
        /// <param name="employeeResponsibilities"></param>
        private void LimitAllRespsByName(EmployeeViewModel employeeViewModelCash, ref List<Responsibility> employeeResponsibilities)
        {
            if (!string.IsNullOrEmpty(employeeViewModelCash.SearchAllRespName))
                employeeResponsibilities = employeeResponsibilities.Where(n => n.Name.ToLower().Contains(employeeViewModelCash.SearchAllRespName)).ToList();
        }
        #endregion

        #region Attaching Selected Responsibilities
        /// <summary>
        /// Метод возвращает список всех полномочий сотрудника
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public List<Responsibility> AttachSelectedResponsibilities(Guid employeeId, int pageNumber = DEFAULT_MIN_PAGE_NUMBER)
        {
            SetViewInfo(currentUser.Id, SELECTED_EMP_RESPS, pageNumber, SELECTED_EMP_RESPS_COUNT);

            // Получение списка всех полномочий сотрудника и ограничение по фильтрам и номеру страницы
            List<EmployeeResponsibility> selectedResponsibilities = context.EmployeeResponsibilities
                .AsNoTracking()
                .Include(resp => resp.Responsibility)
                .Where(emp => emp.EmployeeId == employeeId).ToList();
            EmployeeViewModel employeeViewModelCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, SELECTED_EMP_RESPS);
            LimitSelectedRespsByName(employeeViewModelCash, ref selectedResponsibilities);
            LimitListByPageNumber(SELECTED_EMP_RESPS, ref selectedResponsibilities, SELECTED_EMP_RESPS_COUNT);
            return selectedResponsibilities.Select(resp => resp.Responsibility).ToList();
        }

        /// <summary>
        /// Ограничение списка выбранных полномочий сотрудника по названию
        /// </summary>
        /// <param name="employeeViewModelCash"></param>
        /// <param name="employeeResponsibilities"></param>
        private void LimitSelectedRespsByName(EmployeeViewModel employeeViewModelCash, ref List<EmployeeResponsibility> employeeResponsibilities)
        {
            if (!string.IsNullOrEmpty(employeeViewModelCash.SearchSelectedRespName))
                employeeResponsibilities = employeeResponsibilities.Where(n => n.Responsibility.Name.ToLower().Contains(employeeViewModelCash.SearchSelectedRespName)).ToList();
        }
        #endregion

        #region Other Methods
        public bool TrySyncResponsibilities(SyncRespsViewModel syncViewModel, ref Dictionary<string, string> errors)
        {
            syncRespsTransaction = syncRespsTransactionFactory.Create(currentUser.Id, OperationType.EmployeeResponsibilitiesManagement, syncViewModel);
            Employee employee = context.Employees
                .AsNoTracking()
                .Include(empResp => empResp.EmployeeResponsibilities)
                    .ThenInclude(resp => resp.Responsibility)
                .FirstOrDefault(i => i.Id == syncViewModel.EmployeeId);

            InvokeIntermittinActions(this.errors, new List<Action>()
            {
                () => {
                    if (!new OrganizationRepository(serviceProvider, context).CheckPermissionForEmployeeGroup("EmpRespsManagement", syncRespsTransaction))
                         AddHasNoPermissionsError(OperationType.EmployeeResponsibilitiesManagement);
                },
                () => {
                    FormAddRespsList(syncViewModel.ResponsibilitiesToAdd, employee);
                },
                () => {
                    FormRemoveRespsList(syncViewModel.ResponsibilitiesToRemove, employee);
                }
            });
            if (!this.errors.Any())
            {
                respsToAdd.ForEach(respToAdd => syncRespsTransaction.AddChange(respToAdd, EntityState.Added));
                respsToRemove.ForEach(respToRemove => syncRespsTransaction.AddChange(respToRemove, EntityState.Deleted));
                if (syncRespsTransactionFactory.TryCommit(syncRespsTransaction, this.errors))
                {
                    syncRespsTransactionFactory.Close(syncRespsTransaction);
                    return true;
                }
            }
            errors = this.errors;
            syncRespsTransactionFactory.Close(syncRespsTransaction, TransactionStatus.Error);
            return false;
        }

        /// <summary>
        /// Формирует список полномочий для добавления
        /// </summary>
        /// <param name="respsToAdd"></param>
        /// <param name="employee"></param>
        private void FormAddRespsList(List<string> respsToAdd, Employee employee)
        {
            List<Responsibility> orgResponsibilities = employee.GetOrganization(context).GetResponsibilities(context);
            respsToAdd.ForEach(respId =>
            {
                if (Guid.TryParse(respId, out Guid guid))
                {
                    Responsibility responsibility = orgResponsibilities.FirstOrDefault(n => n.Id == guid);

                    // Если полномочие найдено
                    if (responsibility != null)
                    {
                        // Если полномочие уже присутствует в списке полномочий сотрудника
                        if (employee.EmployeeResponsibilities.Select(resp => resp.ResponsibilityId).Contains(guid))
                        {
                            syncErrors.Add("RespIsAlreadyAdded", resManager.GetString("RespIsAlreadyAdded"));
                            return;
                        }

                        // Если оно уже не было добавлено в список полномочий для добавления
                        if (!this.respsToAdd.Select(n => n.ResponsibilityId).Contains(guid))
                        {
                            // Необходимо сделать состояние полномочия не измененным, иначе Entity Framework попробует создать его заново
                            context.Entry(responsibility).State = EntityState.Unchanged;
                            this.respsToAdd.Add(new EmployeeResponsibility()
                            {
                                Id = Guid.NewGuid(),
                                Employee = employee,
                                EmployeeId = employee.Id,
                                Responsibility = responsibility,
                                ResponsibilityId = responsibility.Id
                            });
                        }
                    }
                    else syncErrors.Add("AddRespNotExists", resManager.GetString("AddRespNotExists"));
                }
                else syncErrors.Add(resManager.GetString("UnhandledException"), "UnhandledException");
            });
        }

        /// <summary>
        /// Формирует список полномочий для удаления
        /// </summary>
        /// <param name="respsToRemove"></param>
        /// <param name="employee"></param>
        private void FormRemoveRespsList(List<string> respsToRemove, Employee employee)
        {
            respsToRemove.ForEach(respId =>
            {
                if (Guid.TryParse(respId, out Guid guid))
                {
                    EmployeeResponsibility employeeResponsibility = employee.EmployeeResponsibilities.FirstOrDefault(n => n.ResponsibilityId == guid);

                    // Если полномочие найдено и оно уже не было добавлено в список полномочий
                    if (employeeResponsibility != null)
                    {
                        if (!this.respsToRemove.Select(resp => resp.ResponsibilityId).Contains(guid))
                            this.respsToRemove.Add(employeeResponsibility);
                    }
                    else syncErrors.Add("RemoveRespNotExists", resManager.GetString("RemoveRespNotExists"));
                }
                else syncErrors.Add(resManager.GetString("UnhandledException"), "UnhandledException");
            });
        }
        #endregion
    }
}
