using Grecatech.Steam.Encryption;
using Grecatech.Steam.Security;

namespace Grecatech.Steam.Models
{
    internal class LoginRequestContent
    {
        public LoginRequestContent(string username, RSAPassword password, Captcha captcha = null!, SteamGuardCode guard = null!)
        {
            string timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            _postContent = new Dictionary<string, string>()
            {
                { "donotcache", timestamp },
                { "password", password.EncodedString },
                { "username", username },
                { "twofactorcode", guard?.ToString() ?? string.Empty },
                { "emailauth", string.Empty },
                { "loginfriendlyname", string.Empty },
                { "captchagid", captcha?.Gid ?? string.Empty },
                { "captcha_text", captcha?.Answer ?? string.Empty },
                { "emailsteamid", string.Empty },
                { "rsatimestamp", password.Timestamp },
                { "remember_login", false.ToString() },
                { "tokentype", (-1).ToString() }
            };
        }

        private Dictionary<string, string> _postContent;
        public HttpContent ToHttpContent()
        {
            return new FormUrlEncodedContent(_postContent);
        }
    }
}
