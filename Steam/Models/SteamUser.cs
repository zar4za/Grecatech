using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grecatech.Steam.Models
{
    public class SteamUser
    {
        public string Username { get; }
        public string Password { get; }
        public string ApiKey { get; }
        public string TwoFactorSecret { get; }


        public SteamUser(string username, string password, string apiKey, string twoFactorSecret)
        {
            Username = username;
            Password = password;
            ApiKey = apiKey;
            TwoFactorSecret = twoFactorSecret;
        }
    }
}
