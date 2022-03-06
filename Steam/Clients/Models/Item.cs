using Newtonsoft.Json.Linq;

namespace Grecatech.Steam.Clients.Models
{
    internal class Item
    {
        public string MarketHashName { get; set; }
        public decimal SellMinPrice { get; set; }

        public Item(string marketHashName, decimal sellMinPrice)
        {
            MarketHashName = marketHashName;
            SellMinPrice = sellMinPrice;
        }

        public static Item Parse(JToken jToken)
        {
            return new Item(jToken["market_hash_name"]?.ToString() ?? jToken["MarketHashName"].ToString(),
                jToken["sell_min_price"]?.Value<decimal>() ?? jToken["BestPrice"].Value<decimal>());
        }
    }
}