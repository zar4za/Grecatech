using Grecatech.Steam.Clients.Models;

namespace Grecatech.Steam.Clients 
{
    internal class BitSkinsClient : IMarketClient
    {
        public Task<decimal> GetBalanceAsync()
        {
            throw new NotImplementedException();
        }

        public Task<BuffPage> GetItemsAsync(int page, decimal maxPriceCny, string quality)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, decimal>> GetPricesAsync(Item[] items)
        {
            throw new NotImplementedException();
        }
    }
}