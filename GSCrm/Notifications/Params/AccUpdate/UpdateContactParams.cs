using System;
using GSCrm.Models;

namespace GSCrm.Notifications.Params.AccUpdate
{
    public class UpdateContactParams : AccUpdateParams
    {
        public AccountContact OldAccountContact { get; set; }
        public AccountContact NewAccountContact { get; set; }
        public UpdateContactParams()
        {
            AccUpdateType = Auxiliary.AccUpdateType.UpdateContact;
        }
    }
}
