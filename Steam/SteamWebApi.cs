using Grecatech.Steam.Exceptions;
using Grecatech.Steam.Models;
using System.Text.Json;

namespace Grecatech.Steam
{
    public class SteamWebApi
    {
        private const string SteamWebApiUrl = "https://api.steampowered.com";

        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public SteamWebApi(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async Task<List<TradeOffer>> GetTradeOffersAsync(long unixTimeCutoff, bool onlyActive = true)
        {
            var url = new Uri($"{SteamWebApiUrl}/IEconService/GetTradeOffers/v1/?key={_apiKey}&get_received_offers=true&active_only={onlyActive}&time_historical_cutoff={unixTimeCutoff}");
            var json = await _httpClient.GetJsonAsync(url);

            if (!json.ContainsKey("response"))
                throw new SteamResponseException("Отсутствует ключ 'response'.");
            if (!json["response"]!.AsObject().ContainsKey("trade_offers_received"))
                return new List<TradeOffer>(); 

            var tradeOffersNode = json["response"]!["trade_offers_received"];
            var tradeOffers = JsonSerializer.Deserialize<List<TradeOffer>>(tradeOffersNode)!;

            return tradeOffers;
        }
    }
}
