using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using static GSCrm.CommonConsts;
using static GSCrm.Utils.CollectionsUtils;
using GSCrm.Data;
using GSCrm.Transactions;
using GSCrm.Models.Enums;

namespace GSCrm.Repository
{
    public class AccountManagerRepository : BaseRepository<AccountManager, AccountManagerViewModel>
    {
        #region Declarations
        private readonly AccountRepository accountRepository;
        /// <summary>
        /// Количество одновременно отоброжаемых сотрудников организации, создавшей клиента
        /// </summary>
        private const int ALL_EMPLOYEES_COUNT = 5;
        /// <summary>
        /// Количество одновременно отоброжаемых 
        /// </summary>
        private const int SELECTED_EMPLOYEES_COUNT = 5;
        /// <summary>
        /// Транзакция для синхронизации команды по клиенту
        /// </summary>
        private ITransaction syncRespsTransaction;
        private readonly ITransactionFactory<SyncAccountViewModel> syncRespsTransactionFactory;
        #endregion

        #region Constructs
        public AccountManagerRepository(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(serviceProvider, context)
        {
            accountRepository = new AccountRepository(serviceProvider, context);
            syncRespsTransactionFactory = TFFactory.GetTransactionFactory<SyncAccountViewModel>(serviceProvider, context);
        }
        #endregion

        #region Searching
        /// <summary>
        /// Метод сбрасывает фильтрацию для списка всех сотрудников организации, создавшей клиента
        /// </summary>
        public void ClearAllManagersSearch()
        {
            AccountViewModel accountViewModelCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES);
            accountViewModelCash.SearchAllManagersName = default;
            accountViewModelCash.SearchAllManagersDivision = default;
            accountViewModelCash.SearchAllManagersPosition = default;
            cachService.CacheItem(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES, accountViewModelCash);
        }

        /// <summary>
        /// Метод сбрасывает фильтрацию для команды по клиенту
        /// </summary>
        public void ClearSelectedManagersSearch()
        {
            AccountViewModel accountViewModelCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_TEAM_SELECTED_EMPLOYEES);
            accountViewModelCash.SearchSelectedManagersName = default;
            accountViewModelCash.SearchSelectedManagersPhone = default;
            accountViewModelCash.SearchSelectedManagersPosition = default;
            cachService.CacheItem(currentUser.Id, ACC_TEAM_SELECTED_EMPLOYEES, accountViewModelCash);
        }
        #endregion

