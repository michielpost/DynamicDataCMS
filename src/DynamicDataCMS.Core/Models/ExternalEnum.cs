using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicDataCMS.Core.Models
{
    public class ExternalEnum
    {
        public string title { get; set; } = default!;
        public string type { get; set; } = default!;
        public List<string> @enum { get; set; } = new List<string>();
        public EnumOptions options { get; set; } = default!;
    }

    public class EnumOptions
    {
        public List<string> enum_titles { get; set; } = new List<string>();
    }

}
