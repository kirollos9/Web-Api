
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
            modelBuilder.HasDefaultSchema("dbo.");
            modelBuilder.Entity<User>().ToTable("Users","dbo.").HasKey(user=>user.UserId);
     
           // modelBuilder.Entity<Computer>().ToTable("Computer","dbo.");
        }


    }
}