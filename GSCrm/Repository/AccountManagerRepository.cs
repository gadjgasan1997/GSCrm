using GSCrm.Data;
using GSCrm.Data.ApplicationInfo;
using GSCrm.DataTransformers;
using GSCrm.Helpers;
using GSCrm.Localization;
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
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Repository
{
    public class AccountManagerRepository : GenericRepository<AccountManager, AccountManagerViewModel, AccountManagerValidatior, AccountManagerTransformer>
    {
        private readonly User currentUser;
        private readonly AccountRepository accountRepository;
        private const int TEAM_ALL_EMPLOYEES_COUNT = 5;
        private const int TEAM_SELECTED_EMPLOYEES_COUNT = 5;
        public AccountManagerRepository(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager, HttpContext httpContext = null)
            : base(context, viewsInfo, resManager, new AccountManagerValidatior(context, resManager), new AccountManagerTransformer(context, resManager))
        {
            accountRepository = new AccountRepository(context, resManager);
            if (httpContext != null)
                currentUser = httpContext.GetCurrentUser(context);
        }

        /// <summary>
        /// Метод устанавливает фильтрацию для списка всех сотрудников организации, создавшей клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        public void SearchAllManagers(AccountViewModel accountViewModel)
        {
            //viewsInfo.Reset(ACC_TEAM_ALL_EMPLOYEES);
            AccountViewModel accountViewModelCash = ModelCash<AccountViewModel>.GetViewModel(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES);
            accountViewModelCash.Id = accountViewModel.Id;
            accountViewModelCash.OrganizationId = accountViewModel.OrganizationId;
            accountViewModelCash.SearchAllManagersName = accountViewModel.SearchAllManagersName?.ToLower().TrimStartAndEnd();
            accountViewModelCash.SearchAllManagersDivision = accountViewModel.SearchAllManagersDivision?.ToLower().TrimStartAndEnd();
            accountViewModelCash.SearchAllManagersPosition = accountViewModel.SearchAllManagersPosition?.ToLower().TrimStartAndEnd();
            ModelCash<AccountViewModel>.SetViewModel(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES, accountViewModelCash);
        }

        /// <summary>
        /// Метод сбрасывает фильтрацию для списка всех сотрудников организации, создавшей клиента
        /// </summary>
        public void ClearAllManagersSearch()
        {
            AccountViewModel accountViewModelCash = ModelCash<AccountViewModel>.GetViewModel(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES);
            accountViewModelCash.SearchAllManagersName = default;
            accountViewModelCash.SearchAllManagersDivision = default;
            accountViewModelCash.SearchAllManagersPosition = default;
            ModelCash<AccountViewModel>.SetViewModel(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES, accountViewModelCash);
        }

        /// <summary>
        /// Метод устанавливает фильтрацию для команды по клиенту
        /// </summary>
        /// <param name="accountViewModel"></param>
        public void SearchSelectedManagers(AccountViewModel accountViewModel)
        {
            //viewsInfo.Reset(ACC_TEAM_SELECTED_EMPLOYEES);
            AccountViewModel accountViewModelCash = ModelCash<AccountViewModel>.GetViewModel(currentUser.Id, ACC_TEAM_SELECTED_EMPLOYEES);
            accountViewModelCash.Id = accountViewModel.Id;
            accountViewModelCash.OrganizationId = accountViewModel.OrganizationId;
            accountViewModelCash.SearchSelectedManagersName = accountViewModel.SearchSelectedManagersName?.ToLower().TrimStartAndEnd();
            accountViewModelCash.SearchSelectedManagersPhone = accountViewModel.SearchSelectedManagersPhone?.ToLower().TrimStartAndEnd();
            accountViewModelCash.SearchSelectedManagersPosition = accountViewModel.SearchSelectedManagersPosition?.ToLower().TrimStartAndEnd();
            ModelCash<AccountViewModel>.SetViewModel(currentUser.Id, ACC_TEAM_SELECTED_EMPLOYEES, accountViewModelCash);
        }

        /// <summary>
        /// Метод сбрасывает фильтрацию для команды по клиенту
        /// </summary>
        public void ClearSelectedManagersSearch()
        {
            AccountViewModel accountViewModelCash = ModelCash<AccountViewModel>.GetViewModel(currentUser.Id, ACC_TEAM_SELECTED_EMPLOYEES);
            accountViewModelCash.SearchSelectedManagersName = default;
            accountViewModelCash.SearchSelectedManagersPhone = default;
            accountViewModelCash.SearchSelectedManagersPosition = default;
            ModelCash<AccountViewModel>.SetViewModel(currentUser.Id, ACC_TEAM_SELECTED_EMPLOYEES, accountViewModelCash);
        }

        /// <summary>
        /// Метод возвращает список всех сотрудников организации для отображения в окне управления командой по клиенту
        /// </summary>
        /// <returns></returns>
        public List<Employee> GetTeamAllEmployees(string accountId, int pageNumber = DEFAULT_MIN_PAGE_NUMBER)
        {
            if (accountRepository.TryGetItemById(accountId, out Account account))
            {
                SetViewInfo(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES, pageNumber, TEAM_ALL_EMPLOYEES_COUNT);

                List<Employee> teamAllEmployees = context.GetOrgEmployees(account.OrganizationId);
                AccountViewModel accountViewModelCash = ModelCash<AccountViewModel>.GetViewModel(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES);
                ExcludeSelectedEmployees(ref teamAllEmployees, account.Id);
                LimitAllEmployeesByName(ref teamAllEmployees, accountViewModelCash);
                LimitAllEmployeesByDivision(ref teamAllEmployees, accountViewModelCash);
                LimitAllEmployeesByPosition(ref teamAllEmployees, accountViewModelCash);
                LimitListByPageNumber(currentUser.Id, ACC_TEAM_ALL_EMPLOYEES, ref teamAllEmployees, TEAM_ALL_EMPLOYEES_COUNT);
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
                    removeCondition: (divisionIdList, employee) => !divisionIdList.Contains(employee.DivisionId));
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

        /// <summary>
        /// Метод возвращает список менеджеров клиента для отображения в окне управления командой
        /// </summary>
        /// <returns></returns>
        public List<AccountManager> GetTeamSelectedEmployees(string accountId, int pageNumber = DEFAULT_MIN_PAGE_NUMBER)
        {
            if (accountRepository.TryGetItemById(accountId, out Account account))
            {
                SetViewInfo(currentUser.Id, ACC_TEAM_SELECTED_EMPLOYEES, pageNumber, TEAM_SELECTED_EMPLOYEES_COUNT);
                AccountViewModel accountViewModelCash = ModelCash<AccountViewModel>.GetViewModel(currentUser.Id, ACC_TEAM_SELECTED_EMPLOYEES);
                List<AccountManager> teamSelectedEmployees = context.AccountManagers
                    .Include(man => man.Manager)
                    .Where(accId => accId.AccountId == account.Id).ToList();
                LimitSelectedEmployeesByName(ref teamSelectedEmployees, accountViewModelCash);
                LimitSelectedEmployeesByPosition(ref teamSelectedEmployees, accountViewModelCash);
                LimitSelectedEmployeesByPhone(ref teamSelectedEmployees, accountViewModelCash);
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

        /// <summary>
        /// Метод пытается синхронизовать команду по клиенту, и, в случае ошибок, возвращает их
        /// </summary>
        /// <param name="syncViewModel"></param>
        /// <param name="syncErrors"></param>
        /// <returns></returns>
        public bool TrySyncPositions(SyncAccountViewModel syncViewModel, Dictionary<string, string> syncErrors)
        {
            if (!accountRepository.TryGetItemById(syncViewModel.AccountId, out Account account))
            {
                syncErrors.Add("SyncManagersException", resManager.GetString("SyncManagersException"));
                return false;
            }

            if (string.IsNullOrEmpty(syncViewModel.PrimaryManagerId) || !Guid.TryParse(syncViewModel.PrimaryManagerId, out Guid primaryManagerId))
            {
                syncErrors.Add("SyncManagersException", resManager.GetString("SyncManagersException"));
                return false;
            }

            SetUpPrimaryManager(primaryManagerId, account);

            if (syncViewModel.ManagersToAdd.Count == 0 && syncViewModel.ManagersToRemove.Count == 0)
            {
                context.SaveChanges();
                return true;
            }

            foreach(string managerToAddId in syncViewModel.ManagersToAdd)
            {
                if (!TryAddManagerToTeam(account, managerToAddId, ref syncErrors))
                    return false;
            }

            foreach (string managerToRemoveId in syncViewModel.ManagersToRemove)
            {
                if (!TryRemoveManagerFromTeam(account, managerToRemoveId, ref syncErrors))
                    return false;
            }

            context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Метод добавляет сотрудника в команду по клиенту
        /// </summary>
        /// <param name="managerId">Id менеджера для добавления</param>
        /// <param name="account">Клиент</param>
        private bool TryAddManagerToTeam(Account account, string managerId, ref Dictionary<string, string> errors)
        {
            if (string.IsNullOrEmpty(managerId) || !Guid.TryParse(managerId, out Guid guid))
            {
                errors.Add("SyncManagersException", resManager.GetString("SyncManagersException"));
                return false;
            }

            Employee employee = context.Employees.FirstOrDefault(i => i.Id == guid);
            if (employee == null)
            {
                errors.Add("SyncManagersException", resManager.GetString("SyncManagersException"));
                return false;
            }

            if (employee.EmployeeStatus == EmployeeStatus.Lock)
            {
                errors.Add("AccountManagerIsLocked", resManager.GetString("AccountManagerIsLocked"));
                return false;
            }

            context.AccountManagers.Add(new AccountManager()
            {
                ManagerId = employee.Id,
                AccountId = account.Id
            });
            return true;
        }

        /// <summary>
        /// Метод удаляет сотрудника из команды по клиенту
        /// </summary>
        /// <param name="managerId">Id менеджера для удаления</param>
        /// <param name="account">Клиент</param>
        private bool TryRemoveManagerFromTeam(Account account, string managerId, ref Dictionary<string, string> errors)
        {
            if (string.IsNullOrEmpty(managerId) || !Guid.TryParse(managerId, out Guid guid))
            {
                errors.Add("SyncManagersException", resManager.GetString("SyncManagersException"));
                return false;
            }

            AccountManager accountManager = context.AccountManagers.FirstOrDefault(man => man.Id == guid);
            if (accountManager == null)
            {
                errors.Add("SyncManagersException", resManager.GetString("SyncManagersException"));
                return false;
            }
            context.AccountManagers.Remove(accountManager);
            return true;
        }

        /// <summary>
        /// Метод устанавливает менеджера основным в команде по клиенту
        /// </summary>
        /// <param name="primaryManagerId">Id основного выбранного менеджера</param>
        /// <param name="account">Клиент</param>
        private void SetUpPrimaryManager(Guid primaryManagerId, Account account)
        {
            account.PrimaryManagerId = primaryManagerId;
            context.Accounts.Update(account);
        }
    }
}
