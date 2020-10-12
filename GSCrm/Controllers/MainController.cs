using GSCrm.Data;
using GSCrm.Data.ApplicationInfo;
using GSCrm.DataTransformers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using GSCrm.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GSCrm.Controllers
{
    public class MainController<TDataModel, TViewModel, TValidator, TTransformer, TRepository> : Controller
        where TDataModel : BaseDataModel, new()
        where TViewModel : BaseViewModel, new()
        where TValidator : BaseValidator<TViewModel>
        where TTransformer : BaseTransformer<TDataModel, TViewModel>
        where TRepository : GenericRepository<TDataModel, TViewModel, TValidator, TTransformer>
    {
        protected readonly ApplicationDbContext context;
        protected readonly IViewsInfo viewsInfo;
        protected readonly ResManager resManager;
        protected readonly TTransformer transformer;
        protected TRepository repository;
        public MainController(ApplicationDbContext context, IViewsInfo viewsInfo, ResManager resManager, TTransformer transformer, TRepository repository)
        {
            this.context = context;
            this.viewsInfo = viewsInfo;
            this.resManager = resManager;
            this.transformer = transformer;
            this.repository = repository;
        }

        [HttpPost("Create")]
        public virtual IActionResult Create(TViewModel viewModel)
        {
            ModelStateDictionary modelState = ModelState;
            if (repository.TryCreate(ref viewModel, modelState))
                return Json("");
            return BadRequest(modelState);
        }

        [HttpPost("Update")]
        public virtual IActionResult Update(TViewModel viewModel)
        {
            ModelStateDictionary modelState = ModelState;
            if (repository.TryUpdate(ref viewModel, modelState))
                return Json("");
            return BadRequest(modelState);
        }

        [HttpDelete("Delete")]
        public virtual IActionResult Delete(string id)
        {
            ModelStateDictionary modelState = ModelState;
            if (repository.TryDelete(id, modelState))
                return Json("");
            return BadRequest(modelState);
        }
    }
}
