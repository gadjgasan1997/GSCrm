using GSCrm.Models;

namespace GSCrm.Notifications.Params.AccUpdate
{
    public class AddContactParams : AccUpdateParams
    {
        public AccountContact NewAccountContact { get; set; }
        public AddContactParams()
        {
            AccUpdateType = Auxiliary.AccUpdateType.AddContact;
        }
    }
}
