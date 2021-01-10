﻿using GSCrm.Models;

namespace GSCrm.Notifications.Params.EmpUpdate
{
    public class ChangeDivisionParams : EmpUpdateParams
    {
        public Position NewEmployeePosition { get; set; }
        public ChangeDivisionParams()
        {
            EmpUpdateType = Auxiliary.EmpUpdateType.ChangeDivision;
        }
    }
}