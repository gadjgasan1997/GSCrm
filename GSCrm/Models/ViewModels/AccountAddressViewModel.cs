namespace GSCrm.Models.ViewModels
{
    public class AccountAddressViewModel : BaseViewModel
    {
        public string AccountId { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string FullAddress { get; set; }
        public string AddressType { get; set; }
        public string NewLegalAddressId { get; set; }
        public string CurrentAddressNewType { get; set; }
    }
}
