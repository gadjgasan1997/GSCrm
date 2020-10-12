using GSCrm.Data;
using GSCrm.Data.ApplicationInfo;
using GSCrm.DataTransformers;
using GSCrm.Helpers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Repository
{
    public class EmployeePositionRepository : GenericRepository<EmployeePosition, EmployeePositionViewModel, EmployeePositionValidator, EmployeePositionTransformer>
    {
        private readonly User currentUser;
        private readonly Dictionary<string, string> syncErrors;
        private readonly List<EmployeePosition> positionsToAdd;
        private readonly List<EmployeePosition> positionsToRemove;
        private const int SYNC_POS_ITEMS_COUNT = 5;

        public EmployeePositionRepository(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager, HttpContext httpContext = null)
            : base(context, viewsInfo, resManager, new EmployeePositionValidator(context, resManager), new EmployeePositionTransformer(context, resManager))
        {
            syncErrors = new Dictionary<string, string>();
            positionsToAdd = new List<EmployeePosition>();
            positionsToRemove = new List<EmployeePosition>();
            if (httpContext != null)
                currentUser = httpContext.GetCurrentUser(context);
        }

        #region Override Methods
        public override bool TryDeletePrepare(Guid id, EmployeePosition employeePosition, ModelStateDictionary modelState)
        {
            if (!base.TryDeletePrepare(id, employeePosition, modelState)) return false;
            UpdatePositionPrimaryEmployee(employeePosition);
            return true;
        }
        #endregion

        /// <summary>
        /// Метод устанавливает значения для поиска по всем должностям
        /// </summary>
        /// <param name="empPosViewModelCash"></param>
        /// <returns></returns>
        public void SearchAllPosition(EmployeeViewModel employeeViewModel)
        {
            //viewsInfo.Reset(ALL_EMP_POSS);
            EmployeeViewModel employeeViewModelCash = ModelCash<EmployeeViewModel>.GetViewModel(currentUser.Id, ALL_EMP_POSS);
            employeeViewModelCash.IdCash.AddOrReplace(currentUser.Id, employeeViewModel.Id);
            employeeViewModelCash.DivisionIdCash.AddOrReplace(currentUser.Id, employeeViewModel.DivisionId);
            employeeViewModelCash.OrganizationIdCash.AddOrReplace(currentUser.Id, employeeViewModel.OrganizationId);
            employeeViewModelCash.SearchAllPosNameCash.AddOrReplace(currentUser.Id, employeeViewModel.SearchAllPosName?.ToLower().TrimStartAndEnd());
            employeeViewModelCash.SearchAllParentPosNameCash.AddOrReplace(currentUser.Id, employeeViewModel.SearchAllParentPosName?.ToLower().TrimStartAndEnd());
            ModelCash<EmployeeViewModel>.SetViewModel(currentUser.Id, ALL_EMP_POSS, employeeViewModelCash);
        }

        /// <summary>
        /// Метод очищает поиск по всем должностям
        /// </summary>
        public void ClearAllPositionSearch()
        {
            EmployeeViewModel employeeViewModelCash = ModelCash<EmployeeViewModel>.GetViewModel(currentUser.Id, ALL_EMP_POSS);
            employeeViewModelCash.SearchAllPosNameCash.AddOrReplace(currentUser.Id, default);
            employeeViewModelCash.SearchAllParentPosNameCash.AddOrReplace(currentUser.Id, default);
            ModelCash<EmployeeViewModel>.SetViewModel(currentUser.Id, ALL_EMP_POSS, employeeViewModelCash);
        }

        /// <summary>
        /// Метод устанавливает значения для поиска по выбранным должностям
        /// </summary>
        /// <param name="empPosViewModelCash"></param>
        /// <returns></returns>
        public void SearchSelectedPosition(EmployeeViewModel employeeViewModel)
        {
            //viewsInfo.Reset(SELECTED_EMP_POSS);
            EmployeeViewModel employeeViewModelCash = ModelCash<EmployeeViewModel>.GetViewModel(currentUser.Id, SELECTED_EMP_POSS);
            employeeViewModelCash.IdCash.AddOrReplace(currentUser.Id, employeeViewModel.Id);
            employeeViewModelCash.DivisionIdCash.AddOrReplace(currentUser.Id, employeeViewModel.DivisionId);
            employeeViewModelCash.OrganizationIdCash.AddOrReplace(currentUser.Id, employeeViewModel.OrganizationId);
            employeeViewModelCash.SearchSelectedPosNameCash.AddOrReplace(currentUser.Id, employeeViewModel.SearchSelectedPosName?.ToLower().TrimStartAndEnd());
            employeeViewModelCash.SearchSelectedParentPosNameCash.AddOrReplace(currentUser.Id, employeeViewModel.SearchSelectedParentPosName?.ToLower().TrimStartAndEnd());
            ModelCash<EmployeeViewModel>.SetViewModel(currentUser.Id, SELECTED_EMP_POSS, employeeViewModelCash);
        }

        /// <summary>
        /// Метод очищает поиск по выбранным должностям
        /// </summary>
        public void ClearSelectedPositionSearch()
        {
            EmployeeViewModel employeeViewModelCash = ModelCash<EmployeeViewModel>.GetViewModel(currentUser.Id, SELECTED_EMP_POSS);
            employeeViewModelCash.SearchSelectedPosNameCash.AddOrReplace(currentUser.Id, default);
            employeeViewModelCash.SearchSelectedParentPosNameCash.AddOrReplace(currentUser.Id, default);
            ModelCash<EmployeeViewModel>.SetViewModel(currentUser.Id, SELECTED_EMP_POSS, employeeViewModelCash);
        }

        /// <summary>
        /// Метод возвращает список всех должностей исходя из подразделения основной должности сотрудника
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public List<Position> GetAllPositions(Guid employeeId, int pageNumber = DEFAULT_MIN_PAGE_NUMBER)
        {
            SetViewInfo(currentUser.Id, ALL_EMP_POSS, pageNumber, SYNC_POS_ITEMS_COUNT);

            // Получение списка всех должностей подразделения и его исключение из него списка должностей сотрудника
            List<Position> allPositions = GetAllPositions(employeeId);
            List<Position> selectedPositions = GetEmployeePositions(employeeId).GetPositionsFromEmployeePositions(context);
            allPositions = allPositions.Except(selectedPositions, new PositionEqualityComparer()).ToList();
            List<Position> allPositionsExceptSelected = new List<Position>(allPositions);

            // Ограничение списка должностей по фильтрам и номеру страницы
            EmployeeViewModel employeeViewModelCash = ModelCash<EmployeeViewModel>.GetViewModel(currentUser.Id, ALL_EMP_POSS);
            LimitAllPosByName(employeeViewModelCash, ref allPositions);
            LimitAllPosByParent(employeeViewModelCash, allPositionsExceptSelected, ref allPositions);
            LimitListByPageNumber(currentUser.Id, ALL_EMP_POSS, ref allPositions, SYNC_POS_ITEMS_COUNT);
            return allPositions;
        }

        /// <summary>
        /// Ограничение списка всех должностей подразделения по названию
        /// </summary>
        /// <param name="empViewModelCash"></param>
        /// <param name="positionsToLimit"></param>
        private void LimitAllPosByName(EmployeeViewModel employeeViewModelCash, ref List<Position> positionsToLimit)
        {
            string searchAllPosName = employeeViewModelCash.SearchAllPosNameCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchAllPosName))
                positionsToLimit = positionsToLimit.Where(n => n.Name.Contains(searchAllPosName)).ToList();
        }

        /// <summary>
        /// Ограничение списка всех должностей подразделения по родительскому подразделению
        /// </summary>
        /// <param name="empViewModelCash"></param>
        /// <param name="allPositionsExceptSelected">Список всех должнойстей подразделения, за исключением тех, которые прикреплены к сотруднику</param>
        /// <param name="positionsToLimit"></param>
        private void LimitAllPosByParent(EmployeeViewModel employeeViewModelCash, List<Position> allPositionsExceptSelected, ref List<Position> positionsToLimit)
        {
            string searchAllParentPosName = employeeViewModelCash.SearchAllParentPosNameCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchAllParentPosName))
            {
                TransformCollection(
                    collectionToLimit: ref positionsToLimit,
                    limitingCollection: allPositionsExceptSelected,
                    limitCondition: n => n.Name.ToLower().Contains(searchAllParentPosName),
                    selectCondition: i => i.Id,
                    removeCondition: (positionIdList, position) => position.ParentPositionId == null || !positionIdList.Contains((Guid)position.ParentPositionId));
            }
        }

        /// <summary>
        /// Метод возвращает список должностей сотрудника для модального окна с ограничением в 5 записей
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public List<EmployeePosition> GetSelectedPositions(Guid employeeId, int pageNumber = DEFAULT_MIN_PAGE_NUMBER)
        {
            SetViewInfo(currentUser.Id, SELECTED_EMP_POSS, pageNumber, SYNC_POS_ITEMS_COUNT);

            // Ограничение списка должностей по фильтрам и номеру страницы
            EmployeeViewModel employeeViewModelCash = ModelCash<EmployeeViewModel>.GetViewModel(currentUser.Id, SELECTED_EMP_POSS);
            List<EmployeePosition> selectedPositions = GetEmployeePositions(employeeId);
            LimitSelectedPosByName(employeeViewModelCash, ref selectedPositions);
            LimitSelectedPosByParent(employeeViewModelCash, ref selectedPositions);
            LimitListByPageNumber(currentUser.Id, SELECTED_EMP_POSS, ref selectedPositions, SYNC_POS_ITEMS_COUNT);
            return selectedPositions;
        }

        /// <summary>
        /// Ограничение списка всех должностей подразделения по названию
        /// </summary>
        /// <param name="empViewModelCash"></param>
        /// <param name="positionsToLimit"></param>
        private void LimitSelectedPosByName(EmployeeViewModel employeeViewModelCash, ref List<EmployeePosition> positionsToLimit)
        {
            string searchSelectedPosName = employeeViewModelCash.SearchSelectedPosNameCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchSelectedPosName))
            {
                employeeViewModelCash.DivisionId = employeeViewModelCash.DivisionIdCash.GetValueOrDefault(currentUser.Id);
                TransformCollection(
                    collectionToLimit: ref positionsToLimit,
                    limitingCollection: employeeViewModelCash.GetDivision(context).Positions,
                    limitCondition: n => n.Name.ToLower().Contains(searchSelectedPosName),
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
            string searchSelectedParentPosName = employeeViewModelCash.SearchSelectedParentPosNameCash.GetValueOrDefault(currentUser.Id);
            positionsToLimit = positionsToLimit.LimitByParent(context, employeeViewModelCash, searchSelectedParentPosName, currentUser.Id);
        }

        /// <summary>
        /// Метод возвращает список должностей сотрудника для таблицы с ограничением в 10 записей
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public List<EmployeePosition> GetSelectedPositionsForTable(Guid employeeId, int pageNumber = DEFAULT_MIN_PAGE_NUMBER)
        {
            List<EmployeePosition> selectedPositions = GetEmployeePositions(employeeId);
            LimitListByPageNumber(currentUser.Id, SELECTED_EMP_POSS, ref selectedPositions, pageNumber);
            return selectedPositions;
        }

        /// <summary>
        /// Метод вызывается при удалении должности сотрудника для обновления данных об основном сотруднике на этой должности
        /// В случае, если на удаляемой из списка должностей сотрудника должности основным сотрудником является текущий сотрудник,
        /// для этой должности поле PrimaryEmployeeId очищается
        /// </summary>
        /// <param name="employeePosition"></param>
        private void UpdatePositionPrimaryEmployee(EmployeePosition employeePosition)
        {
            Position position = context.Positions.FirstOrDefault(i => i.Id == employeePosition.PositionId);
            if (position.PrimaryEmployeeId == employeePosition.EmployeeId)
                position.PrimaryEmployeeId = null;
            context.Positions.Update(position);
        }

        /// <summary>
        /// Метод возвращает список должностей сотрудника
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        private List<EmployeePosition> GetEmployeePositions(Guid employeeId)
        {
            Employee employee = context.Employees.FirstOrDefault(i => i.Id == employeeId);
            List<EmployeePosition> employeePositions = context.EmployeePositions.Where(empId => empId.EmployeeId == employeeId).ToList();
            return employeePositions;
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
                allPositions = primaryPosition.Division.Positions;
            }

            else
            {
                Division division = context.Divisions.FirstOrDefault(i => i.Id == employee.DivisionId);
                Organization organization = division.GetOrganization(context);
                organization.Divisions.ForEach(division => allPositions.AddRange(division.Positions));
            }
            return allPositions;
        }

        /// <summary>
        /// Добавляет к сотруднику выбранные позиции и открепляет существующие
        /// </summary>
        /// <param name="syncViewModel"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public bool TrySyncPositions(SyncPositionsViewModel syncViewModel, Dictionary<string, string> errors)
        {
            Employee employee = context.Employees
                .Include(empPos => empPos.EmployeePositions)
                    .ThenInclude(pos => pos.Position)
                .FirstOrDefault(i => i.Id == syncViewModel.EmployeeId);

            FormAddPositinosList(syncViewModel.PositionsToAdd, employee);
            FormRemovePositionsList(syncViewModel, employee);
            if (syncErrors.Any())
            {
                errors = syncErrors;
                return false;
            }

            AddPositions();
            RemovePositions();
            if (!string.IsNullOrEmpty(syncViewModel.PrimaryPositionName))
                SetPrimaryPosition(syncViewModel.PrimaryPositionName, employee);
            context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Формирует список должностей для добавления
        /// </summary>
        /// <param name="positionsToAdd"></param>
        /// <param name="employee"></param>
        private void FormAddPositinosList(List<string> positionsToAdd, Employee employee)
        {
            Division employeeDivision = employee.GetDivision(context);
            List<Position> allPositions = employeeDivision.Positions;

            positionsToAdd.ForEach(positionName =>
            {
                Position position = employeeDivision.Positions.FirstOrDefault(n => n.Name == positionName);
                if (position != null)
                {
                    this.positionsToAdd.Add(new EmployeePosition()
                    {
                        Employee = employee,
                        EmployeeId = employee.Id,
                        Position = position,
                        PositionId = position.Id
                    });
                }
            });
        }

        /// <summary>
        /// Формирует список должностей для удаления
        /// </summary>
        /// <param name="positionsToRemove"></param>
        /// <param name="employee"></param>
        private void FormRemovePositionsList(SyncPositionsViewModel syncViewModel, Employee employee)
        {
            List<EmployeePosition> selectedPositions = employee.EmployeePositions;
            syncViewModel.PositionsToRemove.ForEach(positionName =>
            {
                if (positionName == syncViewModel.PrimaryPositionName)
                    syncErrors.Add("PrimaryPositionIsReadonly", resManager.GetString("PrimaryPositionIsReadonly"));
                else
                {
                    EmployeePosition employeePosition = employee.EmployeePositions.FirstOrDefault(n => n.Position.Name == positionName);
                    if (employeePosition != null)
                        positionsToRemove.Add(employeePosition);
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
                context.EmployeePositions.Add(postionToAdd);
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
                context.EmployeePositions.Remove(postionToRemove);
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
            if (selectedPrimaryPosition.Id != currentPrimaryPosition.Id)
            {
                employee.PrimaryPositionId = selectedPrimaryPosition.PositionId;
                context.Employees.Update(employee);
            }
        }
    }
}
