using GSCrm.Data;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System.Linq;

namespace GSCrm.DataTransformers
{
    public class DivisionTransformer : BaseTransformer<Division, DivisionViewModel>
    {
        public DivisionTransformer(ApplicationDbContext context, ResManager resManager) : base(context, resManager) { }

        public override Division OnModelCreate(DivisionViewModel divViewModel)
        {
            Division parentDivision = context.Divisions.FirstOrDefault(n => n.Name == divViewModel.ParentDivisionName);
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
            Division parentDivision = context.Divisions.FirstOrDefault(i => i.Id == division.ParentDivisionId);
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
