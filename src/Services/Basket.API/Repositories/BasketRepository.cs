using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Contracts.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using ILogger = Serilog.ILogger;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _cache;
        private readonly ISerializeService _serializeService;
        private readonly ILogger _logger;

        public BasketRepository(IDistributedCache cache, ISerializeService serializeService, ILogger logger)
        {
            _cache = cache;
            _serializeService = serializeService;
            _logger = logger;
        }

        public async Task<Cart?> GetBasketByUserName(string userName)
        {
            _logger.Information($"BEGIN: GetBasketByUserName {userName}");
            var basket = await _cache.GetStringAsync(userName);
            _logger.Information($"END: GetBasketByUserName {userName}");

            return string.IsNullOrEmpty(basket) ? null : _serializeService.Deserialize<Cart>(basket);
        }

        public async Task<Cart> UpdateBasket(Cart cart, DistributedCacheEntryOptions options = null)
        {
            _logger.Information($"BEGIN: UpdateBasket for {cart.UserName}");
            if (options != null)
                await _cache.SetStringAsync(cart.UserName, _serializeService.Serialize(cart), options);
            else
                await _cache.SetStringAsync(cart.UserName, _serializeService.Serialize(cart));
            _logger.Information($"END: UpdateBasket for {cart.UserName}");

            return await GetBasketByUserName(cart.UserName);
        }

        public async Task<bool> DeleteBasketFromUserName(string userName)
        {
            try
            {
                _logger.Information($"BEGIN: DeleteBasketFromUserName {userName}");
                await _cache.RemoveAsync(userName);
                _logger.Information($"BEGIN: DeleteBasketFromUserName {userName}");

                return true;
            }
            catch (Exception e)
            {
                _logger.Error("Error DeleteBasketFromUserName: " + e.Message);
                throw;
            }
        }
    }
}
