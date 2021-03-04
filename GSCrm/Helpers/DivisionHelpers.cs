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
        public static Organization GetOrganization(this Division division, ApplicationDbContext context)
            => context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == division.OrganizationId);
        public static List<Position> GetPositions(this Division division, ApplicationDbContext context)
            => context.Positions.AsNoTracking().Where(divId => divId.DivisionId == division.Id).ToList();
        public static List<Employee> GetEmployees(this Division division, ApplicationDbContext context)
            => context.Employees.AsNoTracking().Where(divId => divId.DivisionId == division.Id).ToList();

        public static void Normalize(this DivisionViewModel divisionViewModel)
        {
            divisionViewModel.Name = divisionViewModel.Name?.TrimStartAndEnd();
            divisionViewModel.ParentDivisionName = divisionViewModel.ParentDivisionName?.TrimStartAndEnd();
        }
    }
}
