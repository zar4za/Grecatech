using Grecatech.Steam.Encryption;
using Grecatech.Steam.Models;
using Grecatech.Steam.Security;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Grecatech.Steam
{
    public class SteamWebClient
    {
        public SteamWebClient(HttpClient client, string telegramToken, long chatId)
        {
            _client = client;
            var telegram = new TelegramCaptchaBot(client, telegramToken, chatId);
            _captchaHandler = telegram.RequestSteamCaptchaAsync;
            _steamGuardHandler = telegram.RequestSteamGuardAsync;
            _notificationHandler = telegram.Notify;
            _warningHandler = telegram.Warn;
        }
        public SteamWebClient(HttpClient client, IUserInteractionProvider provider)
        {
            _client = client;
            _captchaHandler = provider.RequestSteamCaptchaAsync;
            _steamGuardHandler = provider.RequestSteamGuardAsync;
            _notificationHandler = provider.Notify;
            _warningHandler = provider.Warn;
        }
        public SteamWebClient(HttpClient client, CaptchaHandler captchaHandler, SteamGuardHandler steamGuardHandler)
        {
            _client = client;
            _captchaHandler = captchaHandler;
            _steamGuardHandler = steamGuardHandler;
        }

        private const string RootUrl = "https://api.steampowered.com";
        private const long ValveMagicConstant = 76561197960265728;

        private readonly HttpClient _client;
        private readonly CaptchaHandler _captchaHandler;
        private readonly SteamGuardHandler _steamGuardHandler;
        private readonly Action<string>? _notificationHandler;
        private readonly Action<string>? _warningHandler;


        private CookieContainer _cookieContainer;
        private string _apiKey;
        private bool _isAuthorized = false;

        public delegate Task<Captcha> CaptchaHandler(string captchaGid);
        public delegate Task<SteamGuardCode> SteamGuardHandler();

        public async Task<bool> AuthorizeAsync(string username, string password, string apiKey)
        {
            _apiKey = apiKey;
            RSAModel rsa = await GetRSAKeysAsync(username);
            string encryptedPassword = RSAProvider.EncryptPassword(password, rsa);

            LoginResponse? loginResponse = null;
            Captcha? captcha = null;
            SteamGuardCode? code = null;
            CookieContainer cookieContainer = new CookieContainer();
            do
            {
                if (loginResponse != null && loginResponse.CaptchaNeeded)
                    captcha = await _captchaHandler(loginResponse.CaptchaGid);
                
                if (loginResponse != null && loginResponse.TwoFactor)
                    code = await _steamGuardHandler();

                HttpContent content = new LoginPost(username, encryptedPassword, rsa, captcha!, code!).ToHttpContent();
                var response = await _client.PostAsync("https://steamcommunity.com/login/dologin", content);
                response.EnsureSuccessStatusCode();

                loginResponse = JsonSerializer.Deserialize<LoginResponse>(await response.Content.ReadAsStringAsync())!;
                if (code != null && loginResponse.TwoFactor)
                    _warningHandler?.Invoke("Неправильный код.");
                foreach (var cookie in response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value)
                {
                    cookieContainer.SetCookies(new Uri("https://steamcommunity.com/"), cookie);
                }
            } while (loginResponse.CaptchaNeeded || loginResponse.TwoFactor);

            if (loginResponse.Success)
            {
                _isAuthorized = loginResponse.Success;
                _notificationHandler?.Invoke("Бот авторизирован в Steam");
                _client.DefaultRequestHeaders.Add("Cookie", cookieContainer.ToString());
                var response = await _client.GetAsync("https://steamcommunity.com/");

                foreach (var cookie in response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value)
                {
                    cookieContainer.SetCookies(new Uri("https://steamcommunity.com/"), cookie);
                }
            }
            else
                _warningHandler?.Invoke("Не удалось авторизоваться!");

            _cookieContainer = cookieContainer;
            return _isAuthorized;
        }

        public async Task<KeyValuePair<string, long>?> SearchForTradeOffer(string tradeToken)
        {
            var offsetSeconds = 300;
            var timeCutoff = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - offsetSeconds;
            var url = new Uri($"{RootUrl}/IEconService/GetTradeOffers/v1/?key={_apiKey}&get_received_offers=true&active_only=true&time_historical_cutoff={timeCutoff}");
            var response = await _client.GetStringAsync(url);
            var json = JObject.Parse(response);

            if (!json.ContainsKey("response") || !json["response"].HasValues)
                return null;

            var tradeOffers = json["response"]?["trade_offers_received"].ToList<JToken>();
            var i = tradeOffers?.FindIndex(offer => offer["message"].Value<string>().Contains(tradeToken));

            if (i is null || i == -1)
                return null;

            var tradeOffer = tradeOffers![i.Value];
            var pair = new KeyValuePair<string, long>(tradeOffer["tradeofferid"].Value<string>(), tradeOffer["accountid_other"].Value<int>() + ValveMagicConstant);
            return pair;
        }

        public async Task<bool> AcceptTradeOffer(string tradeOfferId, long partnerId)
        {
            var url = new Uri($"https://steamcommunity.com/tradeoffer/{tradeOfferId}/accept");
            var cookies = _cookieContainer.GetCookies(new Uri("https://steamcommunity.com/"));
            var payload = new Dictionary<string, string>()
            {
                { "sessionid", cookies["sessionid"]!.Value },
                { "serverid", "1" },
                { "tradeofferid", tradeOfferId },
                { "partner", partnerId.ToString() },
                { "captcha", "" }
            };
            var content = new FormUrlEncodedContent(payload);

            _client.DefaultRequestHeaders.Add("Cookie", _cookieContainer.ToString());
            var response = await _client.PostAsync(url, content);
            return response.StatusCode == HttpStatusCode.OK;
        }

        private async Task<RSAModel> GetRSAKeysAsync(string username)
        {
            var data = new Dictionary<string, string> { { "username", username } };
            var content = new FormUrlEncodedContent(data);
            var response = await _client.PostAsync("https://steamcommunity.com/login/getrsakey", content);
            var rsaJson = await response.Content.ReadAsStringAsync();
            var rsaResponse = JsonSerializer.Deserialize<RSAModel>(rsaJson)!;
            return rsaResponse;
        }
    }
}
