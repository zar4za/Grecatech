namespace Grecatech.Steam.Clients
{
    public interface IMarketClient
    {
        public Task<decimal> GetActiveBalanceAsync();
        public Task<decimal> GetItemPriceAsync(string marketHashName);
        public Task<long?> BuyItemAsync(string marketHashName, decimal price);
        public Task<bool> SellItemAsync(string marketHashName);

    }
}
