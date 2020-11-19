using DynamicDataCMS.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace DynamicDataCMS.Storage.EntityFramework.Extensions
{
    public static class CmsItemExtensions
    {
        public static T? ToObject<T>(this CmsItem item) where T : class
        {
            var json = JsonSerializer.Serialize(item);
            return JsonSerializer.Deserialize<T>(json);
        }

        public static T? ToObject<T>(this object obj) where T : CmsItem
        {
            string json = JsonSerializer.Serialize(obj);
            return JsonSerializer.Deserialize<T>(json);
        }

        public static CmsItem? ToCmsItem<T>(this T obj) where T : class
        {
            return obj.ToObject<CmsItem>();
        }
    }
}
