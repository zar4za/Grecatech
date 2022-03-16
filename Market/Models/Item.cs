using Newtonsoft.Json.Linq;

namespace Grecatech.Market.Models
{
    public class Item
    {
        public string AssetId { get; }
        public string ClassId { get; }
        public string InstanceId { get; }
        public string MarketHashName { get; }
        public string TradeToken { get; }

        public Item(string assetId, string classId, string instanceId, string marketHashName, string tradeToken)
        {
            AssetId = assetId;
            ClassId = classId;
            InstanceId = instanceId;
            MarketHashName = marketHashName;
            TradeToken = tradeToken;
        }

        public static Item Parse(JObject json)
        {
            var assetId = json["data"]["items"][0]["item_id"].ToString();
            var classId = json["data"]["items"][0]["class_id"].ToString();
            var instanceId = json["data"]["items"][0]["instance_id"].ToString();
            var MarketHashName = json["data"]["items"][0]["market_hash_name"].ToString();
            var tradeToken = json["data"]["trade_tokens"][0].ToString();

            return new Item(assetId, classId, instanceId, MarketHashName, tradeToken);
        }
    }
}