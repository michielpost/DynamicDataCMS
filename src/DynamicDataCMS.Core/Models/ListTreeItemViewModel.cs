using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicDataCMS.Core.Models
{
    public class ListTreeItemViewModel
    {
        public CmsTreeNode Node { get; set; }
        public ListTreeViewModel PageModel { get; set; }
        public bool HideChildNodes { get; }

        public ListTreeItemViewModel(CmsTreeNode node, ListTreeViewModel pageModel, bool hideChildNodes = false)
        {
            Node = node;
            PageModel = pageModel;
            HideChildNodes = hideChildNodes;
        }

    }
}
