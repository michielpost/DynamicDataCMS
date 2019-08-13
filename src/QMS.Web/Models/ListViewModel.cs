using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QMS.Models;
using QMS.Services.Models;

namespace QMS.Web.Models
{
    public class ListViewModel
    {
        public string CmsType { get; set; }

        public List<CmsItem> Items { get; set; }
        public SchemaLocation Schema { get; set; }
    }
}
