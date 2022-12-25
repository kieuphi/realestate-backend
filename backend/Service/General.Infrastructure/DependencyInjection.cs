using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using General.Application.Interfaces;
using General.Domain.Common;
using General.Infrastructure.Identity;
using General.Infrastructure.Persistence;
using General.Infrastructure.Services;
using General.Infrastructure.Repositories;
using General.Application.Common.Interfaces;
using Files.Infrastructure.Services;

namespace General.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            
            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
            services.AddHealthChecks().AddDbContextCheck<ApplicationDbContext>();
            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));


            services.AddTransient<IDateTime, DateTimeService>();
            services.AddTransient<IIdentityService, IdentityService>();

            services.AddHostedService<JwtRefreshTokenCache>();
            services.AddSingleton<IJwtAuthManager, JwtAuthManager>();
            services.AddSingleton<ICommonFunctionService, CommonFunctionService>();
            services.AddSingleton<IConvertVietNameseService, ConvertVietNameseService>();
            services.AddSingleton<IHandlePropertyService, HandlePropertyService>();
            services.AddSingleton<IHandleProjectService, HandleProjectService>();
            services.AddSingleton<IRolesService, RolesService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IExportTemplateService, ExportTemplateService>();

            string verifyAccountUrl = configuration.GetSection("VerifyAccountUrl").Value;
            services.AddSingleton<ISettingValueService, SettingValueService>(c => new SettingValueService(verifyAccountUrl));

            var resetPasswordUrl = configuration.GetSection("ResetPasswordUrl").Value;
            var resetPasswordInternalUserUrl = configuration.GetSection("ResetPasswordInternalUserUrl").Value;
            services.AddSingleton<IResetPasswordService, ResetPasswordService>(c => new ResetPasswordService(resetPasswordUrl, resetPasswordInternalUserUrl));

            services.AddTransient<IUploadFileService, UploadFileService>();

            return services;
        }
    }
}
