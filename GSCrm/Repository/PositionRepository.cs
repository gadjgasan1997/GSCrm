using GSCrm.Mapping;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Models.ViewTypes;
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
using GSCrm.Data;
using System.Text;
using GSCrm.Transactions;
using GSCrm.Models.Enums;

namespace GSCrm.Repository
{
    public class PositionRepository : BaseRepository<Position, PositionViewModel>
    {
        #region Declarations
        private const int POSITION_NAME_MIN_LENGTH = 3;
        /// <summary>
        /// Все типы представлений, связанные с должностями
        /// </summary>
        public static PositionViewType[] PosAllViewTypes => new PositionViewType[] { PositionViewType.POS_EMPLOYEES, PositionViewType.POS_SUB_POSS };
        #endregion

        #region Constructs
        public PositionRepository(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(serviceProvider, context)
        { }
        #endregion

        #region Override Methods
        public override bool HasPermissionsForSeeItem(Position position)
        {
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            return organizationRepository.HasPermissionsForSeeOrgItem();
        }

        protected override bool RespsIsCorrectOnCreate(PositionViewModel positionViewModel)
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForEmployeeGroup("PosCreate", transaction);

        protected override bool TryCreatePrepare(PositionViewModel positionViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CheckPositionLength(positionViewModel),
                () => CheckDivisionLength(positionViewModel),
                () => {
                    if (DivisionExists(positionViewModel))
                        CheckPositionNotExistsOnCreate(positionViewModel);
                },
                () => {
                    if (!string.IsNullOrEmpty(positionViewModel.ParentPositionName))
                        CheckParentPositionExists(positionViewModel);
                },
                () => {
                    if (!string.IsNullOrEmpty(positionViewModel.PrimaryEmployeeInitialName))
                        CheckPrimaryEmployeeExists(positionViewModel);
                }
            });
            return !errors.Any();
        }

        protected override bool RespsIsCorrectOnUpdate(PositionViewModel positionViewModel)
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForEmployeeGroup("PosCreate", transaction);

