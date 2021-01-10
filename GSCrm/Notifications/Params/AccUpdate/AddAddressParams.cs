using GSCrm.Models;

namespace GSCrm.Notifications.Params.AccUpdate
{
    public class AddAddressParams : AccUpdateParams
    {
        public AccountAddress NewAccountAddress { get; set; }
        public AddAddressParams()
        {
            AccUpdateType = Auxiliary.AccUpdateType.AddAddress;
        }
    }
}
