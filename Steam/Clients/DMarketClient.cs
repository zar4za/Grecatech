using Chaos.NaCl;
using Grecatech.Steam.Clients.Models;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Grecatech.Steam.Clients
{
    internal class DMarketClient // : IMarketClient
    {
        private const string RootUrl = "https://api.dmarket.com";

        private readonly string _publicKey;
        private readonly string _secretKey;

        public DMarketClient(string publicKey, string secretKey)
        {
            _publicKey = publicKey;
            _secretKey = secretKey;
        }

        public async Task<decimal> GetBalanceAsync()
        {
            var query = "/account/v1/balance";
            var client = new HttpClient();
            AddApiHeaders(client, HttpMethod.Get, query, string.Empty);
            var response = await client.GetAsync(RootUrl + query);
            if (response.StatusCode != HttpStatusCode.OK)
                return decimal.Zero;

            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
            return json.Value<int>("usd") / 100m;
        }

        public Task<BuffPage> GetItemsAsync(int page, decimal maxPriceCny, string quality)
        {
            throw new NotImplementedException();
        }

        public async Task<Dictionary<string, decimal>> GetPricesAsync(Item[] items)
        {
            var queryBuilder = new StringBuilder("/price-aggregator/v1/aggregated-prices?");
            foreach (var item in items)
            {
                queryBuilder.Append($"Titles={HttpUtility.UrlEncode(item.MarketHashName)}&");
            }

            string query = queryBuilder.ToString();
            var client = new HttpClient();
            AddApiHeaders(client, HttpMethod.Get, query, string.Empty);
            var response = await client.GetAsync(RootUrl + query);
            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());

            var prices = new Dictionary<string, decimal>();
            foreach (var item in (JArray)json["AggregatedTitles"])
            {
                prices.Add(item["MarketHashName"].ToString(), item["Offers"]["BestPrice"].Value<decimal>());
            }
            return prices;
        }

        private void AddApiHeaders(HttpClient client, HttpMethod method, string query, string body)
        {
            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            client.DefaultRequestHeaders.Add("X-Api-Key", _publicKey);
            client.DefaultRequestHeaders.Add("X-Sign-Date", timestamp.ToString());
            client.DefaultRequestHeaders.Add("X-Request-Sign", Sign(method, query, body, timestamp));
        }

        private string Sign(HttpMethod method, string query, string body, long timestamp)
        {
            var signaturePrefix = "dmar ed25519 ";
            string stringToSign = $"{method.Method.ToUpperInvariant()}{query}{body}{timestamp}";
            byte[] targetBytes = Encoding.UTF8.GetBytes(stringToSign);
            byte[] secretBytes = Convert.FromHexString(_secretKey);
            byte[] signatureBytes = Ed25519.Sign(targetBytes, secretBytes);
            return signaturePrefix + BitConverter.ToString(signatureBytes).Replace("-", "");
        }
    }
}