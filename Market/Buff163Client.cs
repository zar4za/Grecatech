using Grecatech.Market.Models;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Grecatech.Market
{
    public class Buff163Client
    {
        public readonly decimal Fees = 0.975m;

        private const int PointDigits = 4;
        private const string RootUrl = "https://buff.163.com/api";

        private readonly string _session;
        private readonly Dictionary<string, int> _itemIds;
        private readonly HttpClient _httpClient;
        private readonly CookieCollection _cookies;

        public Buff163Client(HttpClient client, string session, string itemIdPath)
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            _session = session;
            _httpClient = client;
            _cookies = new CookieCollection()
            {
                new Cookie("session", _session)
            };

            _itemIds = JsonSerializer.Deserialize<Dictionary<string, int>>(File.ReadAllText(itemIdPath))!;
        }

        private static long Nonce => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public async Task<decimal> GetActiveBalanceAsync()
        {
            var url = new Uri($"{RootUrl}/asset/get_brief_asset/?_={Nonce}");
            var response = await _httpClient.GetStringAsync(url);
            var json = JsonNode.Parse(response)!.AsObject();

            EnsureCodeOk(json, "Не удалось получить состояние баланса.");

            var cny = json["data"]!["alipay_amount"]!.GetValue<decimal>();

            return await ConvertToUsd(cny);
        }

        public async Task<decimal> GetItemPriceAsync(string marketHashName)
        {
            if (!_itemIds.ContainsKey(marketHashName))
                return decimal.Zero;

            var id = _itemIds[marketHashName];
            var url = new Uri($"{RootUrl}/market/goods/sell_order?game=csgo&goods_id={id}&page_num=1&sort_by=price.asc&mode=&allow_tradable_cooldown=0&_={Nonce}");
            var response = await _httpClient.GetStringAsync(url);
            var json = JsonNode.Parse(response)!.AsObject();

            EnsureCodeOk(json, $"Не удалось получить цену {marketHashName}");

            var cny = json["data"]!["items"]![0]!["price"]!.GetValue<decimal>();

            return await ConvertToUsd(cny);
        }

        public async Task<string?> BuyItemAsync(string marketHashName, decimal price)
        {
            throw new NotImplementedException();
        }

        public async Task<Item> SellItemAsync(Item item, decimal price)
        {
            var cnyPrice = Math.Round(price / await GetUsdConvertRateAsync(), 2) - 0.01m;
            var url = new Uri("https://buff.163.com/api/market/sell_order/create/manual_plus");
            var buffId = _itemIds[item.MarketHashName];
            var income = await GetIncomeAsync(cnyPrice);

            var itemJson = new JsonObject
            {
                { "game", "csgo" },
                { "market_hash_name", item.MarketHashName },
                { "contextid", 2 },
                { "assetid", item.AssetId },
                { "classid", item.ClassId },
                { "instanceid", item.InstanceId },
                { "goods_id", buffId },
                { "price", cnyPrice },
                { "income", income },
                { "has_market_min_price", false },
                { "cdkey_id", string.Empty }
            };
            var assets = new JsonArray
            {
                itemJson
            };
            var payload = new JsonObject
            {
                { "game", "csgo" },
                { "assets", assets }
            };

            var request = CreateRequest(url, payload);

            var response = await _httpClient.SendAsync(request);
            var json = JsonNode.Parse(await response.Content.ReadAsStringAsync())!.AsObject();

            EnsureCodeOk(json, "Не удалось продать предмет.");
            SetCookies(response);

            return item;
        }

        public async Task<decimal> GetUsdConvertRateAsync()
        {
            var url = new Uri($"https://buff.163.com/account/api/user/info?_={Nonce}");
            var response = await _httpClient.GetStringAsync(url);
            var json = JsonNode.Parse(response)!.AsObject();

            EnsureCodeOk(json, "Не удалось получить курс валюты.");

            var rate = json["data"]!["buff_price_currency_rate_base_cny"]!.GetValue<decimal>();
            return rate;
        }

        private static void EnsureCodeOk(JsonObject json, string exceptionMessage)
        {
            if (!json.ContainsKey("code") || (json["code"]!.GetValue<string>() != "OK"))
                throw new Exception(exceptionMessage);
        }

        private async Task<decimal> GetIncomeAsync(decimal price)
        {
            var url = new Uri($"{RootUrl}/market/batch/fee?game=csgo&prices={price}&is_change=0&check_price=1&_={Nonce}");

            var request = CreateRequest(url);
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonNode.Parse(content)!.AsObject();

            EnsureCodeOk(json, "Невозможно проверить комиссию.");
            SetCookies(response);

            var fee = json["data"]!["fees"]![0]!.GetValue<decimal>();
            return price - fee;
        }

        private async Task<decimal> ConvertToUsd(decimal cny)
        {
            var rate = await GetUsdConvertRateAsync();
            return Math.Round(cny * rate, PointDigits);
        }

        private HttpRequestMessage CreateRequest(Uri url)
        {
            var cookieString = string.Join("; ", _cookies);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cookie", cookieString);
            return request;
        }

        private HttpRequestMessage CreateRequest(Uri url, JsonObject json)
        {
            var cookieString = string.Join("; ", _cookies);
            var content = new StringContent(json.ToJsonString());
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var token = _cookies.SingleOrDefault(c => c.Name == "csrf_token")!.Value.Split(';')[0];
            request.Headers.Add("X-CSRFToken", token);
            request.Headers.Add("Cookie", cookieString);
            request.Headers.Add("Referer", "https://buff.163.com/market/steam_inventory?game=csgo");
            request.Content = content;
            return request;
        }

        private void SetCookies(HttpResponseMessage message)
        {
            var setCookies = message.Headers.Single(h => h.Key == "Set-Cookie").Value;
            foreach (var setCookie in setCookies)
            {
                var cookie = setCookie.Split(';')[0].Split('=');
                _cookies.Add(new Cookie(cookie[0], cookie[1]));
            }
        }
    }
}