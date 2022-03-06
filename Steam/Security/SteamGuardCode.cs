using System.Text.RegularExpressions;

namespace Grecatech.Steam.Security
{
    public class SteamGuardCode
    {
        public SteamGuardCode(string code)
        {
            Regex regex = new(@"[\w]{5}", RegexOptions.IgnoreCase);
            if (!regex.Match(code).Success)
                throw new ArgumentException("Incorrect Steam Guard code");

            _code = code.ToLower();
        }
        private readonly string _code;

        public override string ToString()
        {
            return _code;
        }
    }
}
