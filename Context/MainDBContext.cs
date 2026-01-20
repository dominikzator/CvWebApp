using CvWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CvWebApp.Context
{
    public class MainDBContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public MainDBContext() { }

        public MainDBContext(DbContextOptions<MainDBContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"));
        }
    }
}
