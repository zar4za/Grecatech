using Grecatech.Steam.Encryption;
using Grecatech.Steam.Models;
using Grecatech.Steam.Security;
using System.Net;
using System.Text.Json;

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

        private readonly HttpClient _client;
        private readonly CaptchaHandler _captchaHandler;
        private readonly SteamGuardHandler _steamGuardHandler;
        private readonly Action<string>? _notificationHandler;
        private readonly Action<string>? _warningHandler;
        private bool _isAuthorized = false;

        public delegate Task<Captcha> CaptchaHandler(string captchaGid);
        public delegate Task<SteamGuardCode> SteamGuardHandler();

        public async Task<CookieContainer> AuthorizeAsync(string username, string password)
        {
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

            return cookieContainer;
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
