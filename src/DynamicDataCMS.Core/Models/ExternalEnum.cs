using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicDataCMS.Core.Models
{
    public class ExternalEnum
    {
        public string title { get; set; }
        public string type { get; set; }
        public List<string> @enum { get; set; }
        public EnumOptions options { get; set; }
    }

    public class EnumOptions
    {
        public List<string> enum_titles { get; set; }
    }

}
