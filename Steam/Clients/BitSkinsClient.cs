using Grecatech.Steam.Clients.Models;
using Newtonsoft.Json.Linq;

namespace Grecatech.Steam.Clients 
{
    internal class BitSkinsClient : IMarketClient
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

        public BitSkinsClient(HttpClient httpClient)
        {
            _client = httpClient;
        }
        private const string RootUrl = "https://bitskins.com/api/v1";
        private HttpClient _client;
        public async Task<decimal> GetAccountBalanceAsync(string apiKey, string code)
        {
            var url = new Uri($"{RootUrl}/get_account_balance/?api_key={apiKey}&code={code}");
            string response = await _client.GetStringAsync(url);

            //  response json
            //  {
            //      available_balance: '13370.420',
            //      pending_withdrawals: '0.0000',
            //      withdrawable_balance: '13370.420',
            //      couponable_balance: '13370.420'
            //  }

            JObject balance = JObject.Parse(response);
            return Convert.ToDecimal(balance["available_balance"]);
        }
    }
}