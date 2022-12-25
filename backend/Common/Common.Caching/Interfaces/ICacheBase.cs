using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace Common.Caching.Interfaces
{
    public interface ICacheBase
    {
        T Get<T>(string key);
        T TryGet<T>(string key);
        List<T> GetList<T>();
        List<T> GetList<T>(string key);
        void Add<T>(T o, string key, int slidingExpiration = 300, int absoluteExpiration = 300, CacheItemPriority priority = CacheItemPriority.Normal);
        void AddList<T>(List<T> listData, int slidingExpiration = 60, int absoluteExpiration = 60, CacheItemPriority priority = CacheItemPriority.Normal);
        void AddList<T>(List<T> listData, string key, int slidingExpiration = 10, int absoluteExpiration = 60, CacheItemPriority priority = CacheItemPriority.Normal);
        void Remove(string key);
    }
}
