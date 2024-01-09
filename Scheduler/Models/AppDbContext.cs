using Microsoft.EntityFrameworkCore;

namespace TMS_Training_Management_StatusChange_Scheduler
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
		public DbSet<Training>? Trainings { get; set; }
        public DbSet<Registration>? Registrations { get; set; }
    }
}
