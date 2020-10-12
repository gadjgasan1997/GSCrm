using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Validators
{
    public class OrganizationValidatior : BaseValidator<OrganizationViewModel>
    {
        private readonly User currentUser;
        private const int ORGANIZATION_NAME_MIN_LENGTH = 3;
        public OrganizationValidatior(ApplicationDbContext context, ResManager resManager, User currentUser = null)
            : base(context, resManager)
        {
            this.currentUser = currentUser;
        }

        /// <summary>
        /// Проверка организации при создании
        /// </summary>
        /// <param name="orgViewModel"></param>
        public override Dictionary<string, string> CreationCheck(OrganizationViewModel orgViewModel)
        {
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckOrganizationLength(orgViewModel),
                () => CheckOrganizationNotExists(orgViewModel)
            });
            return errors;
        }

        /// <summary>
        /// Проверка длины названия организации
        /// </summary>
        /// <param name="orgViewModel"></param>
        private void CheckOrganizationLength(OrganizationViewModel orgViewModel)
        {
            orgViewModel.Name = orgViewModel.Name.TrimStartAndEnd();
            if (string.IsNullOrEmpty(orgViewModel.Name) || orgViewModel.Name.Length < ORGANIZATION_NAME_MIN_LENGTH)
                errors.Add("OrganizationNameLength", resManager.GetString("OrganizationNameLength"));
        }

        /// <summary>
        /// Проверка на наличие организации с таким же названием, где владельцем является текущий пользователь
        /// </summary>
        /// <param name="orgViewModel"></param>
        private void CheckOrganizationNotExists(OrganizationViewModel orgViewModel)
        {
            User user = context.Users.FirstOrDefault(n => n.UserName == currentUser.UserName);
            if (context.Organizations.Where(owId => owId.OwnerId == user.Id)?.FirstOrDefault(n => n.Name == orgViewModel.Name) != null)
                errors.Add("OrganizationAlreadyExists", resManager.GetString("OrganizationAlreadyExists"));
        }
    }
}
