using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DynamicDataCMS.Core.Models
{
    /// <summary>
    /// Single CmsDataItem (single language)
    /// </summary>
    public class CmsItem
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("cmstype")]
        public CmsType CmsType { get; set; }

        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

        public string? LastModifiedBy { get; set; }

        public DateTimeOffset LastModifiedDate { get; set; } = DateTimeOffset.UtcNow;

        [JsonExtensionData]
        public Dictionary<string, JsonElement> AdditionalProperties { get; set; } = new Dictionary<string, JsonElement>();
    }
}
