using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace QMS.Web.Models
{
    public class Book
    {
        [Key]
        [NJsonSchema.Annotations.JsonSchemaIgnore]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }
    }
}
