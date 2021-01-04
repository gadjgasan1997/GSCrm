using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using System.Linq;
using GSCrm.Data;
using Microsoft.EntityFrameworkCore;

namespace GSCrm.Mapping
{
    public class DivisionMap : BaseMap<Division, DivisionViewModel>
    {
        public DivisionMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        public override Division OnModelCreate(DivisionViewModel divViewModel)
        {
            base.OnModelCreate(divViewModel);
            Division parentDivision = (Division)transaction.GetParameterValue("ParentDivision");
            return new Division()
            {
                Id = divViewModel.Id,
                OrganizationId = divViewModel.OrganizationId,
                Name = divViewModel.Name,
                ParentDivisionId = parentDivision?.Id
            };
        }

        public override DivisionViewModel DataToViewModel(Division division)
        {
            Division parentDivision = context.Divisions.AsNoTracking().FirstOrDefault(i => i.Id == division.ParentDivisionId);
            return new DivisionViewModel()
            {
                Id = division.Id,
                OrganizationId = division.OrganizationId,
                Name = division.Name,
                ParentDivisionId = parentDivision?.Id,
                ParentDivisionName = parentDivision?.Name
            };
        }
    }
}
