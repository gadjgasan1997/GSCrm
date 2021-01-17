using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;
using static GSCrm.Utils.CollectionsUtils;
using GSCrm.Data;
using GSCrm.Transactions;

namespace GSCrm.Repository
{
    public class EmployeePositionRepository : BaseRepository<EmployeePosition, EmployeePositionViewModel>
    {
        #region Declarations
        /// <summary>
        /// Ошибки, возникшие при синхронизации списка должностей
        /// </summary>
        private readonly Dictionary<string, string> syncErrors = new Dictionary<string, string>();
        /// <summary>
        /// Должности, которые пользователь выбрал для добавления в список должностей сотрудника
        /// </summary>
        private readonly List<EmployeePosition> positionsToAdd = new List<EmployeePosition>();
        /// <summary>
        /// Должности сотрудника, которые пользователь хочет удалить из его списка должностей
        /// </summary>
        private readonly List<EmployeePosition> positionsToRemove = new List<EmployeePosition>();
        /// <summary>
        /// Транзакция для синхронизации должностей
        /// </summary>
        private ITransaction syncPossTransaction;
        private readonly ITransactionFactory<SyncPositionsViewModel> syncPossTransactionFactory;
        #endregion

        #region Constructors
        public EmployeePositionRepository(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(serviceProvider, context)
        {
            syncPossTransactionFactory = TFFactory.GetTransactionFactory<SyncPositionsViewModel>(serviceProvider, context);
        }
        #endregion

        #region Override Methods
        protected override bool TryDeletePrepare(EmployeePosition employeePosition)
        {
            if (!base.TryDeletePrepare(employeePosition)) return false;
            UpdatePositionPrimaryEmployee(employeePosition);
            return true;
        }
        #endregion

        #region Searching
        /// <summary>
        /// Метод очищает поиск по всем должностям
        /// </summary>
        public void ClearAllPositionSearch()
        {
            EmployeeViewModel employeeViewModelCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, ALL_EMP_POSS);
            employeeViewModelCash.SearchAllPosName = default;
            employeeViewModelCash.SearchAllParentPosName = default;
            cachService.CacheItem(currentUser.Id, ALL_EMP_POSS, employeeViewModelCash);
        }

        /// <summary>
        /// Метод очищает поиск по выбранным должностям
        /// </summary>
        public void ClearSelectedPositionSearch()
        {
            EmployeeViewModel employeeViewModelCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, SELECTED_EMP_POSS);
            employeeViewModelCash.SearchSelectedPosName = default;
            employeeViewModelCash.SearchSelectedParentPosName = default;
            cachService.CacheItem(currentUser.Id, SELECTED_EMP_POSS, employeeViewModelCash);
        }
        #endregion

