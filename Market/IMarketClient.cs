namespace Grecatech.Market
{
    public interface IMarketClient
    {
        public Task<decimal> GetActiveBalanceAsync();
        public Task<decimal> GetItemPriceAsync(string marketHashName);
        public Task<string?> BuyItemAsync(string marketHashName, decimal price);
        public Task<string?> SellItemAsync(string marketHashName, decimal price);

    }
}
