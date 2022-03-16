using Grecatech.Steam.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Grecatech.Steam.Encryption
{
    internal class RSAProvider
    {
        public RSAProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private readonly HttpClient _httpClient;

        public async Task<RSAPassword> EncryptPassword(SteamUser user)
        {
            var rsa = await GetRSAKeysAsync(user.Username);
            var rsaProvider = new RSACryptoServiceProvider();
            var rsaParameters = new RSAParameters
            {
                Exponent = HexStringToByteArray(rsa.Exponent),
                Modulus = HexStringToByteArray(rsa.Modulus)
            };

            rsaProvider.ImportParameters(rsaParameters);
            var bytePassword = Encoding.ASCII.GetBytes(user.Password);
            var encodedPassword = rsaProvider.Encrypt(bytePassword, false);

            var rsaResult = new RSAPassword(rsa.Timestamp, Convert.ToBase64String(encodedPassword));
            return rsaResult;
        }

        private async Task<RSA> GetRSAKeysAsync(string username)
        {
            var url = new Uri("https://steamcommunity.com/login/getrsakey");
            var data = new Dictionary<string, string> { { "username", username } };
            var content = new FormUrlEncodedContent(data);
            var response = await _httpClient.PostAsync(url, content);
            var rsaJson = await response.Content.ReadAsStringAsync();
            var rsaResponse = JsonSerializer.Deserialize<RSA>(rsaJson)!;
            return rsaResponse;
        }

        private static byte[] HexStringToByteArray(string hexString)
        {
            using var stream = new MemoryStream(hexString.Length / 2);

            for (int i = 0; i < hexString.Length; i += 2)
            {
                stream.WriteByte(byte.Parse(hexString.Substring(i, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
            }
            return stream.ToArray();
        }
    }
}
