using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Store.Date.Context;
using Store.Date.Entities.IdentityEntity;
using Store.Repository;

namespace Store.Web.Helper
{
    public class ApplySeeding
    {
        public static async Task ApplySeedingAsync(WebApplication app) 
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var DB = services.GetRequiredService<StoreDbContext>();
                    var userManger = services.GetRequiredService<UserManager<AppUser>>();
                    await DB.Database.MigrateAsync();
                    await StoreContextSeed.SeedAsync(DB, loggerFactory);
                    await StoreIdentityContextSeed.SeedUserAsync(userManger);
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<ApplySeeding>();
                    logger.LogError(ex.Message);
                }
            }
        }
    }
}
