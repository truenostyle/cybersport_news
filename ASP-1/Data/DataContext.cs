using Microsoft.EntityFrameworkCore;

namespace ASP_1.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Entity.User> Users { get; set; }
        public DbSet<Entity.EmailConfirmToken> EmailConfirmTokens { get; set; } 

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
        }
    }
}
