using Microsoft.EntityFrameworkCore;
using JWT_Authentication1_Core_.Models;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;


namespace JWT_Authentication1_Core_.Contexts
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Refresh> Refresh { get; set; }
    }
}
