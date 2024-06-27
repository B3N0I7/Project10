using Microsoft.EntityFrameworkCore;
using PatientMS.PatientInfrastructure.Data;

namespace PatientMS.PatientInfrastructure.Service
{
    public class DatabaseManagementService
    {
        public static void MigrationInitialisation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                serviceScope.ServiceProvider.GetService<PatientDbContext>().Database.Migrate();
            }
        }
    }
}
