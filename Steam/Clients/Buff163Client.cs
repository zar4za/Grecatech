using Grecatech.Steam.Clients.Models;
using Newtonsoft.Json.Linq;
using System.Net;

namespace Grecatech.Steam.Clients
{
    public class Buff163Client : IMarketClient
    {
        private const string RootUrl = "https://buff.163.com/api";
        private readonly string _session;
        private HttpClient _httpClient;
        private long _nonce => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public Buff163Client(HttpClient client, string session)
        {
            _session = session;
            _httpClient = client;

            //Cookie Authorization
            var cookieUrl = new Uri("https://buff.163.com/");
            var request = new HttpRequestMessage(HttpMethod.Get, cookieUrl);
            request.Headers.Add("Cookie", $"session={_session}");
            _httpClient.Send(request);
        }

        public async Task<decimal> GetActiveBalanceAsync()
        {
            var url = new Uri($"{RootUrl}/asset/get_brief_asset/?_={_nonce}");
            var response = await _httpClient.GetStringAsync(url);
            var json = JObject.Parse(response);

            if (json.ContainsKey("code") && json["code"].Value<string>() == "OK")
            {
                var cny = json["data"]["alipay_amount"].Value<decimal>();
                var provider = new Money.CurrencyRateProvider(_httpClient);
                var rate = await provider.GetUsdToCny();

                return cny / rate;
            }
            return decimal.Zero;
        }

        public Task<decimal> GetItemPriceAsync(string marketHashName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BuyItemAsync(string marketHashName, decimal price)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SellItemAsync(string marketHashName)
        {
            throw new NotImplementedException();
        }
    }

}