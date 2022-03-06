using System.Text.Json.Serialization;

namespace Grecatech.Steam.Models
{
    internal class RSAModel : IResponse
    {
        [JsonConstructor]
        public RSAModel(bool success, string modulus, string exponent, string timestamp, string tokenGid)
        {
            Success = success;
            Modulus = modulus;
            Exponent = exponent;
            Timestamp = timestamp;
            TokenGid = tokenGid;
        }

        [JsonPropertyName("success")]
        public bool Success { get; }

        [JsonPropertyName("publickey_mod")]
        public string Modulus { get; }

        [JsonPropertyName("publickey_exp")]
        public string Exponent { get; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; }

        [JsonPropertyName("token_gid")]
        public string TokenGid { get; }
    }
}