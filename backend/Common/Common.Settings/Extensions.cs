using Common.Logging;
using Common.Shared.Localize;
using Common.Shared.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using OpenApiSecurityScheme = NSwag.OpenApiSecurityScheme;

namespace Common.Settings
{
    public static class Extensions
    {
        public static readonly string CORS_POLICY = "MILISALE";

        public static IServiceCollection AddCommonConfiguration(this IServiceCollection services, string serviceName, IConfiguration configuration)
        {
            services.AddOpenApiDocument(configure =>
            {
                configure.Title = $"{serviceName} API";

                configure.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}.",
                });

                configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });

            var validDomains = configuration.GetSection("ValidDomains").GetChildren().Select(x => x.Value).ToArray();

            services.AddCors(options =>
            {
                options.AddPolicy(name: CORS_POLICY,
                                  builder =>
                                  {
                                      builder.WithOrigins(validDomains)
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowAnyOrigin();
                                  });
            });
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("vi-VN")
                };

                options.DefaultRequestCulture = new RequestCulture(culture: "vi-VN", uiCulture: "vi-VN");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            services.AddSingleton<ILocalizationParser, LocalizationParser>();

            services.AddHttpContextAccessor();
            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<ICommonFunctionService, CommonFunctionService>();

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            var fileHostingDomain = configuration.GetSection("FileHostingService").Value;
            services.AddSingleton<IFileHostingService, FileHostingService>(c => new FileHostingService(fileHostingDomain));
            return services;
        }

        public static IServiceCollection AddGatewayCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            var validDomains = configuration.GetSection("ValidDomains").GetChildren().Select(x => x.Value).ToArray();

            services.AddCors(options =>
            {
                options.AddPolicy(name: CORS_POLICY,
                                  builder =>
                                  {
                                      builder.WithOrigins(validDomains)
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowAnyOrigin();
                                  });
            });

            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var validIdentityUrl = configuration.GetSection("IdentityUrl").Value;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.Authority = validIdentityUrl;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = false
                        };
                    });

            return services;
        }

        public static IServiceCollection AddConfigureAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            string secret = configuration["JWTTokenConfig:Secret"];
            var environment = configuration.GetSection("Environment").Value;

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ValidateIssuer = true,
                    ValidateAudience = true
                };
            });
            
            return services;
        }

        public static IServiceCollection ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            //var jwtTokenConfig = configuration.GetSection("JWTTokenConfig").Get<JwtTokenConfig>();
            string secret = configuration["JWTTokenConfig:Secret"];
            string issuer = configuration["JWTTokenConfig:Issuer"];
            string audience = configuration["JWTTokenConfig:Audience"];
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
                    ValidIssuer = issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
                    ValidAudience = audience,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
            });
            return services;
        }

        public static IApplicationBuilder UseCommonConfiguration(this IApplicationBuilder app, string serviceName, IConfiguration configuration)
        {
            var environment = configuration.GetSection("Environment").Value;

            if (!environment.Equals("Production"))
            {
                app.UseMiddleware<RequestResponseLoggingMiddleware>();
                app.UseOpenApi();
                app.UseSwaggerUi3();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{serviceName} API V1");
                });
            }

            var locationOption = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locationOption.Value);

            return app;
        }

        public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
        {
            app.UseCors(CORS_POLICY);

            return app;
        }

        public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder app)
        {
            app.UseCors(CORS_POLICY);
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
