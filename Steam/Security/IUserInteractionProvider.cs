using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grecatech.Steam.Security
{
    public interface IUserInteractionProvider
    {
        public Task<Captcha> RequestSteamCaptchaAsync(string captchaId);
        public Task<SteamGuardCode> RequestSteamGuardAsync();

        public void Notify(string message);
        public void Warn(string message);
    }
}
