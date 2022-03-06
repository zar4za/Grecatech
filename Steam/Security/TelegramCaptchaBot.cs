using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Grecatech.Steam.Security
{
    public class TelegramCaptchaBot : IUserInteractionProvider
    {
        public TelegramCaptchaBot(HttpClient client, string token, long chatId)
        {
            _httpClient = client;
            _botClient = new TelegramBotClient(token, client);
            _chatId = new ChatId(chatId);
        }

        private readonly HttpClient _httpClient;
        private readonly TelegramBotClient _botClient;
        private readonly ChatId _chatId;

        public async Task<Captcha> RequestSteamCaptchaAsync(string captchaId)
        {
            var msg = "Введите код с картинки:";

            var response = await _httpClient.GetAsync($"https://steamcommunity.com/login/rendercaptcha/?gid={captchaId}");
            var stream = await response.Content.ReadAsStreamAsync();

            var info = new System.Diagnostics.ProcessStartInfo($"https://steamcommunity.com/login/rendercaptcha/?gid={captchaId}")
            {
                UseShellExecute = true
            };

            System.Diagnostics.Process.Start(info);

            await _botClient.SendPhotoAsync(_chatId, stream, caption: msg);
            return new Captcha(captchaId, await RequestAnswerAsync());
        }

        public async Task<SteamGuardCode> RequestSteamGuardAsync()
        {
            var msg = "Введите код Steam Guard";
            await _botClient.SendTextMessageAsync(_chatId, msg);
            return new SteamGuardCode(await RequestAnswerAsync());
        }

        private async Task<string> RequestAnswerAsync(int tryCount = 300, int pauseMilliseconds = 1000)
        {
            var offset = (await _botClient.GetUpdatesAsync())?.LastOrDefault()?.Id + 1;
            for (var i = 0; i < tryCount; i++)
            {
                var updates = await _botClient.GetUpdatesAsync(offset);

                foreach (var update in updates)
                {
                    if (update!.Message!.Chat.Id == _chatId.Identifier)
                    {
                        Console.WriteLine(update.Message.Text);
                        return update.Message.Text;
                    }
                }
                await Task.Delay(pauseMilliseconds);
            }
            return null;
        }

        public async void Notify(string message)
        {
            await _botClient.SendTextMessageAsync(_chatId, message);
        }

        public async void Warn(string message)
        {
            var msg = $"\u2757 *Ошибка* \u2757\n{message}";
            await _botClient.SendTextMessageAsync(_chatId, msg);
        }
    }
}
