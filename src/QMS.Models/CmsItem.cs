using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace QMS.Models
{
    /// <summary>
    /// Single CmsDataItem (single language)
    /// </summary>
    public class CmsItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("cmstype")]
        public string CmsType { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JToken> AdditionalProperties { get; set; } = new Dictionary<string, JToken>();
    }
}
