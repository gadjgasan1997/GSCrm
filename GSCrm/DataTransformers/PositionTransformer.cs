using GSCrm.Data;
using GSCrm.Data.ApplicationInfo;
using GSCrm.Helpers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.CommonConsts;

namespace GSCrm.DataTransformers
{
    public class PositionTransformer : BaseTransformer<Position, PositionViewModel>
    {
        private readonly User currentUser;
        private readonly PositionRepository positionRepository;
        public PositionTransformer(ApplicationDbContext context, ResManager resManager, HttpContext httpContext = null) : base(context, resManager)
        {
            positionRepository = new PositionRepository(context, resManager);
            if (httpContext != null)
                currentUser = httpContext.GetCurrentUser(context);
        }

        public override Position OnModelCreate(PositionViewModel positionViewModel)
        {
            Division division = positionViewModel.GetDivision(context);
            Position position = new Position()
            {
                Division = division,
                DivisionId = division.Id,
                Name = positionViewModel.Name
            };

            positionRepository.SetParentPosition(positionViewModel, division, ref position);
            positionRepository.SetPrimaryEmployee(positionViewModel, ref position);
            return position;
        }

        public override Position OnModelUpdate(PositionViewModel positionViewModel)
        {
            Division division = positionViewModel.GetDivision(context);
            Position position = division.Positions.FirstOrDefault(i => i.Id == positionViewModel.Id);
            position.Name = positionViewModel.Name;
            positionRepository.SetParentPosition(positionViewModel, division, ref position);
            positionRepository.SetPrimaryEmployee(positionViewModel, ref position);
            return position;
        }

        public override PositionViewModel DataToViewModel(Position position)
        {
            List<PositionViewModel> parentPositionsHieararchy = new List<PositionViewModel>();
            positionRepository.GetParentPositionsHierarchy(position).ForEach(parentPosition => parentPositionsHieararchy.Add(DataToViewModelExceptHierarchy(parentPosition)));
            PositionViewModel positionViewModel = DataToViewModelExceptHierarchy(position);
            positionViewModel.PositionsHierarchy = parentPositionsHieararchy;
            positionViewModel.PositionsHierarchy.Insert(0, positionViewModel);
            positionViewModel.PositionsHierarchy.Reverse();
            return positionViewModel;
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
            Organization organization = division.GetOrganization(context);
            Position parentPosition = position.GetParentPosition(context);
            Employee primaryEmployee = position.GetPrimaryEmployee(context);
            PositionViewModel positionViewModel = new PositionViewModel()
            {
                Id = position.Id,
                Name = position.Name,
                DivisionId = division.Id,
                DivisionName = division.Name,
                ParentPositionId = parentPosition?.Id,
                ParentPositionName = parentPosition?.Name,
                OrganizationId = organization.Id,
                OrganizationName = organization.Name,
                PrimaryEmployeeId = primaryEmployee?.Id,
                PrimaryEmployeeInitialName = primaryEmployee?.GetIntialsFullName()
            };
            return positionViewModel;
        }

        public override PositionViewModel UpdateViewModelFromCash(PositionViewModel positionViewModel)
        {
            PositionViewModel posEmployeesView = ModelCash<PositionViewModel>.GetViewModel(currentUser.Id, POS_EMPLOYEES);
            PositionViewModel posSubPositionsView = ModelCash<PositionViewModel>.GetViewModel(currentUser.Id, POS_SUB_POSS);
            positionViewModel.SearchEmployeeInitialName = posEmployeesView.SearchEmployeeInitialNameCash.GetValueOrDefault(currentUser.Id);
            positionViewModel.SearchSubPositionName = posSubPositionsView.SearchSubPositionNameCash.GetValueOrDefault(currentUser.Id);
            positionViewModel.SearchSubPositionPrimaryEmployee = posSubPositionsView.SearchSubPositionPrimaryEmployeeCash.GetValueOrDefault(currentUser.Id);
            return positionViewModel;
        }
    }
}
