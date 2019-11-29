using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QMS.Core.Models
{
    public class ListTreeViewModel
    {
        public string CmsType { get; set; }

        public IReadOnlyList<CmsItem> Items { get; set; }

        public CmsTreeItem CmsTreeItem { get; set; }
        public MenuItem MenuCmsItem { get; set; }
    }
}
