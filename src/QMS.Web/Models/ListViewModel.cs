using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QMS.Web.Models
{
    public class ListViewModel
    {
        public string CmsType { get; set; }

        public List<dynamic> Items { get; set; }
    }
}
