using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace QMS.Models
{
    /// <summary>
    /// Core CmsItem, holds the default language version and all translations
    /// </summary>
    public class CmsItem : CmsDataItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("cmstype")]
        public string CmsType { get; set; }

        public Dictionary<string, CmsDataItem> Translations { get; set; } = new Dictionary<string, CmsDataItem>();

    }

    /// <summary>
    /// Single CmsDataItem (single language)
    /// </summary>
    public class CmsDataItem
    {
        [JsonExtensionData]
        public Dictionary<string, JToken> AdditionalProperties { get; set; } = new Dictionary<string, JToken>();
    }
}
