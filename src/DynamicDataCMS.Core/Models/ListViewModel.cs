using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicDataCMS.Core.Models
{
    public class ListViewModel
    {
        public string CmsType { get; set; } = default!;

        public IReadOnlyList<CmsItem> Items { get; set; } = new List<CmsItem>();

        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public SchemaLocation Schema { get; set; } = default!;
        public MenuItem MenuCmsItem { get; set; } = default!;
    }
}
