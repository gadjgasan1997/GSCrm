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
                .AsNoTracking()
                .Include(div => div.Divisions)
                    .ThenInclude(pos => pos.Positions)
                .FirstOrDefault(i => i.Id == division.OrganizationId);
        }
        public static List<Position> GetPositions(this Division division, ApplicationDbContext context)
            => context.Positions.AsNoTracking().Where(divId => divId.DivisionId == division.Id).ToList();
        public static List<Employee> GetEmployees(this Division division, ApplicationDbContext context)
            => context.Employees.AsNoTracking().Where(divId => divId.DivisionId == division.Id).ToList();
    }
}
