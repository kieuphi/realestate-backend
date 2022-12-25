### How to use

1. Define cache key in Caching Project or your project

2. Add `services.AddCache();` into the DI of your project to init In-Memory Cache

3. Inject `private readonly ICacheBase _cache` into class that you want to use in-memory cache

4. Example to get/set value from cache 

            var purchaseOrders = _cache.TryGet<List<PurchaseOrderDto>>(ShippingOrderKeys.PurchaseOrders);

            if (purchaseOrders == null)
            {
                purchaseOrders = await _db.PurchaseOrders
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .ProjectTo<PurchaseOrderDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                _cache.Add(purchaseOrders, ShippingOrderKeys.PurchaseOrders);
            }

            return purchaseOrders;

FYI: 

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(absoluteExpiration)) <--- Time that value will be deleted when hit the line
                        .SetPriority(priority)                                           <--- The Priority of cache value when setter
                        .SetSlidingExpiration(TimeSpan.FromSeconds(slidingExpiration));  <--- Time that will keep the value, auto reset time if the value is set. Remove when hit the line