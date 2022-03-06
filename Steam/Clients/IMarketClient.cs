using Grecatech.Steam.Clients.Models;

namespace Grecatech.Steam.Clients
{
    internal interface IMarketClient
    {
        public Task<decimal> GetBalanceAsync();
        Task<BuffPage> GetItemsAsync(int page, decimal maxPriceCny, string quality);
        Task<Dictionary<string, decimal>> GetPricesAsync(Item[] items);
    }
}
