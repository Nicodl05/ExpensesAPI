using Microsoft.EntityFrameworkCore;
using ExpensesAPI.Domain;

namespace ExpensesAPI.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuration des entités
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<Expense>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Données de départ
            modelBuilder.Entity<User>().HasData(
                new User { FirstName = "Anthony", LastName = "Stark", Currency = Currency.USD },
                new User { FirstName = "Natasha", LastName = "Romanova", Currency = Currency.RUB }
            );
        }
    }
}
