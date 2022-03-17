using Grecatech.Steam.Encryption;
using Grecatech.Steam.Exceptions;
using Grecatech.Steam.Models;
using Grecatech.Steam.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Grecatech.Steam
{
    public class SteamWebInterface
    {
        public SteamWebInterface(HttpClient httpClient, IUserInteractionProvider provider)
        {
            _httpClient = httpClient;
            _captchaHandler = provider.RequestSteamCaptchaAsync;
            _steamGuardHandler = provider.RequestSteamGuardAsync;
            _notificationHandler = provider.Notify;
            _warningHandler = provider.Warn;
        }


        public string CookieString = string.Empty;

        private const int MaxAuthorizationTries = 5;


        public delegate Task<Captcha> CaptchaHandler(string captchaGid);
        public delegate Task<SteamGuardCode> SteamGuardHandler();

        private readonly Uri Url = new("https://steamcommunity.com/");
        private readonly HttpClient _httpClient;
        private readonly CaptchaHandler _captchaHandler;
        private readonly SteamGuardHandler _steamGuardHandler;
        private readonly Action<string>? _notificationHandler;
        private readonly Action<string>? _warningHandler;

        private SteamSession? _steamSession;

        public async Task AuthorizeAsync(SteamUser user)
        {
            var url = new Uri("https://steamcommunity.com/login/dologin");
            var rsaProvider = new RSAProvider(_httpClient);
            var password = await rsaProvider.EncryptPasswordAsync(user);


            LoginResponseContent? loginResponse = null;
            for (var i = 0; i < MaxAuthorizationTries; i++)
            {
                Captcha? captcha = null;
                SteamGuardCode? code = null;
                if (loginResponse?.CaptchaNeeded == true)
                    captcha = await _captchaHandler(loginResponse.CaptchaGid);
                if (loginResponse?.TwoFactor == true)
                    code = await _steamGuardHandler();

                var content = new LoginRequestContent(user.Username, password, captcha, code).ToHttpContent();
                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                loginResponse = JsonSerializer.Deserialize<LoginResponseContent>(json)!;

                if (code != null && loginResponse.TwoFactor)
                    _warningHandler?.Invoke("Неправильный код.");

                if (loginResponse?.Success == true)
                {
                    var cookies = await GetAllCookies(response);
                    _steamSession = new SteamSession(cookies);
                    _httpClient.DefaultRequestHeaders.Add("Cookie", _steamSession.CookieString);
                    _notificationHandler?.Invoke($"Бот авторизирован в Steam");
                    return;
                }
            }
            throw new SteamResponseException($"Не удалось авторизироваться после {MaxAuthorizationTries} попыток.");
        }

        public async Task<bool> AcceptTradeOffer(TradeOffer tradeOffer)
        {
            if (_steamSession == null)
                throw new NullReferenceException("Клиент не авторизирован. Вызовите .AuthorizeAsync() сначала.");

            var url = new Uri($"https://steamcommunity.com/tradeoffer/{tradeOffer.TradeOfferId}/accept");
            var payload = new Dictionary<string, string>()
            {
                { "sessionid", _steamSession!.SessionId },
                { "serverid", "1" },
                { "tradeofferid", tradeOffer.TradeOfferId },
                { "partner", tradeOffer.PartnerSteamId64.ToString() },
                { "captcha", "" }
            };

            var content = new FormUrlEncodedContent(payload);
            _httpClient.DefaultRequestHeaders.Add("Referer", $"https://steamcommunity.com/tradeoffer/{tradeOffer.TradeOfferId}/");
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            _httpClient.DefaultRequestHeaders.Remove("Referer");
            return response.StatusCode == HttpStatusCode.OK;
        }

        private async Task<CookieContainer> GetAllCookies(HttpResponseMessage message)
        {
            var cookies = new CookieContainer();
            foreach (var cookie in message.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value)
            {
                cookies.SetCookies(Url, cookie);
            }
            var response = await _httpClient.GetAsync(Url);
            foreach (var cookie in response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value)
            {
                cookies.SetCookies(Url, cookie);
            }

            return cookies;
        }
    }
}
