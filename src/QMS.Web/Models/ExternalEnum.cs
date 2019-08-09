using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QMS.Web.Models
{
    public class ExternalEnum
    {
        public string title { get; set; }
        public string type { get; set; }
        public List<string> @enum { get; set; }
        public Options options { get; set; }
    }

    public class Options
    {
        public List<string> enum_titles { get; set; }
    }

}
