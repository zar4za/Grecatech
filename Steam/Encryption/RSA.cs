using System.Text.Json.Serialization;

namespace Grecatech.Steam.Encryption
{
    internal class RSA
    {
        [JsonPropertyName("success")]
        public readonly bool Success;

        [JsonPropertyName("publickey_mod")]
        public readonly string Modulus;

        [JsonPropertyName("publickey_exp")]
        public readonly string Exponent;

        [JsonPropertyName("timestamp")]
        public readonly string Timestamp;

        [JsonPropertyName("token_gid")]
        public readonly string TokenGid;


        [JsonConstructor]
        public RSA(bool success, string modulus, string exponent, long timestamp, string tokenGid)
        {
            Success = success;
            Modulus = modulus;
            Exponent = exponent;
            Timestamp = timestamp;
            TokenGid = tokenGid;
        }
    }
}