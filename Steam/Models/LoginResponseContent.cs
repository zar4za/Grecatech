using System.Text.Json.Serialization;

namespace Grecatech.Steam.Models
{
    internal class LoginResponseContent
    {
        [JsonPropertyName("success")]
        public readonly bool Success;

        [JsonPropertyName("message")]
        public readonly string Message;

        [JsonPropertyName("requires_twofactor")]
        public readonly bool TwoFactor;

        [JsonPropertyName("captcha_needed")]
        public readonly bool CaptchaNeeded;

        [JsonPropertyName("captcha_gid")]
        public readonly string CaptchaGid;

        [JsonPropertyName("emailsteamid")]
        public readonly string EmailSteamId;

        [JsonConstructor]
        public LoginResponseContent(bool success, string message, bool twoFactor, bool captchaNeeded, string captchaGid, string emailSteamId)
        {
            Success = success;
            Message = message;
            TwoFactor = twoFactor;
            CaptchaNeeded = captchaNeeded;
            CaptchaGid = captchaGid;
            EmailSteamId = emailSteamId;
        }
    }
}