        #region Attaching All Employees
        /// <summary>
        /// Метод возвращает список всех сотрудников организации для отображения в окне управления командой по клиенту
        /// </summary>
        /// <returns></returns>
        public List<Employee> AttachTeamAllEmployees(string accountId, int pageNumber = DEFAULT_MIN_PAGE_NUMBER)
        {
            if (accountRepository.TryGetItemById(accountId, out Account account))
            {
                SetViewInfo(ACC_TEAM_ALL_EMPLOYEES, pageNumber, ALL_EMPLOYEES_COUNT);

                List<Employee> teamAllEmployees = context.GetOrgEmployees(account.OrganizationId);
                AccountViewModel accountViewModelCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES);
                ExcludeSelectedEmployees(ref teamAllEmployees, account.Id);
                LimitAllEmployeesByName(ref teamAllEmployees, accountViewModelCash);
                LimitAllEmployeesByDivision(ref teamAllEmployees, accountViewModelCash);
                LimitAllEmployeesByPosition(ref teamAllEmployees, accountViewModelCash);
                LimitListByPageNumber(ACC_TEAM_ALL_EMPLOYEES, ref teamAllEmployees);
                return teamAllEmployees;
            }
            return new List<Employee>();
        }

        /// <summary>
        /// Метод исключает текущую команду по клиенту из списка всех доступных сотрудников
        /// </summary>
        /// <param name="teamAllEmployees"></param>
        private void ExcludeSelectedEmployees(ref List<Employee> teamAllEmployees, Guid accountId)
        {
            List<AccountManager> teamSelectedManagers = context.AccountManagers
                    .Include(man => man.Manager)
                    .Where(accId => accId.AccountId == accountId).ToList();
            List<Employee> teamSelectedEmployees = teamSelectedManagers.Select(man => man.Manager).ToList();
            teamAllEmployees = teamAllEmployees.Except(teamSelectedEmployees, new EmployeeComparer()).ToList();
        }

        /// <summary>
        /// Мето ограничивает список всех сотрудников по имени
        /// </summary>
        /// <param name="employeesToLimit"></param>
        private void LimitAllEmployeesByName(ref List<Employee> employeesToLimit, AccountViewModel accountViewModelCash)
        {
            if (!string.IsNullOrEmpty(accountViewModelCash.SearchAllManagersName))
            {
                Func<Employee, bool> predicate = n => n.GetFullName().ToLower().Contains(accountViewModelCash.SearchAllManagersName.ToLower());
                employeesToLimit = employeesToLimit.Where(predicate).ToList();
            }
        }

        /// <summary>
        /// Мето ограничивает список всех сотрудников по подразделению
        /// </summary>
        /// <param name="employeesToLimit"></param>
        private void LimitAllEmployeesByDivision(ref List<Employee> employeesToLimit, AccountViewModel accountViewModelCash)
        {
            if (!string.IsNullOrEmpty(accountViewModelCash.SearchAllManagersDivision))
            {
                TransformCollection(
                    collectionToLimit: ref employeesToLimit,
                    limitingCollection: context.GetOrgDivisions(accountViewModelCash.OrganizationId),
                    limitCondition: n => n.Name.ToLower().Contains(accountViewModelCash.SearchAllManagersDivision),
                    selectCondition: i => i.Id,
                    removeCondition: (divisionIdList, employee) => !divisionIdList.Contains((Guid)employee.DivisionId));
            }
        }

        /// <summary>
        /// Мето ограничивает список всех сотрудников по должности
        /// </summary>
        /// <param name="employeesToLimit"></param>
        private void LimitAllEmployeesByPosition(ref List<Employee> employeesToLimit, AccountViewModel accountViewModelCash)
        {
            if (!string.IsNullOrEmpty(accountViewModelCash.SearchAllManagersPosition))
            {
                TransformCollection(
                    collectionToLimit: ref employeesToLimit,
                    limitingCollection: context.GetOrgPositions(accountViewModelCash.OrganizationId),
                    limitCondition: n => n.Name.ToLower().Contains(accountViewModelCash.SearchAllManagersPosition),
                    selectCondition: i => i.Id,
                    removeCondition: (positionIdList, employee) => employee.PrimaryPositionId == null || !positionIdList.Contains((Guid)employee.PrimaryPositionId));
            }
        }
        #endregion

        #region Attaching Selected Employees
        /// <summary>
        /// Метод возвращает список менеджеров клиента для отображения в окне управления командой
        /// </summary>
        /// <returns></returns>
        public List<AccountManager> GetTeamSelectedEmployees(string accountId, int pageNumber = DEFAULT_MIN_PAGE_NUMBER)
        {
            if (accountRepository.TryGetItemById(accountId, out Account account))
            {
                SetViewInfo(ACC_TEAM_SELECTED_EMPLOYEES, pageNumber, SELECTED_EMPLOYEES_COUNT);

                AccountViewModel accountViewModelCash = cachService.GetCachedItem<AccountViewModel>(currentUser.Id, ACC_TEAM_SELECTED_EMPLOYEES);
                List<AccountManager> teamSelectedEmployees = context.AccountManagers
                    .Include(man => man.Manager)
                    .Where(accId => accId.AccountId == account.Id).ToList();
                LimitSelectedEmployeesByName(ref teamSelectedEmployees, accountViewModelCash);
                LimitSelectedEmployeesByPosition(ref teamSelectedEmployees, accountViewModelCash);
                LimitSelectedEmployeesByPhone(ref teamSelectedEmployees, accountViewModelCash);
                //LimitListByPageNumber(currentUser.Id, ACC_TEAM_SELECTED_EMPLOYEES, ref teamSelectedEmployees, SELECTED_EMPLOYEES_COUNT);
                return teamSelectedEmployees;
            }
            return new List<AccountManager>();
        }

        /// <summary>
        /// Метод ограничивает команду по клиенту по имени менеджера
        /// </summary>
        /// <param name="managersToLimit"></param>
        private void LimitSelectedEmployeesByName(ref List<AccountManager> managersToLimit, AccountViewModel accountViewModelCash)
        {
            if (!string.IsNullOrEmpty(accountViewModelCash.SearchSelectedManagersName))
            {
                Func<AccountManager, bool> predicate = n => n.Manager.GetFullName().ToLower().Contains(accountViewModelCash.SearchSelectedManagersName.ToLower());
                managersToLimit = managersToLimit.Where(predicate).ToList();
            }
        }

        /// <summary>
        /// Метод ограничивает команду по клиенту по должности менеджера
        /// </summary>
        /// <param name="managersToLimit"></param>
        private void LimitSelectedEmployeesByPosition(ref List<AccountManager> managersToLimit, AccountViewModel accountViewModelCash)
        {
            if (!string.IsNullOrEmpty(accountViewModelCash.SearchSelectedManagersPosition))
            {
                TransformCollection(
                    collectionToLimit: ref managersToLimit,
                    limitingCollection: context.GetOrgPositions(accountViewModelCash.OrganizationId),
                    limitCondition: n => n.Name.ToLower().Contains(accountViewModelCash.SearchSelectedManagersPosition),
                    selectCondition: i => i.Id,
                    removeCondition: (managerIdList, accountManager) => accountManager.Manager.PrimaryPositionId == null || !managerIdList.Contains((Guid)accountManager.Manager.PrimaryPositionId));
            }
        }

        /// <summary>
        /// Метод ограничивает команду по клиенту по телефону менеджера
        /// </summary>
        /// <param name="managersToLimit"></param>
        private void LimitSelectedEmployeesByPhone(ref List<AccountManager> managersToLimit, AccountViewModel accountViewModelCash)
        {
            if (!string.IsNullOrEmpty(accountViewModelCash.SearchSelectedManagersPhone))
            {
                List<Employee> employees = managersToLimit.Select(man => man.Manager).ToList();
                List<EmployeeContact> employeeContacts = new List<EmployeeContact>();
                employees.ForEach(emp => employeeContacts.AddRange(context.EmployeeContacts.Where(e => e.EmployeeId == emp.Id && e.ContactType == ContactType.Work)));
                TransformCollection(
                    collectionToLimit: ref managersToLimit,
                    limitingCollection: employeeContacts,
                    limitCondition: n => !string.IsNullOrEmpty(n.PhoneNumber) && n.PhoneNumber.ToLower().Contains(accountViewModelCash.SearchSelectedManagersPhone),
                    selectCondition: i => i.EmployeeId,
                    removeCondition: (employeeIdList, accountManager) => !employeeIdList.Contains(accountManager.ManagerId));
            }
        }
        #endregion

        #region Validations
        private bool TrySyncAccTeamValidate(SyncAccountViewModel syncViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (!new AccountRepository(serviceProvider, context).CheckPermissionForAccountGroup("AccTeamManagement", syncRespsTransaction))
                        AddHasNoPermissionsError(OperationType.AccountTeamManagement);
                },
                () => {
                    if (!accountRepository.TryGetItemById(syncViewModel.AccountId, out Account account))
                        errors.Add("RecordNotFound", resManager.GetString("RecordNotFound"));
                    else syncRespsTransaction.AddParameter("Account", account);
                },
                () => {
                    if (string.IsNullOrEmpty(syncViewModel.PrimaryManagerId) || !Guid.TryParse(syncViewModel.PrimaryManagerId, out Guid primaryManagerId))
                        errors.Add("UnhandledException", resManager.GetString("UnhandledException"));
                    else syncRespsTransaction.AddParameter("PrimaryManagerId", primaryManagerId);
                }
            });
            return !errors.Any();
        }
        #endregion

        #region Other Methods
        /// <summary>
        /// Метод пытается синхронизовать команду по клиенту, и, в случае ошибок, возвращает их
        /// </summary>
        /// <param name="syncViewModel"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public bool TrySyncAccTeam(SyncAccountViewModel syncViewModel, out Dictionary<string, string> errors)
        {
            errors = this.errors;
            syncRespsTransaction = syncRespsTransactionFactory.Create(currentUser.Id, OperationType.AccountTeamManagement, syncViewModel);
            if (TrySyncAccTeamValidate(syncViewModel))
            {
                // Простановка основного менеджера
                Account account = (Account)syncRespsTransaction.GetParameterValue("Account");
                Guid primaryManagerId = (Guid)syncRespsTransaction.GetParameterValue("PrimaryManagerId");
                account.PrimaryManagerId = primaryManagerId;

                // Добавление и удаление сотруднкиов из команды по клиенту
                if (syncViewModel.ManagersToAdd.Count > 0 || syncViewModel.ManagersToRemove.Count > 0)
                {
                    foreach (string managerToAddId in syncViewModel.ManagersToAdd)
                        if (!TryAddManagerToTeam(managerToAddId, account.Id)) break;
                    if (!this.errors.Any())
                    {
                        foreach (string managerToRemoveId in syncViewModel.ManagersToRemove)
                            if (!TryRemoveManagerFromTeam(managerToRemoveId)) break;
                    }
                }

                // Закрытие транзакции, если не было ошибок и коммит прошел успешно
                if (!this.errors.Any() && viewModelsTransactionFactory.TryCommit(syncRespsTransaction, this.errors))
                {
                    viewModelsTransactionFactory.Close(syncRespsTransaction);
                    return true;
                }
            }
            syncRespsTransactionFactory.Close(syncRespsTransaction, TransactionStatus.Error);
            errors = this.errors;
            return false;
        }

        /// <summary>
        /// Метод добавляет сотрудника в команду по клиенту
        /// </summary>
        /// <param name="managerId">Id менеджера для добавления</param>
        /// <param name="accountId">Клиент</param>
        private bool TryAddManagerToTeam(string managerId, Guid accountId)
        {
            if (string.IsNullOrEmpty(managerId) || !Guid.TryParse(managerId, out Guid guid))
            {
                errors.Add("UnhandledException", resManager.GetString("UnhandledException"));
                return false;
            }

            Employee employee = context.Employees.AsNoTracking().FirstOrDefault(i => i.Id == guid);
            if (employee == null)
            {
                errors.Add("EmployeeNotExists", resManager.GetString("EmployeeNotExists"));
                return false;
            }

            switch (employee.EmployeeStatus)
            {
                case EmployeeStatus.Lock:
                    errors.Add("AccountManagerIsLocked", resManager.GetString("AccountManagerIsLocked"));
                    return false;
                case EmployeeStatus.None:
                case EmployeeStatus.AwaitingInvitationAcceptance:
                    errors.Add("AccountManagerIsNonActive", resManager.GetString("AccountManagerIsNonActive"));
                    return false;
            }

            syncRespsTransaction.AddChange(new AccountManager()
            {
                ManagerId = employee.Id,
                AccountId = accountId
            }, EntityState.Added);
            return true;
        }

        /// <summary>
        /// Метод удаляет сотрудника из команды по клиенту
        /// </summary>
        /// <param name="managerId">Id менеджера для удаления</param>
        private bool TryRemoveManagerFromTeam(string managerId)
        {
            if (string.IsNullOrEmpty(managerId) || !Guid.TryParse(managerId, out Guid guid))
            {
                errors.Add("UnhandledException", resManager.GetString("UnhandledException"));
                return false;
            }

            AccountManager accountManager = context.AccountManagers.AsNoTracking().FirstOrDefault(man => man.Id == guid);
            if (accountManager == null)
            {
                errors.Add("AccountManagerNotFound", resManager.GetString("AccountManagerNotFound"));
                return false;
            }
            syncRespsTransaction.AddChange(accountManager, EntityState.Deleted);
            return true;
        }
        #endregion
    }
}
