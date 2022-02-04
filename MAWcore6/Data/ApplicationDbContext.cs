using Duende.IdentityServer.EntityFramework.Options;
using MAWcore6.Models;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MAWcore6.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {
        }
        public DbSet<City> Cities { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<UserItems> UserItems { get; set; }
        public DbSet<UserResearch> UserResearch { get; set; }

        //Overide if you don't wan't table name to be "Cities"
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<City>().ToTable("City");
        //    modelBuilder.Entity<Building>().ToTable("Building");
        //}

    }
}