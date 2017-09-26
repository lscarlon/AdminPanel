using Microsoft.AspNetCore.Identity;

namespace AdminPanel.Identity
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
    }
}
