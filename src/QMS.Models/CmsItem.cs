using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QMS.Models
{
    /// <summary>
    /// Core CmsItem, holds the default language version and all translations
    /// </summary>
    public class CmsItem : CmsDataItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("cmstype")]
        public string CmsType { get; set; }

        public Dictionary<string, CmsDataItem> Translations { get; set; } = new Dictionary<string, CmsDataItem>();

    }

    /// <summary>
    /// Single CmsDataItem (single language)
    /// </summary>
    public class CmsDataItem
    {
        [JsonExtensionData]
        public Dictionary<string, JsonElement> AdditionalProperties { get; set; } = new Dictionary<string, JsonElement>();
    }
}
