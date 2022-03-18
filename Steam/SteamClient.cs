using Grecatech.Steam.Models;
using Grecatech.Steam.Security;

namespace Grecatech.Steam
{
    public class SteamClient
    {
        private readonly SteamWebApi _api;
        private readonly SteamWebInterface _interface;
        private readonly SteamUser _user;

        public SteamClient(HttpClient httpClient, SteamUser user, IUserInteractionProvider provider, bool autoCodeGeneration = true)
        {
            _api = new SteamWebApi(httpClient, user.ApiKey);
            _user = user;

            if (autoCodeGeneration)
                _interface = new SteamWebInterface(httpClient, provider, user.TwoFactorSecret);
            else
                _interface = new SteamWebInterface(httpClient, provider);
        }

        public async Task AuthorizeAsync()
        {
            await _interface.AuthorizeAsync(_user);
        }

        public async Task<List<TradeOffer>> GetTradeOffersAsync(long unixTimeCutoff, bool onlyActive = true)
        {
            return await _api.GetTradeOffersAsync(unixTimeCutoff, onlyActive);
        }

        public async Task<bool> AcceptTradeAsync(TradeOffer tradeOffer)
        {
            return await _interface.AcceptTradeOffer(tradeOffer);
        }
    }
}
