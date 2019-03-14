using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AdminPanel.Identity;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace AdminPanel.Models
{
    public class AppDbContext : IdentityDbContext<User, Role, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Command> Commands { get; set; }
        public DbSet<Menu> Menus { get; set; }
    }

    public static class Database
    {
        public static AppDbContext dbContext = null;

        public static void InizializeDbContext(DbContextOptionsBuilder<AppDbContext> optionsBuilder)
        {
            dbContext = new AppDbContext(optionsBuilder.Options);
        }
    }
}
