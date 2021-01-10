using GSCrm.Models;

namespace GSCrm.Notifications.Params.AccUpdate
{
    public class DeleteContactParams : AccUpdateParams
    {
        public AccountContact RemovedContact { get; set; }
        public DeleteContactParams()
        {
            AccUpdateType = Auxiliary.AccUpdateType.DeleteContact;
        }
    }
}
