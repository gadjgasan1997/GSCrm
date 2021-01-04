using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace GSCrm.Models
{
    public class UserOrganization : BaseDataModel
    {
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("Organization")]
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public bool Accepted { get; set; } = true;

        public OrgNotificationsSetting OrgNotificationsSetting { get; set; }
    }

    public class UserOrganizationEqualityComparer : IEqualityComparer<UserOrganization>
    {
        public bool Equals([AllowNull] UserOrganization firstOrg, [AllowNull] UserOrganization secondOrg) => firstOrg.Id == secondOrg.Id;

        public int GetHashCode([DisallowNull] UserOrganization userOrganization) => userOrganization.Id.GetHashCode();
    }
}
