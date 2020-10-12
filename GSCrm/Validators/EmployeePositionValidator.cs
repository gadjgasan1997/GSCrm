using GSCrm.Data;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GSCrm.Validators
{
    public class EmployeePositionValidator : BaseValidator<EmployeePositionViewModel>
    {
        public EmployeePositionValidator(ApplicationDbContext context, ResManager resManager) : base(context, resManager) { }

        public override Dictionary<string, string> DeleteCheck(Guid id)
        {
            CheckPrimaryPosition(id);
            return errors;
        }

        /// <summary>
        /// Проверка на удаление основной должности
        /// </summary>
        /// <param name="positionViewModel"></param>
        private void CheckPrimaryPosition(Guid id)
        {
            EmployeePosition employeePosition = context.EmployeePositions
                .Include(emp => emp.Employee)
                .FirstOrDefault(i => i.Id == id);
            if (employeePosition.Employee.PrimaryPositionId == employeePosition.PositionId)
                errors.Add("PrimaryPositionIsReadonly", resManager.GetString("PrimaryPositionIsReadonly"));
        }
    }
}
