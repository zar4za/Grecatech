using Grecatech.Steam.Security;

namespace Grecatech.Steam.Models
{
    internal class LoginPost
    {
        public LoginPost(string username, string encryptedPassword, RSAModel rsa, Captcha captcha = null!, SteamGuardCode guard = null!)
        {
            string timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            postData = new Dictionary<string, string>()
            {
                { "donotcache", timestamp },
                { "password", encryptedPassword },
                { "username", username },
                { "twofactorcode", guard != null ? guard.ToString() : string.Empty },
                { "emailauth", string.Empty },
                { "loginfriendlyname", string.Empty },
                { "captchagid", captcha != null ?  captcha.Gid : string.Empty },
                { "captcha_text", captcha != null ? captcha.Answer : string.Empty },
                { "emailsteamid", string.Empty },
                { "rsatimestamp", rsa.Timestamp },
                { "remember_login", false.ToString() },
                { "tokentype", (-1).ToString() }
            };
        }

        private Dictionary<string, string> postData;
        public HttpContent ToHttpContent()
        {
            return new FormUrlEncodedContent(postData);
        }
    }
}
