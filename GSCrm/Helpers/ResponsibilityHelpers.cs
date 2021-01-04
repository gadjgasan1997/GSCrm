using GSCrm.Data;
using GSCrm.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GSCrm.Helpers
{
    public static class ResponsibilityHelpers
    {
        public static Organization GetOrganization(this Responsibility responsibility, ApplicationDbContext context)
            => context.Organizations.AsNoTracking().FirstOrDefault(org => org.Id == responsibility.OrganizationId);
    }
}
