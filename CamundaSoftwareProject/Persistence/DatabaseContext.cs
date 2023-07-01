using CamundaSoftwareProject.Models;
using Microsoft.EntityFrameworkCore;

namespace CamundaSoftwareProject.Persistence
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }
        
        public DbSet<User> Users {get; set;}
        public DbSet<Storage> Storage { get; set;}
    }
}
