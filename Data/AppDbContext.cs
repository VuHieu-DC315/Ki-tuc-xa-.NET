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
        public DbSet<User> User { get; set; }
        public DbSet<StudentRoomHistory> StudentRoomHistories { get; set; }
        public DbSet<Report_Room> ReportRooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Room)
                .WithMany(r => r.Students)
                .HasForeignKey(s => s.RoomId);

            modelBuilder.Entity<Report_Room>().ToTable("reportroom");

            modelBuilder.Entity<Report_Room>()
                .HasOne(r => r.Student)
                .WithMany()
                .HasForeignKey(r => r.StudentId);

            modelBuilder.Entity<Report_Room>()
                .HasOne(r => r.OldRoom)
                .WithMany()
                .HasForeignKey(r => r.OldRoomId);

            modelBuilder.Entity<Report_Room>()
                .HasOne(r => r.NewRoom)
                .WithMany()
                .HasForeignKey(r => r.NewRoomId);
            modelBuilder.Entity<Report_Room>().ToTable("report_room");
            modelBuilder.Entity<User>().ToTable("users");
        }
    }
}