using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DynamicDataCMS.Web.Models
{
    public class Book
    {
        [Key]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [MaxLength(128)]
        public string Name { get; set; } = default!;

        public string Author { get; set; } = default!;

        public string Description { get; set; } = default!;
    }
}
