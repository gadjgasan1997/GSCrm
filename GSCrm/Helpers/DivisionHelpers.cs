using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace GSCrm.Helpers
{
    public static class DivisionHelpers
    {
        /// <summary>
        /// Получает текущую выбранную организацию для объекта Division
        /// </summary>
        /// <param name="division"></param>
        /// <returns></returns>
        public static Organization GetOrganization(this Division division, ApplicationDbContext context)
        {
            return context.Organizations
                .Include(div => div.Divisions)
                    .ThenInclude(pos => pos.Positions)
                .FirstOrDefault(i => i.Id == division.OrganizationId);
        }

        /// <summary>
        /// Метод возвращает все должности подразделения
        /// </summary>
        /// <param name="division"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<Position> GetPositions(this Division division, ApplicationDbContext context)
            => context.Positions.Where(divId => divId.DivisionId == division.Id).ToList();

        /// <summary>
        /// Метод возвращает всех сотрудников подразделения
        /// </summary>
        /// <param name="division"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<Employee> GetEmployees(this Division division, ApplicationDbContext context)
            => context.Employees.Where(divId => divId.DivisionId == division.Id).ToList();
    }
}
