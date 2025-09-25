using Microsoft.EntityFrameworkCore;

namespace Excel.EfCoreDb
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("users"); 
                entity.HasKey(e => e.id);
            });
        }

    }
}
