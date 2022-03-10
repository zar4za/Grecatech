using Newtonsoft.Json.Linq;

namespace Grecatech.Steam.Clients
{
    public class Buff163Client : IMarketClient
    {
        private const int PointDigits = 4;
        private const string RootUrl = "https://buff.163.com/api";
        private readonly string _session;
        private HttpClient _httpClient;
        private static long Nonce => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        private Dictionary<string, int> _itemIds;

        public Buff163Client(HttpClient client, string session)
        {
            _session = session;
            _httpClient = client;

            //Cookie Authorization
            var cookieUrl = new Uri("https://buff.163.com/");
            var request = new HttpRequestMessage(HttpMethod.Get, cookieUrl);
            request.Headers.Add("Cookie", $"session={_session}");
            _httpClient.SendAsync(request);

            _itemIds = JObject.Parse(File.ReadAllText("idsb.json")).ToObject<Dictionary<string, int>>();
        }

        public readonly decimal Fees = 0.975m;

        public async Task<decimal> GetActiveBalanceAsync()
        {
            var url = new Uri($"{RootUrl}/asset/get_brief_asset/?_={Nonce}");
            var response = await _httpClient.GetStringAsync(url);
            var json = JObject.Parse(response);

            if(!json.ContainsKey("code") || json["code"].Value<string>() != "OK")
                return decimal.Zero;

            var cny = json["data"]["alipay_amount"].Value<decimal>();

            return await ConvertToUsd(cny);
        }

        public async Task<decimal> GetItemPriceAsync(string marketHashName)
        {
            if (!_itemIds.ContainsKey(marketHashName))
                return decimal.Zero;

            var id = _itemIds[marketHashName];
            var url = new Uri($"{RootUrl}/market/goods/sell_order?game=csgo&goods_id={id}&page_num=1&sort_by=price.asc&mode=&allow_tradable_cooldown=0&_={Nonce}");
            var response = await _httpClient.GetStringAsync(url);
            var json = JObject.Parse(response);

            if (!json.ContainsKey("code") || json["code"].Value<string>() != "OK")
                return decimal.Zero;

            var cny = json["data"]["items"][0]["price"].Value<decimal>();

            return await ConvertToUsd(cny);
        }

        public async Task<bool> BuyItemAsync(string marketHashName, decimal price)
        {
            throw new NotImplementedException();
        }

        //POST: https://buff.163.com/market/sell_order/preview/manual_plus
        public async Task<bool> SellItemAsync(string marketHashName)
        {
            throw new NotImplementedException();
        }

        public async Task<decimal> GetUsdConvertRateAsync()
        {
            var url = new Uri($"https://buff.163.com/account/api/user/info?_={Nonce}");
            var response = await _httpClient.GetStringAsync(url);
            var json = JObject.Parse(response);

            if (!json.ContainsKey("code") || json["code"].Value<string>() != "OK")
                return decimal.Zero;

            var rate = json["data"]["buff_price_currency_rate_base_cny"].Value<decimal>();
            return rate;
        }

        private async Task<decimal> ConvertToUsd(decimal cny)
        {
            var rate = await GetUsdConvertRateAsync();
            return Math.Round(cny * rate, PointDigits);
        }
    }
}