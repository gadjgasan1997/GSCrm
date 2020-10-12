using GSCrm.Data;
using GSCrm.Data.ApplicationInfo;
using GSCrm.DataTransformers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Validators;

namespace GSCrm.Repository
{
    public class EmployeeContactRepository : GenericRepository<EmployeeContact, EmployeeContactViewModel, EmployeeContactValidator, EmployeeContactTransformer>
    {
        public EmployeeContactRepository(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager)
            : base(context, viewsInfo, resManager, new EmployeeContactValidator(context, resManager), new EmployeeContactTransformer(context, resManager))
        { }
    }
}
