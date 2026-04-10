using Microsoft.EntityFrameworkCore;
using kitucxa.Models;

namespace kitucxa.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Student { get; set; }
        public DbSet<Room> Room { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Room)
                .WithMany(r => r.Students)
                .HasForeignKey(s => s.RoomId);
        }
    }
}