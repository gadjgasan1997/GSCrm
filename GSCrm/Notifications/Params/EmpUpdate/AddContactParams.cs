using GSCrm.Models;

namespace GSCrm.Notifications.Params.EmpUpdate
{
    public class AddContactParams : EmpUpdateParams
    {
        public EmployeeContact NewEmployeeContact { get; set; }
        public AddContactParams()
        {
            EmpUpdateType = Auxiliary.EmpUpdateType.AddContact;
        }
    }
}
