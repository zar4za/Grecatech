using Newtonsoft.Json.Linq;

namespace Grecatech.Steam.Clients.Models
{
    public class BuffPage
    {
        public List<Item> Items { get; set; }
        public int TotalPage { get; set; }

        public string[] GetNames()
        {
            var names = new string[Items.Count];
            for (int i = 0; i < Items.Count; i++)
            {
                names[i] = Items[i].MarketHashName;
            }
            return names;
        }
        public BuffPage(IEnumerable<Item> items, int totalPage)
        {
            Items = (List<Item>)items;
            TotalPage = totalPage;
        }

        public static BuffPage Parse(string json)
        {
            JObject jObject = JObject.Parse(json);
            var items = new List<Item>();
            if (jObject["code"]?.Value<string>() == "OK")
            {
                foreach (JToken item in jObject["data"]["items"])
                {
                    items.Add(Item.Parse(item));
                }
            }
            return new BuffPage(items, jObject["data"]["total_page"].Value<int>());
        }
    }
}