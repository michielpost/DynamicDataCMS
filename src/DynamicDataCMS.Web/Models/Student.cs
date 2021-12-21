﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DynamicDataCMS.Web.Models
{
    public class Student
    {
        [Key]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [MaxLength(128)]
        public string Name { get; set; } = default!;

        public int? Age { get; set; }

        public string? Description { get; set; }
    }
}
