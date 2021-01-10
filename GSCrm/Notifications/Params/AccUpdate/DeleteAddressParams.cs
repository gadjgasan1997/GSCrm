using GSCrm.Models;

namespace GSCrm.Notifications.Params.AccUpdate
{
    public class DeleteAddressParams : AccUpdateParams
    {
        public AccountAddress RemovedAddress { get; set; }
        public DeleteAddressParams()
        {
            AccUpdateType = Auxiliary.AccUpdateType.DeleteAddress;
        }
    }
}
