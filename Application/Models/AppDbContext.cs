using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TMS_Traning_Management.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
		public DbSet<ApplicationUser> appUser { get; set; }
		public DbSet<Entity> Entities { get; set; }
        public DbSet<Location>? Locations { get; set; }
        public DbSet<Center>? Centers { get; set; }
        public DbSet<Training>? Trainings { get; set; }
        public DbSet<Registration>? Registrations { get; set; }
		public DbSet<UserOTP>? UserOTP { get; set; }
		public DbSet<Country>? Country { get; set; }
		public DbSet<SecurityQuestion>? SecurityQuestion { get; set; }
		protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
