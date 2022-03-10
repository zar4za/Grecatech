using Chaos.NaCl;
using Grecatech.Steam.Clients.Models;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Grecatech.Steam.Clients
{
    internal class DMarketClient : IMarketClient
    {
        private const string RootUrl = "https://api.dmarket.com";

        private readonly HttpClient _httpClient;
        private readonly string _publicKey;
        private readonly string _secretKey;

        public DMarketClient(HttpClient httpClient, string publicKey, string secretKey)
        {
            _httpClient = httpClient;
            _publicKey = publicKey;
            _secretKey = secretKey;
        }

        public async Task<decimal> GetActiveBalanceAsync()
        {
            var endPoint = "/account/v1/balance";
            var url = new Uri(RootUrl + endPoint);
            AddApiHeaders(endPoint, string.Empty, HttpMethod.Get);
            var response = await _httpClient.GetStringAsync(url);
            JObject json = JObject.Parse(response);

            return json.Value<int>("usd") / 100m;
        }

        public Task<decimal> GetItemPriceAsync(string marketHashName)
        {
            throw new NotImplementedException();
        }

        public Task<long?> BuyItemAsync(string marketHashName, decimal price)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SellItemAsync(string marketHashName)
        {
            throw new NotImplementedException();
        }

        private void AddApiHeaders(string query, string body, HttpMethod method)
        {
            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            _httpClient.DefaultRequestHeaders.Add("X-Api-Key", _publicKey);
            _httpClient.DefaultRequestHeaders.Add("X-Sign-Date", timestamp.ToString());
            _httpClient.DefaultRequestHeaders.Add("X-Request-Sign", Sign(method, query, body, timestamp));
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