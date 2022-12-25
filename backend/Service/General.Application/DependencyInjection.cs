using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using General.Application.Common.Behaviours;
using Microsoft.Extensions.Configuration;
using Common.Shared.Microservice.CommunicationClient;
using Common.Shared.Services;

namespace General.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));

            //services.AddTransient<ICommunicationApiClient, CommunicationApiClient>(c => new CommunicationApiClient(configuration.GetSection("MicroserviceUrl").GetSection("CommunicationMicroserviceUrl").Value));
            services.AddSingleton<IClientFileFactoryService, ClientFileFactoryService>();
            return services;
        }
    }
}
