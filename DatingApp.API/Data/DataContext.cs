using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(prop => prop.Id);
                entity.Property(prop => prop.Id).ValueGeneratedOnAdd().IsRequired();
                entity.Property(prop => prop.Username).HasMaxLength(50).IsRequired();
                entity.Property(prop => prop.PasswordHash).IsRequired();
                entity.Property(prop => prop.PasswordSalt).IsRequired();
                entity.Property(prop => prop.Gender).HasDefaultValue(Gender.Unspecified).IsRequired();
                entity.Property(prop => prop.DateOfBirth).IsRequired();
                entity.Property(prop => prop.KnownAs).HasMaxLength(50).IsRequired();
                entity.Property(prop => prop.Created).IsRequired();
                entity.Property(prop => prop.LastActive).IsRequired();
                entity.Property(prop => prop.Introduction).HasMaxLength(500).IsRequired();
                entity.Property(prop => prop.LookingFor).HasMaxLength(500).IsRequired();
                entity.Property(prop => prop.Interests).HasMaxLength(500).IsRequired();
                entity.Property(prop => prop.City).HasMaxLength(50).IsRequired();
                entity.Property(prop => prop.Country).HasMaxLength(50).IsRequired();

                entity.HasMany<Photo>(x => x.Photos).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Photo>(entity =>
            {
                entity.HasKey(prop => prop.Id);
                entity.Property(prop => prop.Id).ValueGeneratedOnAdd().IsRequired();
                entity.Property(prop => prop.Url).HasMaxLength(250).IsRequired();
                entity.Property(prop => prop.Description).HasMaxLength(500).HasDefaultValue(string.Empty).IsRequired();
                entity.Property(prop => prop.DateAdded).IsRequired();
                entity.Property(prop => prop.IsProfilePic).HasDefaultValue(false).IsRequired(); 
                entity.Property(prop => prop.UserId).IsRequired();
            });
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
    }
}