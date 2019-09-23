using Newtonsoft.Json;
using QMS.Models;
using QMS.Storage.CosmosDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace QMS.Storage.CosmosDB.Extensions
{
    internal static class CosmosCmsItemExtensions
    {
        internal static CmsItem ToCmsItem(this CosmosCmsItem item)
        {
            string json = JsonConvert.SerializeObject(item);
            return System.Text.Json.JsonSerializer.Deserialize<CmsItem>(json);
        }

        internal static CosmosCmsItem ToCosmosCmsItem(this CmsItem item)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(item);
            return JsonConvert.DeserializeObject<CosmosCmsItem>(json);
        }

    }
}
