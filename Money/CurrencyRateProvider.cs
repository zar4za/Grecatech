using Newtonsoft.Json.Linq;

namespace Grecatech.Money
{
    public class CurrencyRateProvider
    {
        public CurrencyRateProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private HttpClient _httpClient;

        public async Task<decimal> GetUsdToCny()
        {
            var url = new Uri("https://www.chinamoney.com.cn/r/cms/www/chinamoney/data/fx/ccpr.json");
            var response = await _httpClient.GetStringAsync(url);
            var json = JObject.Parse(response);

            return json["records"][0]["price"].Value<decimal>();
        }
    }
}
