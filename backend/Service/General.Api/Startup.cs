using Common.Settings;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using Serilog;
using System;
using System.Linq;
using System.Text;
using General.Api.Filters;
using General.Application;
using General.Application.Interfaces;
using General.Domain.Common;
using General.Domain.Config;
using General.Infrastructure;
using General.Infrastructure.Identity;
using General.Infrastructure.Persistence;
using General.Infrastructure.Services;
using OpenApiSecurityScheme = NSwag.OpenApiSecurityScheme;
using General.Api.Configs;
using BasicLicenseLibrary;

namespace General.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration);
            services.AddApplication(Configuration);
            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            services.AddInfrastructure(Configuration);

            services.AddHttpContextAccessor();

            services.AddControllers(options =>
                options.Filters.Add<ApiExceptionFilterAttribute>())
                    .AddFluentValidation();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddCommonConfiguration(nameof(General), Configuration);
            ConfigureJWT(services);
            ConfiureIdentityServer(services);

            services.AddSingleton<IEnvironmentApplication, EnvironmentApplication>();
            //services.AddLicenseConfig();
            services.AddHostedService<ExpiredPropertyService>();
        }
        private void ConfigureJWT(IServiceCollection services)
        {
            var jwtTokenConfig = Configuration.GetSection("JWTTokenConfig").Get<JwtTokenConfig>();
            services.AddSingleton(jwtTokenConfig);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtTokenConfig.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)),
                    ValidAudience = jwtTokenConfig.Audience,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
            });
        }
        private void ConfiureIdentityServer(IServiceCollection services)
        {
            services.AddDefaultIdentity<ApplicationUser>(options => {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                //options.Password.RequiredUniqueChars = 0;

                options.SignIn.RequireConfirmedAccount = true;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(36500);
            }).AddRoles<IdentityRole>()
              .AddEntityFrameworkStores<ApplicationDbContext>();
            services.Configure<DataProtectionTokenProviderOptions>(option => option.TokenLifespan = TimeSpan.FromHours(48));

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>()
                .AddProfileService<ProfileService>();

            services.AddAuthentication()
                .AddIdentityServerJwt();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerfactory)
        {
            var environment = Configuration.GetSection("Environment").Value;

            if (!environment.Equals("Production"))
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            loggerfactory.AddSerilog();
            app.UseSerilogRequestLogging();
            //app.UseLicense();
            //string text = app.ReadLicenseFile();

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCommonConfiguration( nameof(General), Configuration);
            app.UseCustomAuthentication();

            app.UseHealthChecks("/health");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
