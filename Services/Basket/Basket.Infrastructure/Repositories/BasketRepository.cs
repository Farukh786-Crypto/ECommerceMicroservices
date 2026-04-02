using Basket.Core.Entities;
using Basket.Core.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Basket.Infrastrucyure.Repositories
{
    // Redis is used
    // Purpose Store shopping cart data
    // Why Redis Cart is temporary + needs speed
    public class BasketRepository : IBasketRepository
    {
        private IDistributedCache _redisCache;

        public BasketRepository(IDistributedCache redisCache)
        {
            _redisCache = redisCache;
        }
        //  GET cart → reads JSON from Redis → converts back to ShoppingCart
        public async Task<ShoppingCart> GetBasket(string userName)
        {
            var basket = await _redisCache.GetStringAsync(userName); // get key
            if (string.IsNullOrEmpty(basket))
            {
                return null;
            }
            // JSON string → C# object
            return JsonConvert.DeserializeObject<ShoppingCart>(basket);
        }
        //DELETE cart → removes entry from Redis
        public async Task DeleteBasket(string userName)
        {
            await _redisCache.RefreshAsync(userName);
        }
        // SAVE cart → converts ShoppingCart object to JSON → stores in Redis
        public async Task<ShoppingCart> UpsertBasket(ShoppingCart shoppingCart)
        {
            await _redisCache.SetStringAsync(shoppingCart.UserName,JsonConvert.SerializeObject(shoppingCart));
            // GET cart → reads JSON from Redis → converts back to ShoppingCart
            return await GetBasket(shoppingCart.UserName);
        }
    }
}
