using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grecatech.Steam.Models
{
    internal class SteamUser
    {
        public SteamUser(JToken transferParameters, string apiKey)
        {
            _steamId = transferParameters["steamid"].Value<string>();
            _tokenSecure = transferParameters["token_secure"].Value<string>();
            _auth = transferParameters["auth"].Value<string>();
            _webCookie = transferParameters["webcookie"].Value<string>();
            _apiKey = apiKey;
        }

        private string _steamId;
        private string _tokenSecure;
        private string _auth;
        private string _webCookie;
        private string _apiKey;
    }
}
