namespace GSCrm.Models
{
    public abstract class Contact : BaseDataModel
    {
        public ContactType ContactType { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
