using Microsoft.EntityFrameworkCore;
using MLS_UI.Data;

namespace MLS_UI.Services
{
    public static class DatabaseManagementService
    {
        public static void MigrationInitialisation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                serviceScope.ServiceProvider.GetService<UserDbContext>().Database.Migrate();
            }
        }
    }
}