        protected override bool TryUpdatePrepare(PositionViewModel positionViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CheckDivisionLength(positionViewModel),
                () => CheckPositionLength(positionViewModel),
                () => {
                    if (DivisionExists(positionViewModel))
                        CheckPositionNotExistsOnUpdate(positionViewModel);
                },
                () => {
                    if (!string.IsNullOrEmpty(positionViewModel.ParentPositionName))
                    {
                        InvokeIntermittinActions(errors, new List<Action>()
                        {
                            () => {
                                if (positionViewModel.Name == positionViewModel.ParentPositionName)
                                    errors.Add("ParentAndCurrentPositionsCompare", resManager.GetString("ParentAndCurrentPositionsCompare"));
                            },
                            // При обновлении вначале требуется проверить родительскую должность на существование
                            () => {
                                if (ParentPositionExists(positionViewModel))
                                {
                                    // Затем требуется убедиться, что при установке родительской должности не произойдет циклической зависимости,
                                    // когда для Position 2 родительской является Position 1, и для Position 1 устанавливается в качесте родительской Position 2
                                    CheckPositionsHierarchy(context.Positions.AsNoTracking().FirstOrDefault(i => i.Id == positionViewModel.Id), positionViewModel.Name);
                                }
                            }
                        });
                    }
                },
                () => {
                    if (!string.IsNullOrEmpty(positionViewModel.PrimaryEmployeeInitialName))
                        CheckPrimaryEmployeeExists(positionViewModel);
                }
            });
            return !errors.Any();
        }

        protected override bool RespsIsCorrectOnDelete(Position position)
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForEmployeeGroup("PosCreate", transaction);

        protected override void FailureUpdateHandler(PositionViewModel positionViewModel)
        {
            if (TryGetItemById(positionViewModel.Id, out Position position))
            {
                positionViewModel = map.DataToViewModel(position);
                positionViewModel = new PositionMap(serviceProvider, context).Refresh(positionViewModel, currentUser, PosAllViewTypes);
                AttachEmployees(positionViewModel);
                AttachSubPositions(positionViewModel);
            }
        }

        protected override bool TryDeletePrepare(Position position)
        {
            if (!base.TryDeletePrepare(position)) return false;
            position = context.Positions
                .AsNoTracking()
                .Include(empPos => empPos.EmployeePositions)
                    .ThenInclude(emp => emp.Employee)
                .FirstOrDefault(i => i.Id == position.Id);
            position.EmployeePositions.ForEach(employeePosition => CheckEmployeeForLock(employeePosition, position));
            return true;
        }
        #endregion

        #region Searching
        /// <summary>
        /// Метод очищает поиск по сотрудникам
        /// </summary>
        public void ClearSearchEmployee()
        {
            PositionViewModel posViewModelModelCash = cachService.GetCachedItem<PositionViewModel>(currentUser.Id, POS_EMPLOYEES);
            posViewModelModelCash.SearchEmployeeInitialName = default;
            cachService.CacheItem(currentUser.Id, POS_EMPLOYEES, posViewModelModelCash);
        }

        /// <summary>
        /// Метод очищает поиск по дочерним должностям
        /// </summary>
        public void ClearSearchSubPosition()
        {
            PositionViewModel posViewModelModelCash = cachService.GetCachedItem<PositionViewModel>(currentUser.Id, POS_SUB_POSS);
            posViewModelModelCash.SearchSubPositionName = default;
            posViewModelModelCash.SearchSubPositionPrimaryEmployee = default;
            cachService.CacheItem(currentUser.Id, POS_SUB_POSS, posViewModelModelCash);
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
                .MapToViewModels<Employee, EmployeeViewModel>(
                    map: new EmployeeMap(serviceProvider, context),
                    limitingFunc: GetLimitedEmployeesList);
        }

        private List<Employee> GetLimitedEmployeesList(List<Employee> employees)
        {
            PositionViewModel posViewModelCash = cachService.GetCachedItem<PositionViewModel>(currentUser.Id, POS_EMPLOYEES);
            List<Employee> limitedEmployees = employees;
            LimitEmployeesByName(posViewModelCash, ref limitedEmployees);
            LimitListByPageNumber(POS_EMPLOYEES, ref limitedEmployees);
            return limitedEmployees;
        }

        /// <summary>
        /// Ограничение списка сотрудников по имени
        /// </summary>
        /// <param name="posViewModelCash"></param>
        /// <param name="employeesToLimit"></param>
        private void LimitEmployeesByName(PositionViewModel posViewModelCash, ref List<Employee> employeesToLimit)
        {
            if (!string.IsNullOrEmpty(posViewModelCash.SearchEmployeeInitialName))
                employeesToLimit = employeesToLimit.Where(n => n.GetFullName().ToLower().Contains(posViewModelCash.SearchEmployeeInitialName)).ToList();
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
                .MapToViewModels(new PositionMap(serviceProvider, context), GetLimitedSubPositionsList);
        }

        private List<Position> GetLimitedSubPositionsList(List<Position> positions)
        {
            PositionViewModel posViewModelCash = cachService.GetCachedItem<PositionViewModel>(currentUser.Id, POS_SUB_POSS);
            List<Position> limitedPositions = positions;
            LimitSubPositionsByName(posViewModelCash, ref limitedPositions);
            LimitSubPositionsByPrimaryEmployee(posViewModelCash, ref limitedPositions);
            LimitListByPageNumber(POS_SUB_POSS, ref limitedPositions);
            return limitedPositions;
        }

        /// <summary>
        /// Ограничение списка дочерних должностей по названию
        /// </summary>
        /// <param name="posViewModelCash"></param>
        /// <param name="positionsToLimit"></param>
        private void LimitSubPositionsByName(PositionViewModel posViewModelCash, ref List<Position> positionsToLimit)
        {
            if (!string.IsNullOrEmpty(posViewModelCash.SearchSubPositionName))
                positionsToLimit = positionsToLimit.Where(n => n.Name.ToLower().Contains(posViewModelCash.SearchSubPositionName)).ToList();
        }

        /// <summary>
        /// Ограничение списка дочерних должностей по основному сотруднику
        /// </summary>
        /// <param name="posViewModelCash"></param>
        /// <param name="positionsToLimit"></param>
        private void LimitSubPositionsByPrimaryEmployee(PositionViewModel posViewModelCash, ref List<Position> positionsToLimit)
        {
            if (!string.IsNullOrEmpty(posViewModelCash.SearchSubPositionPrimaryEmployee))
            {
                Division division = context.Divisions.FirstOrDefault(i => i.Id == posViewModelCash.DivisionId);
                TransformCollection(
                    collectionToLimit: ref positionsToLimit,
                    limitingCollection: division?.GetEmployees(context),
                    limitCondition: n => n.GetFullName().ToLower().Contains(posViewModelCash.SearchSubPositionPrimaryEmployee),
                    selectCondition: i => i.Id,
                    removeCondition: (employeeIdList, position) => position.PrimaryEmployeeId == null || !employeeIdList.Contains((Guid)position.PrimaryEmployeeId));
            }
        }
        #endregion

        #region Validations
        /// <summary>
        /// Проверка длины названия подразделения
        /// </summary>
        /// <param name="positionViewModel"></param>
        private void CheckDivisionLength(PositionViewModel positionViewModel)
        {
            positionViewModel.DivisionName = positionViewModel.DivisionName.TrimStartAndEnd();
            if (string.IsNullOrEmpty(positionViewModel.DivisionName) || positionViewModel.DivisionName.Length < POSITION_NAME_MIN_LENGTH)
                errors.Add("DivisionNameLength", resManager.GetString("DivisionNameLength"));
        }

        /// <summary>
        /// Проверка длины названия должности
        /// </summary>
        /// <param name="positionViewModel"></param>
        private void CheckPositionLength(PositionViewModel positionViewModel)
        {
            positionViewModel.Name = positionViewModel.Name.TrimStartAndEnd();
            if (string.IsNullOrEmpty(positionViewModel.Name) || positionViewModel.Name.Length < POSITION_NAME_MIN_LENGTH)
                errors.Add("PositionNameLength", resManager.GetString("PositionNameLength"));
        }

        /// <summary>
        /// Метод проверяет, что существует подразделение с таким названием
        /// </summary>
        /// <param name="positionViewModel"></param>
        private bool DivisionExists(PositionViewModel positionViewModel)
        {
            Organization organization = (Organization)transaction.GetParameterValue("CurrentOrganization");
            Division division = organization.GetDivisions(context).FirstOrDefault(n => n.Name == positionViewModel.DivisionName);
            if (division == null)
            {
                errors.Add("DivisionNotExists", resManager.GetString("DivisionNotExists"));
                return false;
            }
            transaction.AddParameter("Division", division);
            return true;
        }

        /// <summary>
        /// Проверка на отсутствие должности с таким же названием в этом подразделении
        /// </summary>
        /// <param name="positionViewModel"></param>
        private void CheckPositionNotExistsOnCreate(PositionViewModel positionViewModel)
        {
            Division division = (Division)transaction.GetParameterValue("Division");
            Position positionWithSameName = division.GetPositions(context).FirstOrDefault(n => n.Name == positionViewModel.Name);
            if (!string.IsNullOrEmpty(positionWithSameName?.Name))
                errors.Add("PositionAlreadyExists", resManager.GetString("PositionAlreadyExists"));
        }

        /// <summary>
        /// Проверка на отсутствие должности с таким же названием в этом подразделении при обновлении записи
        /// </summary>
        /// <param name="positionViewModel"></param>
        private void CheckPositionNotExistsOnUpdate(PositionViewModel positionViewModel)
        {
            Division division = (Division)transaction.GetParameterValue("Division");
            Position positionWithSameName = division.GetPositions(context).FirstOrDefault(n => n.Name == positionViewModel.Name);
            if (positionWithSameName != null && positionWithSameName.Id != positionViewModel.Id)
                errors.Add("PositionAlreadyExists", resManager.GetString("PositionAlreadyExists"));
        }

        /// <summary>
        /// Метод проверяет, что существует родительскую должность с таким названием
        /// </summary>
        /// <param name="positionViewModel"></param>
        private bool CheckParentPositionExists(PositionViewModel positionViewModel)
        {
            Position parentPosition = positionViewModel.GetParentPosition((Division)transaction.GetParameterValue("Division"), context);
            if (parentPosition == null)
            {
                errors.Add("PositionNotExists", resManager.GetString("PositionNotExists"));
                return false;
            }
            transaction.AddParameter("ParentPosition", parentPosition);
            return true;
        }

        /// <summary>
        /// Проверка на существование основного сотрудника для должности
        /// </summary>
        /// <param name="positionViewModel"></param>
        private void CheckPrimaryEmployeeExists(PositionViewModel positionViewModel)
        {
            // Если при создании должности не был выбран сотрудник из выпадающего списка
            if (positionViewModel.PrimaryEmployeeId == null)
            {
                errors.Add("EmployeeNotExists", resManager.GetString("EmployeeNotExists"));
                return;
            }
            Employee primaryEmployee = positionViewModel.GetPrimaryEmployee(context);
            // Если не был найден сотрудник по id
            if (primaryEmployee == null)
            {
                errors.Add("EmployeeNotExists", resManager.GetString("EmployeeNotExists"));
                return;
            }
            // Если инициалы найденного по id сотрудника не совпадают инциалами, принятыми с фронта
            // (например, если сотрудник был выбран, а затем часть имени была стерта, но id выбранного сотрудника при этом остался)
            if (primaryEmployee.GetIntialsFullName() != positionViewModel.PrimaryEmployeeInitialName)
                errors.Add("EmployeeNotExists", resManager.GetString("EmployeeNotExists"));
            transaction.AddParameter("PrimaryEmployee", primaryEmployee);
        }

        /// <summary>
        /// Метод пытается получить родительскую должность
        /// </summary>
        /// <param name="positionViewModel"></param>
        private bool ParentPositionExists(PositionViewModel positionViewModel)
        {
            Division division = (Division)transaction.GetParameterValue("Division");
            Position parentPosition = positionViewModel.GetParentPosition(division, context);
            if (parentPosition == null)
            {
                errors.Add("PositionNotExists", resManager.GetString("PositionNotExists"));
                return false;
            }
            transaction.AddParameter("ParentPosition", parentPosition);
            return true;
        }

        /// <summary>
        /// Метод проверряет, что устанавливаемая как родительская должность отсутсвует в списке дочерних должностей у обновляемой
        /// </summary>
        /// <param name="currentPosition">Текущая изменяемая должность</param>
        /// <param name="currentPositinoName">Название текущей изменяемой должность, так как оно может измениться в процессе обновления</param>
        private void CheckPositionsHierarchy(Position currentPosition, string currentPositinoName)
        {
            Position newParentPosition = (Position)transaction.GetParameterValue("ParentPosition");
            if (GetParentPositionsHierarchy(newParentPosition).Select(i => i.Id).Contains(currentPosition.Id))
            {
                errors.Add("PositionCannotBeParent", new StringBuilder()
                    .Append("Невозможно добавить должнось ").Append(newParentPosition.Name)
                    .Append(" как родительскую для ").Append(currentPositinoName)
                    .Append(". Так как добавляемая должность находится в дочерних у текущей.")
                    .ToString());
            }
        }

        /// <summary>
        /// Методы выполняет валидацию модели при смене подразделения
        /// </summary>
        /// <param name="positionViewModel"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        private bool TryChangeDivisionValidate(PositionViewModel positionViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (!new OrganizationRepository(serviceProvider, context).CheckPermissionForEmployeeGroup("PosChangeDiv", transaction))
                         AddHasNoPermissionsError(OperationType.ChangePositionDivision);
                },
                () => CheckDivisionLength(positionViewModel),
                () => {
                    if (DivisionExists(positionViewModel))
                        CheckDivisionsNotCompare(positionViewModel);
                }
            });
            return !errors.Any();
        }

        /// <summary>
        /// Метод проверяет, что выбрано отличное от текущего подразделение
        /// </summary>
        /// <param name="positionViewModel"></param>
        private void CheckDivisionsNotCompare(PositionViewModel positionViewModel)
        {
            Division newDivision = (Division)transaction.GetParameterValue("Division");
            Position position = context.Positions.AsNoTracking().FirstOrDefault(i => i.Id == positionViewModel.Id);
            if (position.DivisionId == newDivision.Id)
                errors.Add("ThisPositionDivisionIsAlreadySelect", resManager.GetString("ThisPositionDivisionIsAlreadySelect"));
        }
        #endregion

        #region Other Methods
        /// <summary>
        /// Метод выполняет проверку и в случае успеха изменяет подразделение должности
        /// </summary>
        /// <param name="positionViewModel"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public bool TryChangeDivision(PositionViewModel positionViewModel, out Dictionary<string, string> errors)
        {
            transaction = transactionFactory.Create(currentUser.Id, OperationType.ChangePositionDivision, positionViewModel);
            if (TryChangeDivisionValidate(positionViewModel))
            {
                Position position = context.Positions
                    .AsNoTracking()
                    .Include(empPos => empPos.EmployeePositions)
                        .ThenInclude(emp => emp.Employee)
                    .FirstOrDefault(i => i.Id == positionViewModel.Id);
                DeletePositionFromEmployees(position);
                ResetParentPositionForChilds(position);

                Division newDivision = context.GetOrgDivisions(positionViewModel.OrganizationId).FirstOrDefault(n => n.Name == positionViewModel.DivisionName);
                position.Division = newDivision;
                position.DivisionId = newDivision.Id;
                position.PrimaryEmployeeId = null;
                position.ParentPositionId = null;
                transaction.AddChange(position, EntityState.Modified);
                if (transactionFactory.TryCommit(transaction, this.errors))
                {
                    transactionFactory.Close(transaction);
                    errors = this.errors;
                    return true;
                }
            }
            transactionFactory.Close(transaction, TransactionStatus.Error);
            errors = this.errors;
            return false;
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
                transaction.AddChange(childPosition, EntityState.Modified);
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
                transaction.AddChange(employeePosition, EntityState.Deleted);
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
                employeePosition.Employee.Lock(EmployeeLockReason.PrimaryPositionAbsent);
                new AccountRepository(serviceProvider, context).CheckAccountsForLock(employeePosition.Employee, transaction);
                transaction.AddChange(employeePosition.Employee, EntityState.Modified);
            }
        }
        #endregion
    }
}
