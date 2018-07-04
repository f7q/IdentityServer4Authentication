using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer4Authentication.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public virtual int OfficeNumber { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
    }

    public class Group
    {
        public virtual string Id { get; set; }
        public virtual string Name { get; set; }
    }
}
