using System.Text.Json.Serialization;

namespace Grecatech.Steam.Models
{
    internal class LoginResponseContent
    {
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

        [JsonPropertyName("success")]
        public bool Success { get; }

        [JsonPropertyName("message")]
        public string Message { get; }

        [JsonPropertyName("requires_twofactor")]
        public bool TwoFactor { get; }

        [JsonPropertyName("captcha_needed")]
        public bool CaptchaNeeded { get; }

        [JsonPropertyName("captcha_gid")]
        public string CaptchaGid { get; }

        [JsonPropertyName("emailsteamid")]
        public string EmailSteamId { get; }
    }
}