        #region Attaching All Positions
        /// <summary>
        /// Метод возвращает список всех должностей исходя из подразделения основной должности сотрудника
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public List<Position> AttachAllPositions(Guid employeeId, int pageNumber = DEFAULT_MIN_PAGE_NUMBER)
        {
            SetViewInfo(ALL_EMP_POSS, pageNumber);

            // Получение списка всех должностей подразделения и исключение из него списка должностей сотрудника
            List<Position> allPositions = GetAllPositions(employeeId);
            List<EmployeePosition> employeePositions = context.EmployeePositions.AsNoTracking().Where(empId => empId.EmployeeId == employeeId).ToList();
            List<Position> selectedPositions = employeePositions.GetPositionsFromEmployeePositions(context);
            allPositions = allPositions.Except(selectedPositions, new PositionEqualityComparer()).ToList();
            List<Position> allPositionsExceptSelected = new List<Position>(allPositions);

            // Ограничение списка должностей по фильтрам и номеру страницы
            EmployeeViewModel employeeViewModelCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, ALL_EMP_POSS);
            LimitAllPosByName(employeeViewModelCash, ref allPositions);
            LimitAllPosByParent(employeeViewModelCash, allPositionsExceptSelected, ref allPositions);
            LimitListByPageNumber(ALL_EMP_POSS, ref allPositions);
            return allPositions;
        }

        /// <summary>
        /// Ограничение списка всех должностей подразделения по названию
        /// </summary>
        /// <param name="empViewModelCash"></param>
        /// <param name="positionsToLimit"></param>
        private void LimitAllPosByName(EmployeeViewModel employeeViewModelCash, ref List<Position> positionsToLimit)
        {
            if (!string.IsNullOrEmpty(employeeViewModelCash.SearchAllPosName))
                positionsToLimit = positionsToLimit.Where(n => n.Name.Contains(employeeViewModelCash.SearchAllPosName)).ToList();
        }

        /// <summary>
        /// Ограничение списка всех должностей подразделения по родительскому подразделению
        /// </summary>
        /// <param name="empViewModelCash"></param>
        /// <param name="allPositionsExceptSelected">Список всех должнойстей подразделения, за исключением тех, которые прикреплены к сотруднику</param>
        /// <param name="positionsToLimit"></param>
        private void LimitAllPosByParent(EmployeeViewModel employeeViewModelCash, List<Position> allPositionsExceptSelected, ref List<Position> positionsToLimit)
        {
            if (!string.IsNullOrEmpty(employeeViewModelCash.SearchAllParentPosName))
            {
                TransformCollection(
                    collectionToLimit: ref positionsToLimit,
                    limitingCollection: allPositionsExceptSelected,
                    limitCondition: n => n.Name.ToLower().Contains(employeeViewModelCash.SearchAllParentPosName),
                    selectCondition: i => i.Id,
                    removeCondition: (positionIdList, position) => position.ParentPositionId == null || !positionIdList.Contains((Guid)position.ParentPositionId));
            }
        }
        #endregion

        #region Attaching Selected Positions
        /// <summary>
        /// Метод возвращает список должностей сотрудника для модального окна с ограничением в 5 записей
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public List<EmployeePosition> AttachSelectedPositions(Guid employeeId, int pageNumber = DEFAULT_MIN_PAGE_NUMBER)
        {
            // Ограничение списка должностей по фильтрам и номеру страницы
            SetViewInfo(SELECTED_EMP_POSS, pageNumber);
            EmployeeViewModel employeeViewModelCash = cachService.GetCachedItem<EmployeeViewModel>(currentUser.Id, SELECTED_EMP_POSS);
            List<EmployeePosition> selectedPositions = context.EmployeePositions.AsNoTracking().Where(empId => empId.EmployeeId == employeeId).ToList();
            LimitSelectedPosByName(employeeViewModelCash, ref selectedPositions);
            LimitSelectedPosByParent(employeeViewModelCash, ref selectedPositions);
            LimitListByPageNumber(SELECTED_EMP_POSS, ref selectedPositions);
            return selectedPositions;
        }

        /// <summary>
        /// Ограничение списка всех должностей подразделения по названию
        /// </summary>
        /// <param name="empViewModelCash"></param>
        /// <param name="positionsToLimit"></param>
        private void LimitSelectedPosByName(EmployeeViewModel employeeViewModelCash, ref List<EmployeePosition> positionsToLimit)
        {
            if (!string.IsNullOrEmpty(employeeViewModelCash.SearchSelectedPosName))
            {
                TransformCollection(
                    collectionToLimit: ref positionsToLimit,
                    limitingCollection: employeeViewModelCash.GetDivision(context).GetPositions(context),
                    limitCondition: n => n.Name.ToLower().Contains(employeeViewModelCash.SearchSelectedPosName),
                    selectCondition: i => i.Id,
                    removeCondition: (positionIdList, employeePosition) => employeePosition.PositionId != null && !positionIdList.Contains((Guid)employeePosition.PositionId));
            }
        }

        /// <summary>
        /// Ограничение списка всех должностей подразделения по родительскому подразделению
        /// </summary>
        /// <param name="empViewModelCash"></param>
        /// <param name="positionsToLimit"></param>
        private void LimitSelectedPosByParent(EmployeeViewModel employeeViewModelCash, ref List<EmployeePosition> positionsToLimit)
        {
            if (!string.IsNullOrEmpty(employeeViewModelCash.SearchSelectedParentPosName))
                positionsToLimit = positionsToLimit.LimitByParent(context, employeeViewModelCash, employeeViewModelCash.SearchSelectedParentPosName);
        }
        #endregion

        #region Other Methods
        /// <summary>
        /// Метод вызывается при удалении должности сотрудника для обновления данных об основном сотруднике на этой должности
        /// В случае, если на удаляемой из списка должностей сотрудника должности основным сотрудником является текущий сотрудник,
        /// для этой должности поле PrimaryEmployeeId очищается
        /// </summary>
        /// <param name="employeePosition"></param>
        private void UpdatePositionPrimaryEmployee(EmployeePosition employeePosition)
        {
            Position position = context.Positions.AsNoTracking().FirstOrDefault(i => i.Id == employeePosition.PositionId);
            if (position.PrimaryEmployeeId == employeePosition.EmployeeId)
                position.PrimaryEmployeeId = null;
            context.Positions.Update(position);
        }

        /// <summary>
        /// Метод возвращает список всех должностей исходя из подразделения основной должности сотрудника
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        private List<Position> GetAllPositions(Guid employeeId)
        {
            List<Position> allPositions = new List<Position>();
            Employee employee = context.Employees.FirstOrDefault(i => i.Id == employeeId);
            if (employee.PrimaryPositionId != Guid.Empty)
            {
                Position primaryPosition = employee.GetPrimaryPosition(context);
                allPositions = primaryPosition.GetDivision(context).GetPositions(context);
            }

            else
            {
                Division division = context.Divisions.FirstOrDefault(i => i.Id == employee.DivisionId);
                Organization organization = division.GetOrganization(context);
                organization.GetDivisions(context).ForEach(division => allPositions.AddRange(division.GetPositions(context)));
            }
            return allPositions;
        }

        /// <summary>
        /// Добавляет к сотруднику выбранные позиции и открепляет существующие
        /// </summary>
        /// <param name="syncViewModel"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public bool TrySyncPositions(SyncPositionsViewModel syncViewModel, ref Dictionary<string, string> errors)
        {
            syncPossTransaction = syncPossTransactionFactory.Create(currentUser.Id, OperationType.EmployeePositionsManagement, syncViewModel);
            Employee employee = context.Employees
                .AsNoTracking()
                .Include(empPos => empPos.EmployeePositions)
                    .ThenInclude(pos => pos.Position)
                .FirstOrDefault(i => i.Id == syncViewModel.EmployeeId);
            syncPossTransaction.AddParameter("Employee", employee);

            // Проверки
            InvokeIntermittinActions(this.errors, new List<Action>()
            {
                () => {
                    if (!new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("EmpPossManagement", syncPossTransaction))
                         AddHasNoPermissionsError(OperationType.EmployeePositionsManagement);
                },
                () => {
                    FormAddPositinosList(syncViewModel.PositionsToAdd, employee);
                },
                () => {
                    FormRemovePositionsList(syncViewModel, employee);
                }
            });

            // Если не было ошибок, выполняется обновление списка должностей
            if (!this.errors.Any())
            {
                AddPositions();
                RemovePositions();
                if (!string.IsNullOrEmpty(syncViewModel.PrimaryPositionName))
                    SetPrimaryPosition(syncViewModel.PrimaryPositionName, employee);

                // Попытка сделать коммит
                if (syncPossTransactionFactory.TryCommit(syncPossTransaction, this.errors))
                {
                    syncPossTransactionFactory.Close(syncPossTransaction);
                    return true;
                }
            }

            // Добавление ошибок и выход
            errors = this.errors;
            syncPossTransactionFactory.Close(syncPossTransaction, TransactionStatus.Error);
            return false;
        }

        /// <summary>
        /// Формирует список должностей для добавления
        /// </summary>
        /// <param name="positionsToAdd"></param>
        /// <param name="employee"></param>
        private void FormAddPositinosList(List<string> positionsToAdd, Employee employee)
        {
            List<Position> allPositions = employee.GetDivision(context).GetPositions(context);
            positionsToAdd.ForEach(positionName =>
            {
                Position position = allPositions.FirstOrDefault(n => n.Name == positionName);

                // Если должность найдена
                if (position != null)
                {
                    if (employee.EmployeePositions.Select(pos => pos.Position.Name).Contains(positionName))
                    {
                        syncErrors.Add("PosIsAlreadyAdded", resManager.GetString("PosIsAlreadyAdded"));
                        return;
                    }

                    // Если она уже не была добавлена в список должностей
                    if (!this.positionsToAdd.Select(pos => pos.Position.Name).Contains(positionName))
                    {
                        this.positionsToAdd.Add(new EmployeePosition()
                        {
                            Employee = employee,
                            EmployeeId = employee.Id,
                            Position = position,
                            PositionId = position.Id
                        });
                    }
                }
                else syncErrors.Add("AddPosNotExists", resManager.GetString("AddPosNotExists"));
            });
        }

        /// <summary>
        /// Формирует список должностей для удаления
        /// </summary>
        /// <param name="positionsToRemove"></param>
        /// <param name="employee"></param>
        private void FormRemovePositionsList(SyncPositionsViewModel syncViewModel, Employee employee)
        {
            syncViewModel.PositionsToRemove.ForEach(positionName =>
            {
                if (positionName == syncViewModel.PrimaryPositionName)
                    syncErrors.Add("PrimaryPositionIsReadonly", resManager.GetString("PrimaryPositionIsReadonly"));
                else
                {
                    EmployeePosition employeePosition = employee.EmployeePositions.FirstOrDefault(n => n.Position.Name == positionName);

                    // Если должность найдена
                    if (employeePosition != null)
                    {
                        // Если она уже не была добавлена в список должностей на удаление
                        if (!positionsToRemove.Select(resp => resp.Position.Name).Contains(positionName))
                            positionsToRemove.Add(employeePosition);
                    }
                    else syncErrors.Add("RemovePosNotExists", resManager.GetString("RemovePosNotExists"));
                }
            });
        }

        /// <summary>
        /// Добавляет выбранные должности к должностям сотрудника
        /// </summary>
        private void AddPositions()
        {
            positionsToAdd.ForEach(postionToAdd =>
            {
                syncPossTransaction.AddChange(postionToAdd, EntityState.Added);
            });
        }

        /// <summary>
        /// Удаляет выбранные должности из списка должностей сотрудника
        /// Дополнительно вызывает метод обновления основного сотрудника на должности
        /// </summary>
        private void RemovePositions()
        {
            positionsToRemove.ForEach(postionToRemove =>
            {
                UpdatePositionPrimaryEmployee(postionToRemove);
                syncPossTransaction.AddChange(postionToRemove, EntityState.Deleted);
            });
        }

        /// <summary>
        /// Устанавливает основную должность для сотрудника
        /// </summary>
        /// <param name="primaryPositionName"></param>
        private void SetPrimaryPosition(string primaryPositionName, Employee employee)
        {
            EmployeePosition selectedPrimaryPosition = employee.EmployeePositions.FirstOrDefault(n => n.Position.Name == primaryPositionName);
            EmployeePosition currentPrimaryPosition = employee.EmployeePositions.FirstOrDefault(i => i.PositionId == employee.PrimaryPositionId);
            employee.PrimaryPositionId = selectedPrimaryPosition.PositionId;
            syncPossTransaction.AddChange(employee, EntityState.Modified);
        }
        #endregion
    }
}
