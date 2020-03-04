using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicDataCMS.Core.Models
{
    public class CmsFile
    {
        public byte[] Bytes { get; set; }
        public string ContentType { get; set; }
    }
}
