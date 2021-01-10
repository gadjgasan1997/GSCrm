using GSCrm.Models;

namespace GSCrm.Notifications.Params.AccUpdate
{
    public class UpdateAddressParams : AccUpdateParams
    {
        public AccountAddress OldAccountAddress { get; set; }
        public AccountAddress NewAccountAddress { get; set; }
        public UpdateAddressParams()
        {
            AccUpdateType = Auxiliary.AccUpdateType.UpdateAddress;
        }
    }
}
