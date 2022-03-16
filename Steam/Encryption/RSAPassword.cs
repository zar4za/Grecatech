namespace Grecatech.Steam.Encryption
{
    internal class RSAPassword
    {
        public readonly string Timestamp;
        public readonly string EncodedString;

        public RSAPassword(string timestamp, string encodedString)
        {
            Timestamp = timestamp;
            EncodedString = encodedString;
        }
    }
}
