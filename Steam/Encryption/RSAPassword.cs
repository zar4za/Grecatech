namespace Grecatech.Steam.Encryption
{
    public class RSAPassword
    {
        public readonly long Timestamp;
        public readonly string EncodedString;

        public RSAPassword(long timestamp, string encodedString)
        {
            Timestamp = timestamp;
            EncodedString = encodedString;
        }
    }
}
