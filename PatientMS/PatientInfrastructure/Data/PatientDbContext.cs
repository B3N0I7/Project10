using Microsoft.EntityFrameworkCore;
using PatientMS.PatientCore.Models;

namespace PatientMS.PatientInfrastructure.Data
{
    public class PatientDbContext : DbContext
    {
        public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Patient>();
        }

        public DbSet<Patient> Patients { get; set; }
    }
}
