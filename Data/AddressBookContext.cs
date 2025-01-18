using DotnetAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data
{
    public class AddressBookContext : DbContext
    {
        public AddressBookContext(DbContextOptions<AddressBookContext> options) : base(options) { }

        public DbSet<AddressBookEntry> AddressBookEntries { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Department> Departments { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    // Configure Job-AddressBookEntry relationship
        //    modelBuilder.Entity<AddressBookEntry>()
        //        .HasOne(entry => entry.Job)
        //        .WithMany(job => job.AddressBookEntries)
        //        .HasForeignKey(entry => entry.JobId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    // Configure Department-AddressBookEntry relationship
        //    modelBuilder.Entity<AddressBookEntry>()
        //        .HasOne(entry => entry.Department)
        //        .WithMany(department => department.AddressBookEntries)
        //        .HasForeignKey(entry => entry.DepartmentId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    base.OnModelCreating(modelBuilder);
        //}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure AddressBookEntry-User relationship
            modelBuilder.Entity<AddressBookEntry>()
                .HasOne(entry => entry.User)
                .WithMany(user => user.AddressBookEntries)
                .HasForeignKey(entry => entry.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Job-User relationship
            modelBuilder.Entity<Job>()
                .HasOne(job => job.User)
                .WithMany(user => user.Jobs)
                .HasForeignKey(job => job.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Department-User relationship
            modelBuilder.Entity<Department>()
                .HasOne(department => department.User)
                .WithMany(user => user.Departments)
                .HasForeignKey(department => department.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }

    }
}
