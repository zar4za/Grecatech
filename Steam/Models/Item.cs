using System.Text.Json.Serialization;

namespace Grecatech.Steam.Models
{
    public class Item
    {
        [JsonPropertyName("contextid")]
        public string ContextId { get; }

        [JsonPropertyName("assetid")]
        public string AssetId { get; }

        [JsonPropertyName("classid")]
        public string ClassId { get; }

        [JsonPropertyName("instanceid")]
        public string InstanceId { get; }

        public Item(string contextId, string assetId, string classId, string instanceId)
        {
            ContextId = contextId;
            AssetId = assetId;
            ClassId = classId;
            InstanceId = instanceId;
        }
    }
}
