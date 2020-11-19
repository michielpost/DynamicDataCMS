using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DynamicDataCMS.Core.Models.JsonConverters
{
    public class CmsTypeJsonConverter : JsonConverter<CmsType>
    {
        public override CmsType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();
            if (value == null)
                return null;

            return new CmsType(value);
        }

        public override void Write(Utf8JsonWriter writer, CmsType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
