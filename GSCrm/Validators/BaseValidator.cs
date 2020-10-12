using GSCrm.Data;
using GSCrm.Localization;
using GSCrm.Models.ViewModels;
using System;
using System.Collections.Generic;

namespace GSCrm.Validators
{
    public abstract class BaseValidator<TViewModel>
        where TViewModel : BaseViewModel
    {
        protected ApplicationDbContext context;
        protected ResManager resManager;
        public Dictionary<string, string> errors;
        public BaseValidator(ApplicationDbContext context, ResManager resManager)
        {
            this.context = context;
            this.resManager = resManager;
            errors = new Dictionary<string, string>();
        }

        /// <summary>
        /// Проверка модели представления перед созданием
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public virtual Dictionary<string, string> CreationCheck(TViewModel viewModel) => errors;

        /// <summary>
        /// Проверка модели представления перед обновлением
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public virtual Dictionary<string, string> UpdateCheck(TViewModel viewModel) => errors;

        /// <summary>
        /// Проверка модели представления перед удалением
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public virtual Dictionary<string, string> DeleteCheck(Guid id) => errors;
    }
}
