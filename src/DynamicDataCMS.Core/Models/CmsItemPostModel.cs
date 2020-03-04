using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DynamicDataCms.Core.Models
{
    public class CmsItemPostModel
    {
        [JsonExtensionData]
        public Dictionary<string, JsonElement> AdditionalProperties { get; set; } = new Dictionary<string, JsonElement>();
    }
}
