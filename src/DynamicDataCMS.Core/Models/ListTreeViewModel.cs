using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicDataCMS.Core.Models
{
    public class ListTreeViewModel
    {
        public string CmsType { get; set; }

        public CmsTreeItem CmsTreeItem { get; set; }
        public MenuItem MenuCmsItem { get; set; }

    }
}
