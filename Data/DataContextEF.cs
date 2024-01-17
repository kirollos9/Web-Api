
using DotnetApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DotnetAPI.Data
{
    public class DataContextEF : DbContext
    {
        
        
        private readonly IConfiguration _config;
        public DataContextEF(IConfiguration config){
            
            //  _connectionServer=config.GetConnectionString("DefaultConnection");
            _config=config;

        }
        public virtual DbSet<User>? Users { get; set; }
        public virtual DbSet<UserSalary>? UserSalary { get; set; }
        public virtual DbSet<UserJobInfo>? UserJobInfo { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // base.OnConfiguring(options);
            if (!options.IsConfigured)
            {
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection"),
                options => options.EnableRetryOnFailure());
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("TutorialAppSchema");
            modelBuilder.Entity<User>().ToTable("Users","TutorialAppSchema").HasKey(user=>user.UserId);
            modelBuilder.Entity<UserJobInfo>().HasKey(uj=>uj.UserId);
            modelBuilder.Entity<UserSalary>().HasKey(us=>us.UserId);
           // modelBuilder.Entity<Computer>().ToTable("Computer","TutorialAppSchema");
        }


    }
}