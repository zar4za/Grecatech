using System.Text.Json.Serialization;

namespace Grecatech.Steam.Encryption
{
    internal class RSA
    {
        [JsonConstructor]
        public RSA(bool success, string modulus, string exponent, long timestamp, string tokenGid)
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
        public long Timestamp { get; }

        [JsonPropertyName("token_gid")]
        public string TokenGid { get; }
    }
}