using GSCrm.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSCrm.Models
{
    public class AccountAddress : BaseDataModel
    {
        public AddressType AddressType { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }

        [ForeignKey("Account")]
        public Guid AccountId { get; set; }
        public Account Account { get; set; }
    }
}
