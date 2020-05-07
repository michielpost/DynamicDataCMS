using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DynamicDataCMS.Core.Models.JsonConverters
{
    public class CmsSchemaTypeJsonConverter : JsonConverter<CmsSchemaType>
    {
        public override CmsSchemaType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString();
            return new CmsSchemaType(value);
        }

        public override void Write(Utf8JsonWriter writer, CmsSchemaType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
