using BlazingRoller.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace BlazingRoller.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var roomBuilder = modelBuilder.Entity<Room>();
            roomBuilder.HasKey(_ => _.Id);
            roomBuilder.Property(_ => _.RoomId).IsRequired();
            roomBuilder.Property(_ => _.RoomKey).IsRequired();
            roomBuilder.Property(_ => _.Name).IsRequired().HasMaxLength(100);
            roomBuilder.Property(_ => _.PasswordHash).IsRequired().HasMaxLength(64);
            roomBuilder.Property(_ => _.PasswordSalt).IsRequired().HasMaxLength(16);

            roomBuilder.HasIndex(_ => _.RoomKey).IsUnique();
        }
    }
}
