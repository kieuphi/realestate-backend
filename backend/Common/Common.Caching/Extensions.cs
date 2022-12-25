using Common.Caching;
using Common.Caching.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Caching
{
    public static class Extensions
    {
        public static IServiceCollection AddCache(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheBase, CacheMemoryService>();

            return services;
        }
    }
}
