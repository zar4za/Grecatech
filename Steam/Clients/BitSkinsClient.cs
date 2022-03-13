using Grecatech.Steam.Clients.Models;
using Newtonsoft.Json.Linq;
using OtpNet;
using Grecatech.Steam.Clients;
using System.Web;
using System.Text.Json;
using System.Net;

namespace Grecatech.Steam.Clients;

public class BitSkinsClient// : IMarketClient
{
    private const string RootUrl = "https://bitskins.com/api/v1";
    private readonly string _apiKey;
    private readonly HttpClient _client;
    private readonly Totp _totp;

    public BitSkinsClient(HttpClient httpClient, string apiKey, string codeSecret)
    {
        _client = httpClient;
        _apiKey = apiKey;
        _totp = new Totp(Base32.Base32Encoder.Decode(codeSecret));
    }

    public readonly decimal Fees = 0.9m;
    public async Task<decimal> GetActiveBalanceAsync()
    {
        var url = new Uri($"{RootUrl}/get_account_balance/?api_key={_apiKey}&code={_totp.ComputeTotp()}");
        var response = await _client.GetStringAsync(url);
        var balance = JObject.Parse(response);
        return Convert.ToDecimal(balance["available_balance"]);
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

    public async void GetAllItemPricesAsync(string appId)
    {
        var url = new Uri($"{RootUrl}/get_all_item_prices/?api_key={_apiKey}&code={_totp.ComputeTotp()}&app_id={appId}");
        var response = await _client.GetStringAsync(url);
    }

    public async void GetMarketDataAsync(string appId)
    {
        var url = new Uri(
            $"{RootUrl}/get_price_data_for_items_on_sale/?api_key={_apiKey}&code={_totp.ComputeTotp()}&app_id={appId}");
        var response = await _client.GetStringAsync(url);
    }

    public async void GetAccountInventoryAsync(string appId, int page = 1)
    {
        var url = new Uri(
            $"{RootUrl}/get_my_inventory/?api_key={_apiKey}&code={_totp.ComputeTotp()}&page={page}&app_id={appId}");
        var response = await _client.GetStringAsync(url);
    }

    public async void GetSpecificItemsOnSaleAsync(string appId, string itemIds)
    {
        var url = new Uri(
            $"{RootUrl}/get_specific_items_on_sale/?api_key={_apiKey}&code={_totp.ComputeTotp()}&item_ids={itemIds}&app_id={appId}");
        var response = await _client.GetStringAsync(url);
    }

    public async void GetResetPriceItemsAsync(string appId, int page = 1)
    {
        var url = new Uri(
            $"{RootUrl}/get_reset_price_items/?api_key={_apiKey}&code={_totp.ComputeTotp()}&page={page}&app_id={appId}");
        var response = await _client.GetStringAsync(url);
    }

    public async void GetMoneyEventsAsync(int page = 1)
    {
        var url = new Uri($"{RootUrl}/get_money_events/?api_key={_apiKey}&code={_totp.ComputeTotp()}&page={page}");
        var response = await _client.GetStringAsync(url);
    }

    public async void RequestWithdrawalAsync(decimal amount, string withdrawalMethod)
    {
        var url = new Uri(
            $"{RootUrl}/request_withdrawal/?api_key={_apiKey}&code={_totp.ComputeTotp()}&amount={amount}&withdrawal_method={withdrawalMethod}");
        var response = await _client.GetStringAsync(url);
    }

    public async Task<string> BuyItemAsync(string marketHashName, decimal price)
    {
        var item = await GetInventoryOnSaleAsync(marketHashName);
        if (item == null || item?.Value != price)
            return string.Empty;

        var url = new Uri($"{RootUrl}/buy_item/?api_key={_apiKey}&code={_totp.ComputeTotp()}&item_ids={item?.Key}&prices={price.ToString("F2").Replace(',','.')}&allow_trade_delayed_purchases=false");
        var response = await _client.GetStringAsync(url);
        var json = JObject.Parse(response);

        /*  {
         *      items: [
         *          {
         *              app_id: '730',
         *              context_id: '2',
         *              item_id: '123641644157',
         *              class_id: '993311319',
         *              instance_id: '519977149',
         *              market_hash_name: 'G3SG1 | Orange Kimono (Well-Worn)',
         *              price: '0.05'
         *          }],
         *      trade_tokens: [ '45d4c690482564a3' ]
         *   }
         */

        if (!json.ContainsKey("status") || !(json["status"].Value<string>() == "success"))
            return string.Empty;

        var tradeToken = json["data"]["trade_tokens"][0].Value<string>();
        return tradeToken;
    }

    public async void SellItemAsync(string itemIds, string prices, string appId)
    {
        var url = new Uri(
            $"{RootUrl}/list_item_for_sale/?api_key={_apiKey}&code={_totp.ComputeTotp()}&item_ids={itemIds}&prices={prices}&app_id={appId}");

        var response = await _client.GetStringAsync(url);
    }

    public async void ModifySaleAsync(string itemIds, string prices, string appId)
    {
        var url = new Uri(
            $"{RootUrl}/modify_sale_item/?api_key={_apiKey}&code={_totp.ComputeTotp()}&item_ids={itemIds}&prices={prices}&app_id={appId}");

        var response = await _client.GetStringAsync(url);
    }

    public async void DelistItemAsync(string itemIds, string appId)
    {
        var url = new Uri(
            $"{RootUrl}/delist_item/?api_key={_apiKey}&code={_totp.ComputeTotp()}&item_ids={itemIds}&app_id={appId}");

        var response = await _client.GetStringAsync(url);
    }

    public async void RelistItemAsync(string itemIds, string prices, string appId)
    {
        var url = new Uri(
            $"{RootUrl}/relist_item/?api_key={_apiKey}&code={_totp.ComputeTotp()}&item_ids={itemIds}&prices={prices}&app_id={appId}");

        var response = await _client.GetStringAsync(url);
    }

    public async void WithdrawItemAsync(string itemIds, string appId)
    {
        var url = new Uri(
            $"{RootUrl}/withdraw_item/?api_key={_apiKey}&code={_totp.ComputeTotp()}&item_ids={itemIds}&app_id={appId}");
        var response = await _client.GetStringAsync(url);
    }

    public async void BumpItemAsync(string itemIds, string appId)
    {
        var url = new Uri(
            $"{RootUrl}/bump_item/?api_key={_apiKey}&code={_totp.ComputeTotp()}&item_ids={itemIds}&app_id={appId}");
        var response = await _client.GetStringAsync(url);
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