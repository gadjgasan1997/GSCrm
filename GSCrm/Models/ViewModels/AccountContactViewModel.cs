using System;

namespace GSCrm.Models.ViewModels
{
    public class AccountContactViewModel : BaseViewModel
    {
        public string AccountId { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string ContactType { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsPrimary { get; set; }
    }
}
