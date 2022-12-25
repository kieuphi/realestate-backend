using System;
using System.IO;
using System.Threading.Tasks;
using Common.Logging;
using Common.Shared.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using General.Domain.Common;
using General.Infrastructure.Identity;
using General.Infrastructure.Persistence;

namespace General.Api
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();

                    if (context.Database.IsSqlServer())
                    {
                        context.Database.Migrate();
                    }

                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var clientFileFactory = services.GetRequiredService<IClientFileFactoryService>();
                    var accessManagementApiClient = clientFileFactory.GetAccessManagementApiClient();

                    await ApplicationDbContextSeed.SeedDefaultRolesAsync(roleManager);
                    await ApplicationDbContextSeed.SeedDefaultUserAsync(userManager, context);
                    await ApplicationDbContextSeed.SeedAttachmentTypesAsync(context);
                    await ApplicationDbContextSeed.SeedImageCategoryAsync(context);
                    await ApplicationDbContextSeed.SeedFolder(context);
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating or seeding the database.");

                    throw;
                }
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog(SeriLogger.Configure)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseIISIntegration()
                    .UseWebRoot("wwwroot")
                    .UseStartup<Startup>();
                });
    }
}
