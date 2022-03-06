using System.Text.RegularExpressions;

namespace Grecatech.Steam.Security
{
    public class Captcha
    {
        public Captcha(string gid, string answer)
        {
            var regex = new Regex(@"[\d]{19}");
            if (!regex.Match(gid).Success)
                throw new ArgumentException("Incorrect Captcha id");
            regex = new(@"[@*#$&%\w]{6}", RegexOptions.IgnoreCase);
            if (!regex.Match(answer).Success)
                throw new ArgumentException("Incorrect Captcha answer");

            _id = gid;
            _answer = answer.ToLower();
        }
        private readonly string _answer;
        private readonly string _id;

        public string Gid => _id;
        public string Answer => _answer;
    }
}
