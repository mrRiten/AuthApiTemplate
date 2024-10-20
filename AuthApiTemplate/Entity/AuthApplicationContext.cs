using Microsoft.EntityFrameworkCore;

namespace AuthApiTemplate.Entity
{
    public class AuthApplicationContext(DbContextOptions<AuthApplicationContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}
