using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicDataCMS.Core.Models
{
    public record CmsFile
    {
        public byte[] Bytes { get; set; } = new byte[0];
        public string? ContentType { get; set; }
    }
}
