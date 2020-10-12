using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Validators
{
    public class DivisionValidatior : BaseValidator<DivisionViewModel>
    {
        private const int DIVISION_NAME_MIN_LENGTH = 3;
        private Organization currentOrganization;
        public DivisionValidatior(ApplicationDbContext context, ResManager resManager) : base(context, resManager) { }

        /// <summary>
        /// Проверка подразделения при создании
        /// </summary>
        /// <param name="divisionViewModel"></param>
        /// <returns></returns>
        public override Dictionary<string, string> CreationCheck(DivisionViewModel divisionViewModel)
        {
            SetUpCurrentOrganization(divisionViewModel);
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckDivisionLength(divisionViewModel),
                () => CheckDivisionNotExists(divisionViewModel),
                () => CheckParentDivisionExists(divisionViewModel)
            });
            return errors;
        }

        /// <summary>
        /// Устанавоивает текущую организацию
        /// </summary>
        /// <param name="divisionViewModel"></param>
        private void SetUpCurrentOrganization(DivisionViewModel divisionViewModel)
            => currentOrganization = context.Organizations.Include(div => div.Divisions).FirstOrDefault(i => i.Id == divisionViewModel.OrganizationId);

        /// <summary>
        /// Проверка длины названия подразделения
        /// </summary>
        /// <param name="divisionViewModel"></param>
        private void CheckDivisionLength(DivisionViewModel divisionViewModel)
        {
            divisionViewModel.Name = divisionViewModel.Name.TrimStartAndEnd();
            if (string.IsNullOrEmpty(divisionViewModel.Name) || divisionViewModel.Name.Length < DIVISION_NAME_MIN_LENGTH)
                errors.Add("DivisionNameLength", resManager.GetString("DivisionNameLength"));
        }

        /// <summary>
        /// Проверка на наличие подразделения с таким же названием в этой организации
        /// </summary>
        /// <param name="divisionViewModel"></param>
        private void CheckDivisionNotExists(DivisionViewModel divisionViewModel)
        {
            List<Division> divisions = currentOrganization.Divisions;
            if (divisions != null)
            {
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
        }

        /// <summary>
        /// Проверка, что в организации существует подразделение с таким названием
        /// </summary>
        /// <param name="divisionViewModel"></param>
        private void CheckParentDivisionExists(DivisionViewModel divisionViewModel)
        {
             if (!string.IsNullOrEmpty(divisionViewModel.ParentDivisionName) && 
                currentOrganization.Divisions.FirstOrDefault(n => n.Name == divisionViewModel.ParentDivisionName) == null)
                errors.Add("DivisionNotExists", resManager.GetString("DivisionNotExists"));
        }
    }
}
