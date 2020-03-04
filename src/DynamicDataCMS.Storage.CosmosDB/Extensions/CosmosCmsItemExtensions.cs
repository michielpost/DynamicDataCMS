using Newtonsoft.Json;
using DynamicDataCms.Core.Models;
using DynamicDataCms.Storage.CosmosDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace DynamicDataCms.Storage.CosmosDB.Extensions
{
    internal static class CosmosCmsItemExtensions
    {
        internal static CmsItem ToCmsItem(this CosmosCmsItem item)
        {
            string json = JsonConvert.SerializeObject(item);
            return System.Text.Json.JsonSerializer.Deserialize<CmsItem>(json);
        }

        internal static T ToCmsItem<T>(this CosmosCmsDataItem item) where T : CmsItem
        {
            string json = JsonConvert.SerializeObject(item);
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }

        internal static CosmosCmsDataItem ToCosmosCmsItem(this CmsItem item)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(item);
            return JsonConvert.DeserializeObject<CosmosCmsItem>(json);
        }

    }
}
