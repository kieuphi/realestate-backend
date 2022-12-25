using Common.Caching.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Common.Caching
{
    public class CacheMemoryService : ICacheBase
    {
        private readonly IMemoryCache _cache;
        private readonly bool IsEnableCache = false;

        public CacheMemoryService(IMemoryCache cache, IConfiguration configuration)
        {
            _cache = cache;
            IsEnableCache = configuration == null ? false : configuration.GetValue<bool>("UseInMemoryCache");
        }

        public void Add<T>(T o, string key, 
            int slidingExpiration = 60, 
            int absoluteExpiration = 60,
            CacheItemPriority priority = CacheItemPriority.Normal)
        {
            if (IsEnableCache)
            {
                T cacheEntry;

                if (!_cache.TryGetValue(key, out _))
                {
                    cacheEntry = o;

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(absoluteExpiration))
                        .SetPriority(priority)
                        .SetSlidingExpiration(TimeSpan.FromSeconds(slidingExpiration));

                    _cache.Set(key, cacheEntry, cacheEntryOptions);
                }
            }
        }
        public void AddList<T>(List<T> listData, int slidingExpiration = 10, int absoluteExpiration = 30, CacheItemPriority priority = CacheItemPriority.Normal)
        {
            if (IsEnableCache)
            {
                string key = typeof(T).Name;
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetPriority(priority)
                    .SetSlidingExpiration(TimeSpan.FromMinutes(slidingExpiration));
                if (absoluteExpiration > 0)
                {
                    cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(absoluteExpiration));
                }

                _cache.Set(key, listData, cacheEntryOptions);
            }
        }
        public void AddList<T>(List<T> listData,string key, int slidingExpiration = 10, int absoluteExpiration = 30, CacheItemPriority priority = CacheItemPriority.Normal)
        {
            if (IsEnableCache)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetPriority(priority)
                    .SetSlidingExpiration(TimeSpan.FromMinutes(slidingExpiration));
                if (absoluteExpiration > 0)
                {
                    cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(absoluteExpiration));
                }

                _cache.Set(key, listData, cacheEntryOptions);
            }
        }

        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public List<T> GetList<T>()
        {
            string key = typeof(T).Name;
            List<T> listData = null;
            _cache.TryGetValue<List<T>>(key, out listData);
            return listData;
        }
        public List<T> GetList<T>(string key)
        {
            List<T> listData = null;
            _cache.TryGetValue<List<T>>(key, out listData);
            return listData;
        }

        public T TryGet<T>(string key)
        {
            if (!_cache.TryGetValue<T>(key, out T cacheEntry))
            {
                return cacheEntry;
            }

            return cacheEntry;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
