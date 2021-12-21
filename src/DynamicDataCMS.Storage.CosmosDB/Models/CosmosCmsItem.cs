using DynamicDataCMS.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicDataCMS.Storage.CosmosDB.Models
{
    /// <summary>
    /// CmsItem for CosmosDB based on Newtonsoft.Json
    /// </summary>
    internal class CosmosCmsItem : CosmosCmsDataItem
    {
        public Dictionary<string, CosmosCmsDataItem> Translations { get; set; } = new Dictionary<string, CosmosCmsDataItem>();
    }

    /// <summary>
    /// Single CmsDataItem (single language)
    /// </summary>
    internal class CosmosCmsDataItem
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("cmstype")]
        public string CmsType { get; set; } = default!;

        public DateTimeOffset LastModifiedDate { get; set; } = DateTimeOffset.UtcNow;

        [JsonExtensionData]
        public Dictionary<string, JToken> AdditionalProperties { get; set; } = new Dictionary<string, JToken>();
    }
}
