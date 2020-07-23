using DynamicDataCMS.Core.Models;
using DynamicDataCMS.Storage.Interfaces;
using DynamicDataCMS.Web.Models.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DynamicDataCMS.Web.Interceptor
{
    public class ExampleInterceptor : IWriteCmsItemInterceptor
    {
        public bool HandlesType(CmsType cmsType)
        {
            return true;
        }

        public Task<T> InterceptAsync<T>(T item, CmsType cmsType, Guid id, string? lang, string? currentUser) where T : CmsItem
        {
            return Task.FromResult(item);
            //using (var stream = new MemoryStream())
            //{
            //    using (var writer = new Utf8JsonWriter(stream))
            //    {
            //        writer.WriteStartObject();
            //        writer.WriteString("date", DateTimeOffset.UtcNow);
            //        writer.WriteNumber("temp", 42);
            //        writer.WriteEndObject();
            //        writer.Flush();

            //        JsonDocument doc = await JsonDocument.ParseAsync(stream);

            //        var el = doc.RootElement;

            //        if (item.AdditionalProperties.ContainsKey("test"))
            //            item.AdditionalProperties["test"] = el;
            //        else
            //            item.AdditionalProperties.Add("test", el);
            //    }
            //}
        }
    }
}
