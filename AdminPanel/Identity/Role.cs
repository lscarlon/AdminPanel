using Microsoft.AspNetCore.Identity;
using System;

namespace AdminPanel.Identity
{
    public class Role : IdentityRole
    {
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string IPAddress { get; set; }
    }
}
