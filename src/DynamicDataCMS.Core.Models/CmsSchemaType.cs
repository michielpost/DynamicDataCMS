using DynamicDataCMS.Core.Models.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DynamicDataCMS.Core.Models
{
    /// <summary>
    /// CmsSchemaType (string)
    /// Contains the Schema key
    /// Has implicit conversion from string
    /// Not to be confused with CmsType
    /// A schema can be used for multiple CmsTypes
    /// </summary>
    [JsonConverter(typeof(CmsSchemaTypeJsonConverter))]
    public class CmsSchemaType
    {
        public string Value { get; set; }

        public CmsSchemaType(string schemaType)
        {
            Value = schemaType;
        }

        public static implicit operator CmsSchemaType(string schemaType)
        {
            return new CmsSchemaType(schemaType);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
