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
    public async Task<decimal> GetAccountBalanceAsync()
    {
        var url = new Uri($"{RootUrl}/get_account_balance/?api_key={_apiKey}&code={_totp.ComputeTotp()}");
        var response = await _client.GetStringAsync(url);

        /*  RESPONSE JSON
         *  
         *  {
         *      "status": "success",
         *      "data": {
         *          "available_balance": "0.0000",
         *          "pending_withdrawals": "0.0000",
         *          "withdrawable_balance": "0.0000",
         *          "couponable_balance": "0.0000"
         *      }
         *  }
         */

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

    //TODO: GetInventoryOnSell

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

    public async void BuyItemAsync(string itemIds, string prices, string appId, bool autoTrade,
        bool allowTradeDelayedPurchases)
    {
        var url = new Uri(
            $"{RootUrl}/buy_item/?api_key={_apiKey}&code={_totp.ComputeTotp()}&item_ids={itemIds}&prices={prices}&app_id={appId}&auto_trade={autoTrade}&allow_trade_delayed_purchases={allowTradeDelayedPurchases}");
        var response = await _client.GetStringAsync(url);
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
}