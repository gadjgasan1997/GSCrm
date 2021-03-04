using System;
using System.Linq;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System.Collections.Generic;
using static GSCrm.CommonConsts;
using static GSCrm.Utils.CollectionsUtils;
using GSCrm.Helpers;

namespace GSCrm.Repository
{
    public class ResponsibilityRepository : BaseRepository<Responsibility, ResponsibilityViewModel>
    {
        #region Constructs
        public ResponsibilityRepository(IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(serviceProvider, context)
        { }
        #endregion

        #region Override Methods
        public override bool HasPermissionsForSeeItem(Responsibility responsibility)
            => new OrganizationRepository(serviceProvider, context).HasPermissionsForSeeOrgItem();

        public override ResponsibilityViewModel LoadView(Responsibility responsibility)
        {
            ResponsibilityViewModel respViewModel = cachService.GetCachedCurrentEntity<ResponsibilityViewModel>(currentUser);
            cachService.SetCurrentView(currentUser.Id, RESPONSIBILITY);
            cachService.CacheEntity(currentUser, respViewModel);
            cachService.CacheCurrentEntity(currentUser, respViewModel);
            return respViewModel;
        }

        protected override bool RespsIsCorrectOnCreate(ResponsibilityViewModel responsibilityViewModel)
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("RespCreate");

        protected override bool TryCreatePrepare(ResponsibilityViewModel responsibilityViewModel)
        {
            responsibilityViewModel.Normalize();
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CheckName(responsibilityViewModel),
                () => {
                    List<Responsibility> responsibilities = (List<Responsibility>)transaction.GetParameterValue("Responsibilities");
                    if (responsibilities.FirstOrDefault(resp => resp.Name == responsibilityViewModel.Name) != null)
                        errors.Add("ResponsibilityAlreadyExists", resManager.GetString("ResponsibilityAlreadyExists"));
                }
            });
            return !errors.Any();
        }

        protected override bool RespsIsCorrectOnUpdate(ResponsibilityViewModel responsibilityViewModel)
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("RespUpdate");

        protected override bool TryUpdatePrepare(ResponsibilityViewModel responsibilityViewModel)
        {
            responsibilityViewModel.Normalize();
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CheckName(responsibilityViewModel),
                () => {
                    List<Responsibility> responsibilities = (List<Responsibility>)transaction.GetParameterValue("Responsibilities");
                    if (responsibilities.FirstOrDefault(resp => resp.Id != responsibilityViewModel.Id && resp.Name == responsibilityViewModel.Name) != null)
                        errors.Add("ResponsibilityAlreadyExists", resManager.GetString("ResponsibilityAlreadyExists"));
                }
            });
            return !errors.Any();
        }

        protected override void UpdateCacheOnDelete(Responsibility responsibility)
        {
            if (cachService.TryGetCachedEntity(currentUser, responsibility.OrganizationId, out Organization organization) &&
                cachService.TryGetCachedEntity(currentUser, responsibility.OrganizationId, out OrganizationViewModel organizationViewModel))
            {
                cachService.CacheCurrentEntity(currentUser, organization);
                cachService.CacheCurrentEntity(currentUser, organizationViewModel);
            }
        }

        protected override bool RespsIsCorrectOnDelete(Responsibility responsibility)
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("RespDelete");
        #endregion

        #region Validations
        /// <summary>
        /// Проверка названия
        /// </summary>
        /// <param name="responsibilityViewModel"></param>
        private void CheckName(ResponsibilityViewModel responsibilityViewModel)
        {
            if (string.IsNullOrEmpty(responsibilityViewModel.Name))
                errors.Add("ResponsibilityNameLength", resManager.GetString("ResponsibilityNameLength"));
        }
        #endregion
    }
}
