using GSCrm.Mapping;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using GSCrm.Data;
using GSCrm.Transactions;
using GSCrm.Models.Enums;
using GSCrm.Data.ApplicationInfo;
using static GSCrm.CommonConsts;
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
            if (cachService.TryGetEntityCache(currentUser, out EmployeeViewModel employeeViewModelCash, ALL_EMP_RESPS))
            {
                employeeViewModelCash.SearchAllRespName = default;
                cachService.AddOrUpdate(currentUser, ALL_EMP_RESPS, employeeViewModelCash);
            }
        }

        /// <summary>
        /// Метод очищает поиск по выбранным полномочиям
        /// </summary>
        public void ClearSelectedResponsibilitiesSearch()
        {
            if (cachService.TryGetEntityCache(currentUser, out EmployeeViewModel employeeViewModelCash, SELECTED_EMP_RESPS))
            {
                employeeViewModelCash.SearchSelectedRespName = default;
                cachService.AddOrUpdate(currentUser, SELECTED_EMP_RESPS, employeeViewModelCash);
            }
        }
        #endregion

        #region Attaching All Responsibilities
        /// <summary>
        /// Метод возвращает список всех полномочий организации
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="employeeViewModelCash"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public List<Responsibility> GetAllResponsibilities(Employee employee, EmployeeViewModel employeeViewModelCash, int pageNumber = DEFAULT_MIN_PAGE_NUMBER)
        {
            SetViewInfo(ALL_EMP_RESPS, pageNumber);

            // Выбранные полномочия сотрудника
            List<Responsibility> selectedResponsibilities = context.EmployeeResponsibilities
                .AsNoTracking()
                .Include(resp => resp.Responsibility)
                .Where(emp => emp.EmployeeId == employee.Id)
                .Select(resp => resp.Responsibility).ToList();

            // Все полномочия сотрудника
            List<Responsibility> allResponsibilities = context.Responsibilities
                .AsNoTracking()
                .Where(org => org.OrganizationId == employee.GetOrganization(context).Id).ToList()
                .Except(selectedResponsibilities, new ResponsibilityComparer()).ToList();

            // Ограничение по фильтрам и номеру страницы
            LimitAllRespsByName(employeeViewModelCash, ref allResponsibilities);
            LimitListByPageNumber(ALL_EMP_RESPS, ref allResponsibilities);
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
        /// <param name="employee"></param>
        /// <param name="employeeViewModelCash"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public List<Responsibility> GetSelectedResponsibilities(Employee employee, EmployeeViewModel employeeViewModelCash, int pageNumber = DEFAULT_MIN_PAGE_NUMBER)
        {
            SetViewInfo(SELECTED_EMP_RESPS, pageNumber);

            // Получение списка всех полномочий сотрудника и ограничение по фильтрам и номеру страницы
            List<EmployeeResponsibility> selectedResponsibilities = context.EmployeeResponsibilities
                .AsNoTracking()
                .Include(resp => resp.Responsibility)
                .Where(emp => emp.EmployeeId == employee.Id).ToList();
            LimitSelectedRespsByName(employeeViewModelCash, ref selectedResponsibilities);
            LimitListByPageNumber(SELECTED_EMP_RESPS, ref selectedResponsibilities);
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
            syncRespsTransaction.AddParameter("Employee", employee);

            // Проверки
            InvokeIntermittinActions(this.errors, new List<Action>()
            {
                () => {
                    if (!new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("EmpRespsManagement", syncRespsTransaction))
                         AddHasNoPermissionsError(OperationType.EmployeeResponsibilitiesManagement);
                },
                () => {
                    FormAddRespsList(syncViewModel.ResponsibilitiesToAdd, employee);
                },
                () => {
                    FormRemoveRespsList(syncViewModel.ResponsibilitiesToRemove, employee);
                }
            });

            // Если не было ошибок, выполняется обновление списка полномочий
            if (!this.errors.Any())
            {
                respsToAdd.ForEach(respToAdd => syncRespsTransaction.AddChange(respToAdd, EntityState.Added));
                respsToRemove.ForEach(respToRemove => syncRespsTransaction.AddChange(respToRemove, EntityState.Deleted));

                // Попытка сделать коммит
                if (syncRespsTransactionFactory.TryCommit(syncRespsTransaction, this.errors))
                {
                    syncRespsTransactionFactory.Close(syncRespsTransaction);
                    return true;
                }
            }

            // Добавление ошибок и выход
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

        /// <summary>
        /// Вызывается для пролистывания всех полномочий
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="navigateDirection">Направление пролистывания</param>
        /// <returns></returns>
        public List<ResponsibilityViewModel> NavigateGetAllRecords(Employee employee, NavigateDirection navigateDirection)
        {
            ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, ALL_EMP_RESPS);
            EmployeeResponsibilityRepository responsibilityRepository = new EmployeeResponsibilityRepository(serviceProvider, context);
            if (!cachService.TryGetEntityCache(currentUser, out EmployeeViewModel allEmployeeRespsCash, ALL_EMP_RESPS))
                allEmployeeRespsCash = new EmployeeViewModel();
            int pageNumber = viewInfo.GetNewPageNumber(navigateDirection);
            List<Responsibility> allResponsibilities = responsibilityRepository.GetAllResponsibilities(employee, allEmployeeRespsCash, pageNumber);
            return allResponsibilities.GetViewModelsFromData(new ResponsibilityMap(serviceProvider, context));
        }

        /// <summary>
        /// Вызывается для пролистывания выбранных полномочий пользователя
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="navigateDirection">Направление пролистывания</param>
        /// <returns></returns>
        public List<ResponsibilityViewModel> NavigateGetSelectedRecords(Employee employee, NavigateDirection navigateDirection)
        {
            ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, SELECTED_EMP_RESPS);
            EmployeeResponsibilityRepository responsibilityRepository = new EmployeeResponsibilityRepository(serviceProvider, context);
            if (!cachService.TryGetEntityCache(currentUser, out EmployeeViewModel selectedEmployeeRespsCash, SELECTED_EMP_RESPS))
                selectedEmployeeRespsCash = new EmployeeViewModel();
            int pageNumber = viewInfo.GetNewPageNumber(navigateDirection);
            List<Responsibility> selectedResponsibilities = responsibilityRepository.GetSelectedResponsibilities(employee, selectedEmployeeRespsCash, pageNumber);
            return selectedResponsibilities.GetViewModelsFromData(new ResponsibilityMap(serviceProvider, context));
        }
        #endregion
    }
}
