using System.Net;
using System.Text;

namespace Grecatech.Steam.Models
{
    internal class SteamSession
    {
        public readonly string CookieString;
        public readonly string SessionId;
        public readonly string BrowserId;
        public readonly string SteamLoginSecure;
        public SteamSession(CookieContainer cookies)
        {
            var builder = new StringBuilder();
            foreach (var cookie in cookies.GetAllCookies())
            {
                builder.Append(cookie.ToString());
                builder.Append("; ");
            }
            CookieString = builder.ToString().TrimEnd();

            var cookieCollection = CookieString.Split("; ");
            SessionId = cookieCollection.First(x => x.Contains("sessionid"))
                                        .Split('=')[1];
            BrowserId = cookieCollection.First(x => x.Contains("browserid"))
                                        .Split('=')[1];
            SteamLoginSecure = cookieCollection.First(x => x.Contains("steamLoginSecure"))
                                        .Split('=')[1];
        }
    }
}
