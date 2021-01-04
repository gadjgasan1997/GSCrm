using GSCrm.Data;
using GSCrm.Mapping;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GSCrm.Helpers
{
    public static class EmployeePositionHelpers
    {
        /// <summary>
        /// Метод возвращает список моделей должности из списка моделей должностей сотрудника
        /// </summary>
        /// <param name="employeePositionViewModel"></param>
        /// <returns></returns>
        public static List<Position> GetPositionsFromEmployeePositions(this List<EmployeePosition> employeePositions, ApplicationDbContext context)
        {
            List<Position> positionViewModels = new List<Position>();
            employeePositions.ForEach(employeePosition =>
            {
                positionViewModels.Add(context.Positions.FirstOrDefault(i => i.Id == employeePosition.PositionId));
            });
            return positionViewModels;
        }

        /// <summary>
        /// Ограничение списка подразделений по названию родительского
        /// </summary>
        /// <param name="positionsToLimit"></param>
        /// <param name="context"></param>
        /// <param name="employeeViewModelCash"></param>
        public static List<EmployeePosition> LimitByParent(this List<EmployeePosition> positionsToLimit, ApplicationDbContext context, EmployeeViewModel employeeViewModelCash, string parentPosName)
        {
            if (positionsToLimit.Count > 0 && !string.IsNullOrEmpty(parentPosName))
            {
                Division division = employeeViewModelCash.GetDivision(context);
                if (division != null)
                {
                    // Поиск должностей, содержащих в названии введенное значение в поле "SearchParentPosName"
                    List<Position> divisionPositions = division.GetPositions(context);
                    Func<Position, bool> parentPosPredicate = n => n.Name.ToLower().Contains(parentPosName);
                    List<Position> parentPositions = divisionPositions.Where(parentPosPredicate).ToList();

                    if (parentPositions.Count > 0)
                    {
                        List<Guid> parentPositionsId = parentPositions.Select(i => i.Id).ToList();
                        List<Position> positionsToRemove = new List<Position>();

                        // Добавление должностей, подлежащих удалению из коллекции "positionsToLimit"
                        divisionPositions.ForEach(divisionPosition =>
                        {
                            if (divisionPosition.ParentPositionId == null || !parentPositionsId.Contains((Guid)divisionPosition.ParentPositionId))
                                positionsToRemove.Add(divisionPosition);
                        });

                        // Удаление должностей из коллекции "positionsToLimit"
                        List<EmployeePosition> constrainedPositions = new List<EmployeePosition>(positionsToLimit);
                        positionsToLimit.ForEach(positionToLimit =>
                        {
                            if (positionsToRemove.Contains(positionToLimit.Position))
                                constrainedPositions.Remove(positionToLimit);
                        });
                        positionsToLimit = constrainedPositions;
                    }
                    else positionsToLimit = new List<EmployeePosition>();
                }
            }
            return positionsToLimit;
        }
    }
}
