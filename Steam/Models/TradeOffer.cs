using System.Text.Json.Serialization;

namespace Grecatech.Steam.Models
{
    public class TradeOffer
    {
        private const long SteamId64Const = 76561197960265728;


        [JsonPropertyName("tradeofferid")]
        public string TradeOfferId { get; }

        [JsonIgnore]
        public long PartnerSteamId64 { get; }

        [JsonPropertyName("accountid_other")]
        public int PartnerSteamId32 { get; }

        [JsonPropertyName("message")]
        public string Message { get; }

        [JsonPropertyName("items_to_receive")]
        public List<Item> ItemsToRecieve { get; }

        [JsonPropertyName("confirmation_method")]
        public int ConfirmationMethod { get; }


        [JsonConstructor]
        public TradeOffer(string tradeOfferId, int partnerSteamId32, string message, List<Item> itemsToRecieve, int confirmationMethod)
        {
            TradeOfferId = tradeOfferId;
            PartnerSteamId64 = SteamId64Const + partnerSteamId32;
            PartnerSteamId32 = partnerSteamId32;
            Message = message;
            ItemsToRecieve = itemsToRecieve;
            ConfirmationMethod = confirmationMethod;
        }
    }
}
