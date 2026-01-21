using CvWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CvWebApp.Context
{
    public class MainDBContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public DbSet<Note> Notes { get; set; }

        public MainDBContext() { }

        public MainDBContext(DbContextOptions<MainDBContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Note>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();
            });
        }

    }
}
