using Grecatech.Steam.Clients.Models;
using Newtonsoft.Json.Linq;
using System.Net;

namespace Grecatech.Steam.Clients
{
    internal class Buff163Client : IMarketClient
    {
        private const string RootUrl = "https://buff.163.com/api";
        private readonly string _session;
        private HttpClient _client;
        private long _nonce => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public Buff163Client(string session)
        {
            _session = session;
            var handler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer()
            };
            handler.CookieContainer.Add(new Uri("https://buff.163.com/"), new Cookie("session", _session));
            _client = new HttpClient(handler);
        }
        public async Task<BuffPage> GetItemsAsync(int pageNum, decimal maxPrice, string quality)
        {
            var query = $"/market/goods?game=csgo&page_num={pageNum}&max_price={maxPrice}&quality={quality}&&use_suggestion=0&trigger=undefined_trigger&_={_nonce}";
            var response = await _client.GetAsync(RootUrl + query);
            return BuffPage.Parse(await response.Content.ReadAsStringAsync());
        }

        public async Task<decimal> GetBalanceAsync()
        {
            var query = $"/asset/get_brief_asset/?_={_nonce}";
            var response = await _client.GetAsync(RootUrl + query);

            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (json["code"]?.Value<string>() == "OK")
            {
                decimal cny = json["data"]["alipay_amount"].Value<decimal>();
                return cny;
            }
            return decimal.Zero;
        }

        public Task<Dictionary<string, decimal>> GetPricesAsync(Item[] items)
        {
            throw new NotImplementedException();
        }
    }

}