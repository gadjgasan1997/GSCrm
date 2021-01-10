using GSCrm.Models;

namespace GSCrm.Notifications.Params.EmpUpdate
{
    public class UpdateContactParams : EmpUpdateParams
    {
        public EmployeeContact OldEmployeeContact { get; set; }
        public EmployeeContact NewEmployeeContact { get; set; }
        public UpdateContactParams()
        {
            EmpUpdateType = Auxiliary.EmpUpdateType.UpdateContact;
        }
    }
}
