using System;

namespace GSCrm.Models.ViewModels
{
    public class BaseViewModel : IMainEntity
    {
        public Guid Id { get; set; }
    }
}
