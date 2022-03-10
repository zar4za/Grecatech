using Newtonsoft.Json.Linq;

namespace Grecatech.Steam.Clients.Models
{
    public class Item
    {
        public string MarketHashName { get; set; }
        public decimal SellMinPrice { get; set; }
        public decimal BuyMaxPrice { get; set; }

        public Item(string marketHashName, decimal sellMinPrice, decimal buyMaxPrice)
        {
            MarketHashName = marketHashName;
            SellMinPrice = sellMinPrice;
            BuyMaxPrice = buyMaxPrice;
        }

        public static Item Parse(JToken jToken)
        {
            return new Item(jToken["market_hash_name"]?.ToString() ?? jToken["MarketHashName"].ToString(),
                jToken["sell_min_price"]?.Value<decimal>() ?? jToken["BestPrice"].Value<decimal>(), jToken["buy_max_price"].Value<decimal>());
        }
    }
}