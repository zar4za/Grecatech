using Grecatech.Steam.Clients.Models;
using Newtonsoft.Json.Linq;
using OtpNet;
using Grecatech.Steam.Clients;
using System.Web;
using System.Text.Json;
using System.Net;

namespace Grecatech.Steam.Clients;

public class BitSkinsClient : IMarketClient
{
    private const string RootUrl = "https://bitskins.com/api/v1";
    private readonly string _apiKey;
    private readonly HttpClient _client;
    private readonly Totp _totp;

    public BitSkinsClient(HttpClient httpClient, string apiKey, string twoFactorSecret)
    {
        _client = httpClient;
        _apiKey = apiKey;
        _totp = new Totp(Base32.Base32Encoder.Decode(twoFactorSecret));
    }

    public readonly decimal Fees = 0.9m;
    public async Task<decimal> GetActiveBalanceAsync()
    {
        var url = new Uri($"{RootUrl}/get_account_balance/?api_key={_apiKey}&code={_totp.ComputeTotp()}");
        var response = await _client.GetStringAsync(url);
        var json = JObject.Parse(response);

        if (!json.ContainsKey("status") || (json["status"].Value<string>() != "success"))
            return decimal.Zero;

        return json["data"]["available_balance"].Value<decimal>(); ;
    }

    public async Task<decimal> GetItemPriceAsync(string marketHashName)
    {
        var url = new Uri($"{RootUrl}/get_inventory_on_sale/?api_key={_apiKey}&code={_totp.ComputeTotp()}&sort_by=price&order=asc&market_hash_name={marketHashName}&show_trade_delayed_items=-1");
        var response = await _client.GetStringAsync(url);
        var json = JObject.Parse(response);

        if (!json.ContainsKey("status") || !(json["status"].Value<string>() == "success"))
            return decimal.Zero;

        var price = json["data"]["items"]?[0]["price"].Value<decimal>();
        return price != null ? price.Value : decimal.Zero;
    }
    public async Task<string?> BuyItemAsync(string marketHashName, decimal price)
    {
        var item = await GetInventoryOnSaleAsync(marketHashName);
        if (item == null || item?.Value != price)
            return null;

        var url = new Uri($"{RootUrl}/buy_item/?api_key={_apiKey}&code={_totp.ComputeTotp()}&item_ids={item?.Key}&prices={price.ToString("F2").Replace(',', '.')}&allow_trade_delayed_purchases=false");
        var response = await _client.GetStringAsync(url);
        var json = JObject.Parse(response);

        if (!json.ContainsKey("status") || !(json["status"].Value<string>() == "success"))
            return null;

        var tradeToken = json["data"]["trade_tokens"][0].Value<string>();
        return tradeToken;
    }

    public async Task<string?> SellItemAsync(string assetId, decimal price)
    {
        var url = new Uri(
            $"{RootUrl}/list_item_for_sale/?api_key={_apiKey}&code={_totp.ComputeTotp()}&item_ids={assetId}&prices={price.ToString("F2").Replace(',', '.')}&app_id=730");

        var response = await _client.GetStringAsync(url);
        var json = JObject.Parse(response);

        if (!json.ContainsKey("status") || (json["status"].Value<string>() != "success"))
            return null;

        var tradeToken = json["data"]["trade_tokens"][0].Value<string>();
        return tradeToken;
    }
    private async Task<KeyValuePair<string, decimal>?> GetInventoryOnSaleAsync(string marketHashName)
    {
        var url = new Uri($"{RootUrl}/get_inventory_on_sale/?api_key={_apiKey}&code={_totp.ComputeTotp()}&sort_by=price&order=asc&market_hash_name={marketHashName}&show_trade_delayed_items=-1");
        var response = await _client.GetStringAsync(url);
        var json = JObject.Parse(response);

        if (!json.ContainsKey("status") || !(json["status"].Value<string>() == "success"))
            return null;

        var id = json["data"]["items"]?[0]["item_id"].Value<string>();
        var price = json["data"]["items"]?[0]["price"].Value<decimal>();
        
        if (id == null || price == null)
            return null;

        return new KeyValuePair<string, decimal>(id, price.Value);
    }
}