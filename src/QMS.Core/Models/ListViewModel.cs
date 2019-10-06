using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QMS.Models;

namespace QMS.Core.Models
{
    public class ListViewModel
    {
        public string CmsType { get; set; }

        public IReadOnlyList<CmsItem> Items { get; set; }

        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public SchemaLocation Schema { get; set; }
    }
}
