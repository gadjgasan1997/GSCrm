using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Models.ViewTypes;
using GSCrm.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;
using GSCrm.Data;
using Microsoft.EntityFrameworkCore;
using GSCrm.Transactions;

namespace GSCrm.Mapping
{
    public class PositionMap : BaseMap<Position, PositionViewModel>
    {
        public PositionMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override Position OnModelCreate(PositionViewModel positionViewModel)
        {
            base.OnModelCreate(positionViewModel);
            Division division = (Division)transaction.GetParameterValue("Division");
            Position position = new Position()
            {
                OrganizationId = division.OrganizationId,
                DivisionId = division.Id,
                Name = positionViewModel.Name
            };
            AddParentPosition(position, positionViewModel.ParentPositionName);
            AddPrimaryEmployee(position, positionViewModel);
            transaction.AddChange(position, EntityState.Added);
            return position;
        }

        public override Position OnModelUpdate(PositionViewModel positionViewModel)
        {
            Position position = base.OnModelUpdate(positionViewModel);
            position.Name = positionViewModel.Name;
            AddParentPosition(position, positionViewModel.ParentPositionName);
            AddPrimaryEmployee(position, positionViewModel);
            return position;
        }

        public override PositionViewModel DataToViewModel(Position position)
            => DataToViewModelExceptHierarchy(position);

        /// <summary>
        /// Необходим для получения более подробной информации о должности, чем дает метод "DataToViewModel"
        /// Вызывается при проваливании в должность
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public PositionViewModel DataToViewModelExt(Position position)
        {
            PositionViewModel positionViewModel = DataToViewModelExceptHierarchy(position);
            if (position.ParentPositionId != null)
            {
                List<PositionViewModel> parentPositionsHieararchy = new List<PositionViewModel>();
                PositionRepository positionRepository = new PositionRepository(serviceProvider, context);
                positionRepository.GetParentPositionsHierarchy(position).ForEach(parentPosition =>
                    parentPositionsHieararchy.Add(DataToViewModelExceptHierarchy(parentPosition)));
                positionViewModel.PositionsHierarchy = parentPositionsHieararchy;
                positionViewModel.PositionsHierarchy.Insert(0, positionViewModel);
                positionViewModel.PositionsHierarchy.Reverse();
            }
            return positionViewModel;
        }

        public PositionViewModel Refresh(PositionViewModel positionViewModel, User currentUser, PositionViewType[] positionViewTypes)
        {
            positionViewTypes.ToList().ForEach(positionViewType =>
            {
                switch (positionViewType)
                {
                    // Восстановление данных поиска по сотрудникам
                    case PositionViewType.POS_EMPLOYEES:
                        PositionViewModel positionEmployeesCash = cachService.GetCachedItem<PositionViewModel>(currentUser.Id, POS_EMPLOYEES);
                        positionViewModel.SearchEmployeeInitialName = positionEmployeesCash.SearchEmployeeInitialName;
                        break;

                    // Восстановление данных поиска по дочерним должностям
                    case PositionViewType.POS_SUB_POSS:
                        PositionViewModel positionSubPositionsCash = cachService.GetCachedItem<PositionViewModel>(currentUser.Id, POS_SUB_POSS);
                        positionViewModel.SearchSubPositionName = positionSubPositionsCash.SearchSubPositionName;
                        positionViewModel.SearchSubPositionPrimaryEmployee = positionSubPositionsCash.SearchSubPositionPrimaryEmployee;
                        break;

                    default:
                        break;
                }
            });
            return positionViewModel;
        }

        /// <summary>
        /// Метод меняет подразделение у долнжости
        /// </summary>
        /// <param name="position"></param>
        /// <param name="positionViewModel"></param>
        public void ChangeDivision(Position position, PositionViewModel positionViewModel)
        {
            SetTransaction(OperationType.ChangePositionDivision);
            Division newDivision = context.Divisions.FirstOrDefault(div => div.OrganizationId == position.OrganizationId && div.Name == positionViewModel.DivisionName);
            position.DivisionId = newDivision.Id;
            position.PrimaryEmployeeId = null;
            position.ParentPositionId = null;
            transaction.AddChange(position, EntityState.Modified);
        }

        /// <summary>
        /// Разблокировка при отсутствии подразделения
        /// </summary>
        /// <param name="position"></param>
        public void UnlockOnDivisionAbsent(Position position)
        {
            SetTransaction(OperationType.UnlockPosition);
            position.Unlock();
            Division newDivision = (Division)transaction.GetParameterValue("Division");
            position.DivisionId = newDivision.Id;
            transaction.AddChange(position, EntityState.Modified);
        }

        /// <summary>
        /// Преобразует модель уровня данных в модель уровня отображения, НЕ выполняя расчет иерархии родительских должностей
        /// Требуется, так как, если запускать метод "GetViewModelsFromData" на иерархии должностей, то он для каждой
        /// родительской должности будет заново получать ее иерархию, которая уже вычислена
        /// Например. Для иерархии Pos4 - Pos3 - Pos2 - Pos1, вначале для Pos4 вычислится Pos3 - Pos2 - Pos1.
        /// Затем, для Pos3, Pos2 - Pos1. Затем, для Pos2 будет получена должность Pos1.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private PositionViewModel DataToViewModelExceptHierarchy(Position position)
        {
            Division division = position.GetDivision(context);
            Organization organization = context.Organizations.AsNoTracking().FirstOrDefault(org => org.Id == position.OrganizationId);
            Position parentPosition = position.GetParentPosition(context);
            Employee primaryEmployee = position.GetPrimaryEmployee(context);
            PositionViewModel positionViewModel = new PositionViewModel()
            {
                Id = position.Id,
                Name = position.Name,
                DivisionId = division?.Id,
                DivisionName = division?.Name,
                ParentPositionId = parentPosition?.Id,
                ParentPositionName = parentPosition?.Name,
                OrganizationId = organization.Id,
                OrganizationName = organization.Name,
                PrimaryEmployeeId = primaryEmployee?.Id,
                PrimaryEmployeeInitialName = primaryEmployee?.GetIntialsFullName(),
                PositionStatus = position.PositionStatus,
                PositionLockReason = position.PositionLockReason
            };
            return positionViewModel;
        }

        private void AddParentPosition(Position position, string parentPositionName)
        {
            if (string.IsNullOrEmpty(parentPositionName))
                position.ParentPositionId = null;
            else
            {
                Position parentPosition = (Position)transaction.GetParameterValue("ParentPosition");
                position.ParentPositionId = parentPosition.Id;
            }
        }

        private void AddPrimaryEmployee(Position position, PositionViewModel positionViewModel)
        {
            if (string.IsNullOrEmpty(positionViewModel.PrimaryEmployeeInitialName) || positionViewModel.PrimaryEmployeeId == null)
                position.PrimaryEmployeeId = null;
            else
            {
                Employee primaryEmployee = (Employee)transaction.GetParameterValue("PrimaryEmployee");
                List<EmployeePosition> employeePositions = context.EmployeePositions.AsNoTracking().Where(pos => pos.EmployeeId == primaryEmployee.Id).ToList();
                if (!employeePositions.Select(pos => pos.PositionId).Contains(position.Id))
                {
                    transaction.AddChange(new EmployeePosition()
                    {
                        Id = Guid.NewGuid(),
                        Employee = primaryEmployee,
                        EmployeeId = primaryEmployee.Id,
                        Position = position,
                        PositionId = position.Id
                    }, EntityState.Added);
                }
                position.PrimaryEmployeeId = primaryEmployee.Id;
            }
        }
    }
}
