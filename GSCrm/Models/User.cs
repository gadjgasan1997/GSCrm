using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace GSCrm.Models
{
    public class User : IdentityUser
    {
        public Guid PrimaryOrganizationId { get; set; }
        public LangType DefaultLanguage { get; set; }
        public string AvatarPath { get; set; }

        public List<UserOrganization> UserOrganizations { get; set; }
        public User()
        {
            UserOrganizations = new List<UserOrganization>();
        }
    }
}
