using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Mapping;
using GSCrm.Models.Enums;
using GSCrm.Transactions;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using static GSCrm.CommonConsts;
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Repository
{
    public class PositionRepository : BaseRepository<Position, PositionViewModel>
    {
        #region Declarations
        private const int POSITION_NAME_MIN_LENGTH = 3;
        #endregion

        #region Constructs
        public PositionRepository(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(serviceProvider, context)
        { }
        #endregion

        #region Override Methods
        public override bool HasPermissionsForSeeItem(Position position)
            => new OrganizationRepository(serviceProvider, context).HasPermissionsForSeeOrgItem();

        public override PositionViewModel LoadView(Position position)
        {
            PositionViewModel positionViewModel = cachService.GetCachedCurrentEntity<PositionViewModel>(currentUser);

            // Прикрепление всех сущностей
            if (positionViewModel.PositionStatus == PositionStatus.Active)
            {
                AttachEmployees(positionViewModel);
                AttachSubPositions(positionViewModel);
            }

            // Кеширование результата
            cachService.SetCurrentView(currentUser.Id, POSITION);
            cachService.CacheEntity(currentUser, positionViewModel);
            cachService.CacheCurrentEntity(currentUser, positionViewModel);
            return positionViewModel;
        }

        protected override bool RespsIsCorrectOnCreate(PositionViewModel positionViewModel)
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("PosCreate");

        protected override bool TryCreatePrepare(PositionViewModel positionViewModel)
        {
            positionViewModel.Normalize();
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
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("PosCreate");

        protected override bool TryUpdatePrepare(PositionViewModel positionViewModel)
        {
            positionViewModel.Normalize();
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

        protected override void UpdateCacheOnDelete(Position position)
        {
            if (cachService.TryGetCachedEntity(currentUser, position.OrganizationId, out Organization organization) &&
                cachService.TryGetCachedEntity(currentUser, position.OrganizationId, out OrganizationViewModel organizationViewModel))
            {
                cachService.CacheCurrentEntity(currentUser, organization);
                cachService.CacheCurrentEntity(currentUser, organizationViewModel);
            }
        }

        protected override bool RespsIsCorrectOnDelete(Position position)
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("PosCreate");

        protected override bool TryDeletePrepare(Position position)
        {
            if (!base.TryDeletePrepare(position)) return false;
            position = context.Positions.AsNoTracking().FirstOrDefault(i => i.Id == position.Id);
            CheckEmployeePositions(position);
            return true;
        }
        #endregion

        #region Searching
        public void SearchEmployee(PositionViewModel positionViewModel)
        {
            positionViewModel.NormalizeSearch();
            PositionViewModel cachedViewModel = cachService.GetCachedCurrentEntity<PositionViewModel>(currentUser);
            cachedViewModel.SearchEmployeeInitialName = positionViewModel.SearchEmployeeInitialName;
        }

        public void ClearSearchEmployee()
        {
            PositionViewModel cachedViewModel = cachService.GetCachedCurrentEntity<PositionViewModel>(currentUser);
            cachedViewModel.SearchEmployeeInitialName = default;
        }
        
        public void SearchSubPosition(PositionViewModel positionViewModel)
        {
            positionViewModel.NormalizeSearch();
            PositionViewModel cachedViewModel = cachService.GetCachedCurrentEntity<PositionViewModel>(currentUser);
            cachedViewModel.SearchSubPositionName = positionViewModel.SearchSubPositionName;
            cachedViewModel.SearchSubPositionPrimaryEmployee = positionViewModel.SearchSubPositionPrimaryEmployee;
        }

        public void ClearSearchSubPosition()
        {
            PositionViewModel cachedViewModel = cachService.GetCachedCurrentEntity<PositionViewModel>(currentUser);
            cachedViewModel.SearchSubPositionName = default;
            cachedViewModel.SearchSubPositionPrimaryEmployee = default;
        }
        #endregion

        #region Attaching Employees
        /// <summary>
        /// Добавляет список работников к модели представления должности
        /// </summary>
        /// <param name="positionViewModel"></param>
        public void AttachEmployees(PositionViewModel positionViewModel)
            => positionViewModel.PositionEmployees = positionViewModel.GetEmployees(context)
                .MapToViewModels(positionViewModel, new EmployeeMap(serviceProvider, context), (positionViewModel, employees) =>
                    GetLimitedEmployeesList(positionViewModel, employees));

        private List<Employee> GetLimitedEmployeesList(PositionViewModel positionViewModel, List<Employee> employees)
        {
            List<Employee> limitedEmployees = employees;
            LimitEmployeesByName(positionViewModel, ref limitedEmployees);
            LimitViewItemsByPageNumber(positionViewModel.Id, POS_EMPLOYEES, ref limitedEmployees);
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
            => positionViewModel.SubPositions = positionViewModel.GetSubPositions(context)
                .MapToViewModels(positionViewModel, new PositionMap(serviceProvider, context), (positionViewModel, positions) =>
                    GetLimitedSubPositionsList(positionViewModel, positions));

        private List<Position> GetLimitedSubPositionsList(PositionViewModel positionViewModel, List<Position> positions)
        {
            List<Position> limitedPositions = positions;
            LimitSubPositionsByName(positionViewModel, ref limitedPositions);
            LimitSubPositionsByPrimaryEmployee(positionViewModel, ref limitedPositions);
            LimitViewItemsByPageNumber(positionViewModel.Id, POS_SUB_POSS, ref limitedPositions);
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
            if (string.IsNullOrEmpty(positionViewModel.DivisionName) || positionViewModel.DivisionName.Length < POSITION_NAME_MIN_LENGTH)
                errors.Add("DivisionNameLength", resManager.GetString("DivisionNameLength"));
        }

        /// <summary>
        /// Проверка длины названия должности
        /// </summary>
        /// <param name="positionViewModel"></param>
        private void CheckPositionLength(PositionViewModel positionViewModel)
        {
            if (string.IsNullOrEmpty(positionViewModel.Name) || positionViewModel.Name.Length < POSITION_NAME_MIN_LENGTH)
                errors.Add("PositionNameLength", resManager.GetString("PositionNameLength"));
        }

        /// <summary>
        /// Метод проверяет, что существует подразделение с таким названием
        /// </summary>
        /// <param name="positionViewModel"></param>
        private bool DivisionExists(PositionViewModel positionViewModel)
        {
            Organization organization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
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
        /// <returns></returns>
        private bool TryChangeDivisionValidate(PositionViewModel positionViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (!new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("PosChangeDiv"))
                         AddHasNoPermissionsError(OperationType.ChangePositionDivision);
                },
                () => CheckDivisionLength(positionViewModel),
                () => {
                    if (DivisionExists(positionViewModel))
                        CheckDivisionsNotCompare();
                }
            });
            return !errors.Any();
        }

        /// <summary>
        /// Метод проверяет, что выбрано отличное от текущего подразделение
        /// </summary>
        /// <param name="positionViewModel"></param>
        private void CheckDivisionsNotCompare()
        {
            Division newDivision = (Division)transaction.GetParameterValue("Division");
            Position position = cachService.GetCachedCurrentEntity<Position>(currentUser);
            if (position.DivisionId == newDivision.Id)
                errors.Add("ThisPositionDivisionIsAlreadySelect", resManager.GetString("ThisPositionDivisionIsAlreadySelect"));
        }

        /// <summary>
        /// Метод выполняет проверки, необходимые при разблокировке должности
        /// </summary>
        /// <param name="positionViewModel"></param>
        /// <returns></returns>
        private bool TryUnlockValidate(PositionViewModel positionViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {   
                () => {
                    if (!new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("PosUnlock"))
                         AddHasNoPermissionsError(OperationType.UnlockPosition);
                },
                () => CheckDivisionLength(positionViewModel),
                () => DivisionExists(positionViewModel)
            });
            return !errors.Any();
        }
        #endregion

        #region Other Methods
        /// <summary>
        /// Метод выполняет проверку и в случае успеха изменяет подразделение должности
        /// </summary>
        /// <param name="positionViewModel"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public bool TryChangeDivision(PositionViewModel positionViewModel, out Dictionary<string, string> errors)
        {
            positionViewModel.Normalize();
            transaction = viewModelsTF.Create(currentUser.Id, OperationType.ChangePositionDivision, positionViewModel);
            if (TryChangeDivisionValidate(positionViewModel))
            {
                Position position = cachService.GetCachedCurrentEntity<Position>(currentUser);
                CheckEmployeePositions(position);
                ResetParentPositionForChilds(position);
                
                // Маппинг и попытка сделать коммит
                new PositionMap(serviceProvider, context).ChangeDivision(position, positionViewModel);
                if (viewModelsTF.TryCommit(transaction, this.errors))
                {
                    viewModelsTF.Close(transaction);
                    errors = this.errors;
                    return true;
                }
            }
            viewModelsTF.Close(transaction, TransactionStatus.Error);
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
                if (position.GetParentPosition(context) is not Position parentPosition) break;

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
        /// Метод проверяет необходимость в блокировке сотрудников, находящихся на заданной должности
        /// И в случае необходимости лочит их, стирая PrimaryPositionId на сотруднике
        /// </summary>
        /// <param name="position"></param>
        private void CheckEmployeePositions(Position position)
        {
            // TODO Сейчас бд кверится дважды, чтобы изменения, сделанные в этом методе не отразились на том, что будет записано в транзакцию в параметр
            // Сделать клонирование списка "transactionEmpPositions"
            // Запоминается список сотрудников, занимаемых должность, который будет необходим в дальнейшем для рассылки им уведомления об удалении должности
            Func<EmployeePosition, bool> predicate = empPos => empPos.PositionId == position.Id;
            List<EmployeePosition> transactionEmpPositions = context.EmployeePositions.AsNoTracking().Include(emp => emp.Employee).Where(predicate).ToList();
            transaction.AddParameter("PosEmployees", transactionEmpPositions);

            // Для каждого сотрудника, занимающего должность
            List<EmployeePosition> empPositions = context.EmployeePositions.AsNoTracking().Include(emp => emp.Employee).Where(predicate).ToList();
            empPositions.ForEach(employeePosition =>
            {
                if (employeePosition.Employee.PrimaryPositionId == position.Id)
                {
                    employeePosition.Employee.PrimaryPositionId = null;
                    employeePosition.Employee.Lock(EmployeeLockReason.PrimaryPositionAbsent);

                    // Если должность была заблокирована, проходится по всем сотрудникам, занимающих должность,
                    // блокируя всех клиентов, у которых данный сотрудник является единственным в команде по клиенту
                    new AccountRepository(serviceProvider, context).CheckAccountsForLock(employeePosition.Employee, transaction);
                    transaction.AddChange(employeePosition.Employee, EntityState.Modified);
                }
                transaction.AddChange(employeePosition, EntityState.Deleted);
            });
        }

        /// <summary>
        /// Метод пытается разблокировать должность
        /// </summary>
        /// <param name="positionViewModel"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public bool TryUnlock(ref PositionViewModel positionViewModel, out Dictionary<string, string> errors)
        {
            positionViewModel.Normalize();

            // Получение должности из бд
            transaction = viewModelsTF.Create(currentUser.Id, OperationType.UnlockPosition, positionViewModel);
            Guid positionId = positionViewModel.Id;
            Position position = context.Positions.AsNoTracking().FirstOrDefault(i => i.Id == positionId);

            // В зависимости от причины, по которой была заблокирована должность
            switch (position.PositionLockReason)
            {
                default:
                case PositionLockReason.DivisionAbsent:
                    if (TryUnlockValidate(positionViewModel))
                    {
                        new PositionMap(serviceProvider, context).UnlockOnDivisionAbsent(position);
                        if (viewModelsTF.TryCommit(transaction, this.errors))
                        {
                            viewModelsTF.Close(transaction);
                            errors = this.errors;
                            return true;
                        }
                    }
                    break;
            }

            // Иначе данные из бд преобразуются в данные для отображения без прикрепления контактов и должностей
            errors = this.errors;
            positionViewModel = map.DataToViewModel(position);
            viewModelsTF.Close(transaction, TransactionStatus.Error);
            return false;
        }
        #endregion
    }
}
