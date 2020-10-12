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
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Repository
{
    public class PositionRepository : GenericRepository<Position, PositionViewModel, PositionValidator, PositionTransformer>
    {
        private readonly User currentUser;
        public static PositionViewModel CurrentPosition { get; set; }
        public PositionRepository(ApplicationDbContext context) : base(context) { }

        public PositionRepository(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager, HttpContext httpContext = null)
            : base(context, viewsInfo, resManager, new PositionValidator(context, resManager), new PositionTransformer(context, resManager, httpContext))
        {
            if (httpContext != null)
                currentUser = httpContext.GetCurrentUser(context);
        }

        public PositionRepository(ApplicationDbContext context, ResManager resManager) : base(context, resManager)
        { }

        #region Override Methods
        public override void FailureUpdateHandler(PositionViewModel positionViewModel, Action<PositionViewModel> handler = null)
        {
            if (TryGetItemById(positionViewModel.Id, out Position position))
            {
                positionViewModel = transformer.DataToViewModel(position);
                positionViewModel = transformer.UpdateViewModelFromCash(positionViewModel);
                AttachEmployees(positionViewModel);
                AttachSubPositions(positionViewModel);
            }
        }

        public override bool TryDeletePrepare(Guid id, Position position, ModelStateDictionary modelState)
        {
            if (!base.TryDeletePrepare(id, position, modelState)) return false;
            position = context.Positions
                .Include(empPos => empPos.EmployeePositions)
                    .ThenInclude(emp => emp.Employee)
                .FirstOrDefault(i => i.Id == position.Id);
            position.EmployeePositions.ForEach(employeePosition => CheckEmployeeForLock(employeePosition, position));
            return true;
        }
        #endregion

        #region Searching
        /// <summary>
        /// Метод устанавливает значения для поиска по сотрудникам
        /// </summary>
        /// <param name="empViewModelCash"></param>
        /// <returns></returns>
        public void SearchEmployee(PositionViewModel positionViewModel)
        {
            //viewsInfo.Reset(POS_EMPLOYEES);
            PositionViewModel posViewModelModelCash = ModelCash<PositionViewModel>.GetViewModel(currentUser.Id, POS_EMPLOYEES);
            posViewModelModelCash.SearchEmployeeInitialNameCash.AddOrReplace(currentUser.Id, positionViewModel.SearchEmployeeInitialName?.ToLower().TrimStartAndEnd());
            ModelCash<PositionViewModel>.SetViewModel(currentUser.Id, POS_EMPLOYEES, posViewModelModelCash);
        }

        /// <summary>
        /// Метод очищает поиск по сотрудникам
        /// </summary>
        public void ClearSearchEmployee()
        {
            PositionViewModel posViewModelModelCash = ModelCash<PositionViewModel>.GetViewModel(currentUser.Id, POS_EMPLOYEES);
            posViewModelModelCash.SearchEmployeeInitialNameCash.AddOrReplace(currentUser.Id, default);
            ModelCash<PositionViewModel>.SetViewModel(currentUser.Id, POS_EMPLOYEES, posViewModelModelCash);
        }

        /// <summary>
        /// Метод устанавливает значения для поиска по дочерним должностям
        /// </summary>
        /// <param name="empViewModelCash"></param>
        /// <returns></returns>
        public void SearchSubPosition(PositionViewModel positionViewModel)
        {
            //viewsInfo.Reset(POS_SUB_POSS);
            PositionViewModel posViewModelModelCash = ModelCash<PositionViewModel>.GetViewModel(currentUser.Id, POS_SUB_POSS);
            posViewModelModelCash.DivisionIdCash.AddOrReplace(currentUser.Id, positionViewModel.DivisionId);
            posViewModelModelCash.SearchSubPositionNameCash.AddOrReplace(currentUser.Id, positionViewModel.SearchSubPositionName?.ToLower().TrimStartAndEnd());
            posViewModelModelCash.SearchSubPositionPrimaryEmployeeCash.AddOrReplace(currentUser.Id, positionViewModel.SearchSubPositionPrimaryEmployee?.ToLower().TrimStartAndEnd());
            ModelCash<PositionViewModel>.SetViewModel(currentUser.Id, POS_SUB_POSS, posViewModelModelCash);
        }

        /// <summary>
        /// Метод очищает поиск по дочерним должностям
        /// </summary>
        public void ClearSearchSubPosition()
        {
            PositionViewModel posViewModelModelCash = ModelCash<PositionViewModel>.GetViewModel(currentUser.Id, POS_SUB_POSS);
            posViewModelModelCash.DivisionIdCash.AddOrReplace(currentUser.Id, default);
            posViewModelModelCash.SearchSubPositionNameCash.AddOrReplace(currentUser.Id, default);
            posViewModelModelCash.SearchSubPositionPrimaryEmployeeCash.AddOrReplace(currentUser.Id, default);
            ModelCash<PositionViewModel>.SetViewModel(currentUser.Id, POS_SUB_POSS, posViewModelModelCash);
        }
        #endregion

        #region Attaching Employees
        /// <summary>
        /// Добавляет список работников к модели представления должности
        /// </summary>
        /// <param name="positionViewModel"></param>
        public void AttachEmployees(PositionViewModel positionViewModel)
        {
            positionViewModel.PositionEmployees = positionViewModel.GetEmployees(context)
                .TransformToViewModels<Employee, EmployeeViewModel, EmployeeTransformer>(
                    transformer: new EmployeeTransformer(context, resManager),
                    limitingFunc: GetLimitedEmployeesList);
        }

        private List<Employee> GetLimitedEmployeesList(List<Employee> employees)
        {
            PositionViewModel posViewModelCash = ModelCash<PositionViewModel>.GetViewModel(currentUser.Id, POS_EMPLOYEES);
            List<Employee> limitedEmployees = employees;
            LimitEmployeesByName(posViewModelCash, ref limitedEmployees);
            LimitListByPageNumber(currentUser.Id, POS_EMPLOYEES, ref limitedEmployees);
            return limitedEmployees;
        }

        /// <summary>
        /// Ограничение списка сотрудников по имени
        /// </summary>
        /// <param name="posViewModelCash"></param>
        /// <param name="employeesToLimit"></param>
        private void LimitEmployeesByName(PositionViewModel posViewModelCash, ref List<Employee> employeesToLimit)
        {
            string searchEmployeeInitialName = posViewModelCash.SearchEmployeeInitialNameCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchEmployeeInitialName))
                employeesToLimit = employeesToLimit.Where(n => n.GetFullName().ToLower().Contains(searchEmployeeInitialName)).ToList();
        }
        #endregion

        #region Attaching Sub Positions
        /// <summary>
        /// Добавляет список дочерних должностей к модели представления должности
        /// </summary>
        /// <param name="positionViewModel"></param>
        public void AttachSubPositions(PositionViewModel positionViewModel)
        {
            positionViewModel.SubPositions = positionViewModel.GetSubPositions(context)
                .TransformToViewModels<Position, PositionViewModel, PositionTransformer>(
                    transformer: new PositionTransformer(context, resManager),
                    limitingFunc: GetLimitedSubPositionsList);
        }

        private List<Position> GetLimitedSubPositionsList(List<Position> positions)
        {
            PositionViewModel posViewModelCash = ModelCash<PositionViewModel>.GetViewModel(currentUser.Id, POS_SUB_POSS);
            List<Position> limitedPositions = positions;
            LimitSubPositionsByName(posViewModelCash, ref limitedPositions);
            LimitSubPositionsByPrimaryEmployee(posViewModelCash, ref limitedPositions);
            LimitListByPageNumber(currentUser.Id, POS_SUB_POSS, ref limitedPositions);
            return limitedPositions;
        }

        /// <summary>
        /// Ограничение списка дочерних должностей по названию
        /// </summary>
        /// <param name="posViewModelCash"></param>
        /// <param name="positionsToLimit"></param>
        private void LimitSubPositionsByName(PositionViewModel posViewModelCash, ref List<Position> positionsToLimit)
        {
            string searchSubPositionName = posViewModelCash.SearchSubPositionNameCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchSubPositionName))
                positionsToLimit = positionsToLimit.Where(n => n.Name.ToLower().Contains(searchSubPositionName)).ToList();
        }

        /// <summary>
        /// Ограничение списка дочерних должностей по основному сотруднику
        /// </summary>
        /// <param name="posViewModelCash"></param>
        /// <param name="positionsToLimit"></param>
        private void LimitSubPositionsByPrimaryEmployee(PositionViewModel posViewModelCash, ref List<Position> positionsToLimit)
        {
            string searchSubPositionPrimaryEmployee = posViewModelCash.SearchSubPositionPrimaryEmployeeCash.GetValueOrDefault(currentUser.Id);
            if (!string.IsNullOrEmpty(searchSubPositionPrimaryEmployee))
            {
                Division division = context.Divisions.FirstOrDefault(i => i.Id == posViewModelCash.DivisionIdCash.GetValueOrDefault(currentUser.Id));
                TransformCollection(
                    collectionToLimit: ref positionsToLimit,
                    limitingCollection: division?.GetEmployees(context),
                    limitCondition: n => n.GetFullName().ToLower().Contains(searchSubPositionPrimaryEmployee),
                    selectCondition: i => i.Id,
                    removeCondition: (employeeIdList, position) => position.PrimaryEmployeeId == null || !employeeIdList.Contains((Guid)position.PrimaryEmployeeId));
            }
        }
        #endregion

        #region Other Methods
        /// <summary>
        /// Метод выполняет проверку и в случае успеха изменяет подразделение должности
        /// </summary>
        /// <param name="positionViewModel"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public bool TryChangeDivision(PositionViewModel positionViewModel, ModelStateDictionary modelState)
        {
            if (TryChangeDivisionValidate(positionViewModel, modelState))
            {
                Position position = context.Positions
                    .Include(empPos => empPos.EmployeePositions)
                        .ThenInclude(emp => emp.Employee)
                    .FirstOrDefault(i => i.Id == positionViewModel.Id);
                DeletePositionFromEmployees(position);
                position.ParentPositionId = null;
                ResetParentPositionForChilds(position);
                position.PrimaryEmployeeId = null;
                ChangePositionDivision(position, positionViewModel);
                context.Update(position);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Методы выполняет валидацию модели при смене подразделения
        /// </summary>
        /// <param name="positionViewModel"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        private bool TryChangeDivisionValidate(PositionViewModel positionViewModel, ModelStateDictionary modelState)
        {
            Dictionary<string, string> errors = validator.ChangeDivisionCheck(positionViewModel);
            if (errors.Any())
            {
                foreach (KeyValuePair<string, string> error in errors)
                    modelState.AddModelError(error.Key, error.Value);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Метод изменяет подразделение, к которому относится должность
        /// </summary>
        /// <param name="positionViewModel"></param>
        private void ChangePositionDivision(Position position, PositionViewModel positionViewModel)
        {
            Division newDivision = context.GetOrgDivisions(positionViewModel.OrganizationId).FirstOrDefault(n => n.Name == positionViewModel.DivisionName);
            position.Division = newDivision;
            position.DivisionId = newDivision.Id;
        }

        /// <summary>
        /// Добавляет к должности родительскую должность
        /// </summary>
        /// <param name="positionViewModel"></param>
        /// <param name="division"></param>
        /// <param name="position"></param>
        public void SetParentPosition(PositionViewModel positionViewModel, Division division, ref Position position)
        {
            if (string.IsNullOrEmpty(positionViewModel.ParentPositionName))
                position.ParentPositionId = null;
            else
            {
                Position parentPosition = positionViewModel.GetParentPosition(division);
                position.ParentPositionId = parentPosition.Id;
            }
        }

        /// <summary>
        /// Добавляет или отвязывает от должности основного сотрудника
        /// Также добавляет его в список сотрудников
        /// </summary>
        /// <param name="positionViewModel"></param>
        /// <param name="position"></param>
        public void SetPrimaryEmployee(PositionViewModel positionViewModel, ref Position position)
        {
            if (positionViewModel.PrimaryEmployeeId == null || positionViewModel.PrimaryEmployeeId == Guid.Empty)
                position.PrimaryEmployeeId = null;
            else
            {
                Employee primaryEmployee = positionViewModel.GetPrimaryEmployee(context);
                AddEmployeePosition(primaryEmployee, position);
                position.PrimaryEmployeeId = primaryEmployee.Id;
            }
        }

        /// <summary>
        /// Метод добавляет должность в список должностей сотрудника
        /// </summary>
        /// <param name="positionViewModel"></param>
        private void AddEmployeePosition(Employee employee, Position position)
        {
            if (!context.EmployeePositions.Where(pos => pos.EmployeeId == employee.Id).ToList().ContainsPosition(position))
            {
                context.EmployeePositions.Add(new EmployeePosition()
                {
                    Id = Guid.NewGuid(),
                    Employee = employee,
                    EmployeeId = employee.Id,
                    Position = position,
                    PositionId = position.Id
                });
            }
        }

        /// <summary>
        /// Возвращает иерархию родительских должностей
        /// </summary>
        /// <param name="position">Должность, для которой будет находиться иерархия</param>
        /// <param name="constrainingPositionId">Id должности, на которой надо остановиться</param>
        /// <returns></returns>
        public List<Position> GetParentPositionsHierarchy(Position position, Guid? constrainingPositionId = null)
        {
            List<Position> positionsHierarchy = new List<Position>();
            while (true)
            {
                if (position?.ParentPositionId == null) break;
                Position parentPosition = position.GetParentPosition(context);
                if (parentPosition == null) break;
                positionsHierarchy.Add(parentPosition);

                // Если передан ограничитель и id родительской должности равен ограничителю
                if (constrainingPositionId != null && position.ParentPositionId == constrainingPositionId) break;
                position = parentPosition;
            }
            return positionsHierarchy;
        }

        /// <summary>
        /// Получение списка всех дочерних должностей
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public List<Position> GetAllChildPositions(Position position)
        {
            // Находится подразделение и все его должности, из которых исключаются родительские должности
            Division division = context.Divisions.FirstOrDefault(i => i.Id == position.DivisionId);
            return division.GetPositions(context).Except(GetParentPositionsHierarchy(position)).Except(new List<Position>() { position }).ToList();
        }

        /// <summary>
        /// Для выбранной должности находит все дочерние должности в текущем подразделении и для всех обнуляет ParentPositionId(при смене подразделения)
        /// </summary>
        /// <param name="position"></param>
        private void ResetParentPositionForChilds(Position position)
        {
            position.GetSubPositions(context).ForEach(childPosition =>
            {
                childPosition.ParentPositionId = null;
                context.Positions.Update(childPosition);
            });
        }

        /// <summary>
        /// Метод удаляет текущую должность у всех сотрудников подразделения
        /// </summary>
        /// <param name="position"></param>
        private void DeletePositionFromEmployees(Position position)
        {
            position.EmployeePositions.ForEach(employeePosition =>
            {
                CheckEmployeeForLock(employeePosition, position);
                context.EmployeePositions.Remove(employeePosition);
            });
        }

        /// <summary>
        /// Метод проверяет необходимость в блокировке сотрудника, находящихся на заданной должности(требуется при удалении или смене подразделения)
        /// И в случае необходимости лочит его, стирая PrimaryPositionId на сотруднике
        /// </summary>
        /// <param name="position"></param>
        private void CheckEmployeeForLock(EmployeePosition employeePosition, Position position)
        {
            if (employeePosition.Employee.PrimaryPositionId == position.Id)
            {
                employeePosition.Employee.PrimaryPositionId = null;
                employeePosition.Employee.Lock();
                new AccountRepository(context, resManager).CheckAccountsForLock(employeePosition.Employee);
                context.Employees.Update(employeePosition.Employee);
            }
        }
        #endregion
    }
}
