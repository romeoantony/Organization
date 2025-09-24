using Microsoft.EntityFrameworkCore;

namespace Organization.Models
{
    public class OrganizationDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=organization.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map decimal Salary to REAL for better SQLite compatibility
            modelBuilder.Entity<Employee>()
                .Property(e => e.Salary)
                .HasConversion<double>()
                .HasColumnType("REAL");
        }
    }
}
