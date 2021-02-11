using System;
using System.Linq;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System.Collections.Generic;
using static GSCrm.Utils.CollectionsUtils;

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
        {
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            return organizationRepository.HasPermissionsForSeeOrgItem();
        }

        protected override bool RespsIsCorrectOnCreate(ResponsibilityViewModel responsibilityViewModel)
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("RespCreate", transaction);

        protected override bool TryCreatePrepare(ResponsibilityViewModel responsibilityViewModel)
        {
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
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("RespUpdate", transaction);

        protected override bool TryUpdatePrepare(ResponsibilityViewModel responsibilityViewModel)
        {
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

        protected override bool RespsIsCorrectOnDelete(Responsibility responsibility)
            => new OrganizationRepository(serviceProvider, context).CheckPermissionForOrgGroup("RespDelete", transaction);
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
