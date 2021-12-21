using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicDataCMS.Core.Models
{
    public record SearchResult
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
    }
}
