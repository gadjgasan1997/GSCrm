using GSCrm.Data;
using GSCrm.Data.ApplicationInfo;
using GSCrm.DataTransformers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Validators;

namespace GSCrm.Repository
{
    public class DivisionRepository : GenericRepository<Division, DivisionViewModel, DivisionValidatior, DivisionTransformer>
    {
        public DivisionRepository(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager)
            : base(context, viewsInfo, resManager, new DivisionValidatior(context, resManager), new DivisionTransformer(context, resManager))
        { }
    }
}
