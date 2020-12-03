using DynamicDataCMS.Core.Models.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DynamicDataCMS.Core.Models
{
    /// <summary>
    /// CmsType (string)
    /// Contains the CmsType key
    /// Has implicit conversion from string
    /// Not to be confused with CmsSchemaType.
    /// A schema can be used for multiple CmsTypes
    /// </summary>
    [JsonConverter(typeof(CmsTypeJsonConverter))]
    public record CmsType
    {
        public string Value { get; set; }

        public CmsType(string cmsType)
        {
            Value = cmsType;
        }

        public static implicit operator CmsType(string cmsType)
        {
            return new CmsType(cmsType);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
