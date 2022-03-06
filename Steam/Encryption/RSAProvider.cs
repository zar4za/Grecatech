using Grecatech.Steam.Models;
using System.Security.Cryptography;
using System.Text;

namespace Grecatech.Steam.Encryption
{
    internal class RSAProvider
    {
        public static string EncryptPassword(string password, RSAModel rsaResponse)
        {
            var rsa = new RSACryptoServiceProvider();
            var rsaParameters = new RSAParameters
            {
                Exponent = HexStringToByteArray(rsaResponse.Exponent),
                Modulus = HexStringToByteArray(rsaResponse.Modulus)
            };
            rsa.ImportParameters(rsaParameters);
            byte[] bytePassword = Encoding.ASCII.GetBytes(password);
            byte[] encodedPassword = rsa.Encrypt(bytePassword, false);
            return Convert.ToBase64String(encodedPassword);
        }
        private static byte[] HexStringToByteArray(string hexString)
        {
            MemoryStream stream = new MemoryStream(hexString.Length / 2);

            for (int i = 0; i < hexString.Length; i += 2)
            {
                stream.WriteByte(byte.Parse(hexString.Substring(i, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
            }
            return stream.ToArray();
        }
    }
}
