using System.Text.Json.Serialization;

namespace Grecatech.Steam.Models
{
    public class Item
    {

        [JsonPropertyName("assetid")]
        public string AssetId { get; }

        [JsonPropertyName("classid")]
        public string ClassId { get; }

        [JsonPropertyName("instanceid")]
        public string InstanceId { get; }

        public Item(string assetId, string classId, string instanceId)
        {
            AssetId = assetId;
            ClassId = classId;
            InstanceId = instanceId;
        }
    }
}
