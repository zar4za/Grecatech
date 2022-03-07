using Grecatech.Steam.Clients.Models;
using Newtonsoft.Json.Linq;

namespace Grecatech.Steam.Clients 
{
    public class BitSkinsClient : IMarketClient
    {
        #region IMarketClient inteface
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
        #endregion

        public BitSkinsClient(HttpClient httpClient, string apiKey)
        {
            _client = httpClient;
            _apiKey = apiKey;
        }
        private const string RootUrl = "https://bitskins.com/api/v1";
        private readonly HttpClient _client;
        private readonly string _apiKey;
        private string _twoFactorCode => throw new NotImplementedException();
        public async Task<decimal> GetAccountBalanceAsync()
        {
            var url = new Uri($"{RootUrl}/get_account_balance/?api_key={_apiKey}&code={_twoFactorCode}");
            string response = await _client.GetStringAsync(url);

            /*  RESPONSE JSON
             *  
             *  {
             *      "status": "success",
             *      "data": {
             *          "available_balance": "0.0000",
             *          "pending_withdrawals": "0.0000",
             *          "withdrawable_balance": "0.0000",
             *          "couponable_balance": "0.0000"
             *      }
             *  }
             */

            JObject balance = JObject.Parse(response);
            return Convert.ToDecimal(balance["available_balance"]);
        }

        public async void GetAllItemPricesAsync(string appId)
        {
            var url = new Uri($"{RootUrl}/get_all_item_prices/?api_key={_apiKey}&code={_twoFactorCode}&app_id={appId}");
            string response = await _client.GetStringAsync(url);
        }

        public async void GetMarketDataAsync(string appId)
        {
            var url = new Uri($"{RootUrl}/get_price_data_for_items_on_sale/?api_key={_apiKey}&code={_twoFactorCode}&app_id={appId}");
            string response = await _client.GetStringAsync(url);
        }
        public async void GetAccountInventoryAsync(string appId, int page = 1)
        {
            var url = new Uri($"{RootUrl}/get_my_inventory/?api_key={_apiKey}&code={_twoFactorCode}&page={page}&app_id={appId}");
            string response = await _client.GetStringAsync(url);
        }

        //TODO: GetInventoryOnSell


    }
}