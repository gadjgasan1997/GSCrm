using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using GSCrm.Data;
using System.Linq;
using System.Collections.Generic;
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Repository
{
    public class DivisionRepository : BaseRepository<Division, DivisionViewModel>
    {
        private const int DIVISION_NAME_MIN_LENGTH = 3;

        #region Constructs
        public DivisionRepository(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(serviceProvider, context)
        { }
        #endregion

        #region Override Methods
        protected override bool RespsIsCorrectOnCreate(DivisionViewModel divisionViewModel)
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("DivCreate");

        protected override bool TryCreatePrepare(DivisionViewModel divisionViewModel)
        {
            divisionViewModel.Normalize();
            Organization currentOrganization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CheckDivisionLength(divisionViewModel),
                () => CheckParentDivisionExists(divisionViewModel, currentOrganization),
                () => CheckDivisionNotExists(divisionViewModel, currentOrganization)
            });
            return !errors.Any();
        }

        protected override void UpdateCacheOnDelete(Division division)
        {
            if (cachService.TryGetCachedEntity(currentUser, division.OrganizationId, out Organization organization) &&
                cachService.TryGetCachedEntity(currentUser, division.OrganizationId, out OrganizationViewModel organizationViewModel))
            {
                cachService.CacheCurrentEntity(currentUser, organization);
                cachService.CacheCurrentEntity(currentUser, organizationViewModel);
            }
        }

        protected override bool RespsIsCorrectOnDelete(Division division)
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("DivDelete");
        #endregion

        #region Validations
        /// <summary>
        /// Проверка длины названия подразделения
        /// </summary>
        /// <param name="divisionViewModel"></param>
        private void CheckDivisionLength(DivisionViewModel divisionViewModel)
        {
            if (string.IsNullOrEmpty(divisionViewModel.Name) || divisionViewModel.Name.Length < DIVISION_NAME_MIN_LENGTH)
                errors.Add("DivisionNameLength", resManager.GetString("DivisionNameLength"));
        }

        /// <summary>
        /// Проверка на наличие подразделения с таким же названием в этой организации
        /// </summary>
        /// <param name="divisionViewModel"></param>
        /// <param name="currentOrganization"></param>
        private void CheckDivisionNotExists(DivisionViewModel divisionViewModel, Organization currentOrganization)
        {
            List<Division> divisions = currentOrganization.GetDivisions(context);

            // Если у нового подразделения есть родительское, ограничение списка по id родителя
            if (!string.IsNullOrEmpty(divisionViewModel.ParentDivisionName))
            {
                Division parentDivision = divisions.FirstOrDefault(n => n.Name == divisionViewModel.ParentDivisionName);
                divisions = divisions.Where(divId => divId.ParentDivisionId == parentDivision.Id).ToList();
            }

            // Подразделение с тем же названием, что и создаваемое
            Division divisionWithSameName = divisions.FirstOrDefault(n => n.Name == divisionViewModel.Name);
            if (divisionWithSameName != null)
                errors.Add("DivisionAlreadyExists", resManager.GetString("DivisionAlreadyExists"));
        }

        /// <summary>
        /// Проверка, что в организации существует подразделение с таким названием
        /// </summary>
        /// <param name="divisionViewModel"></param>
        /// <param name="currentOrganization"></param>
        private void CheckParentDivisionExists(DivisionViewModel divisionViewModel, Organization currentOrganization)
        {
            Division parentDivision = currentOrganization.GetDivisions(context).FirstOrDefault(n => n.Name == divisionViewModel.ParentDivisionName);
            if (!string.IsNullOrEmpty(divisionViewModel.ParentDivisionName) && parentDivision == null)
            {
                errors.Add("DivisionNotExists", resManager.GetString("DivisionNotExists"));
                return;
            }
            transaction.AddParameter("ParentDivision", parentDivision);
        }
        #endregion
    }
}