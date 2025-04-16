using DataBaseModel.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseModel.Context
{
    public class PostgresContext : DbContext
    {
        private static string _connectionString;
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }

        public PostgresContext()
        {
            Database.EnsureCreated();
        }

        public static void SetConnectionString(string connectionString) => _connectionString = connectionString;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(_connectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>().Property(b => b.Id).UseIdentityAlwaysColumn();
            modelBuilder.Entity<Department>().HasMany(e => e.Employees).WithOne(e => e.DepartmentField);
            modelBuilder.Entity<Employee>().Property(b => b.Id).UseIdentityAlwaysColumn();
            modelBuilder.Entity<JobTitle>().Property(b => b.Id).UseIdentityAlwaysColumn();
            modelBuilder.Entity<JobTitle>().HasMany(e => e.Employees).WithOne(e => e.JobTitleField);
        }
    }
}
