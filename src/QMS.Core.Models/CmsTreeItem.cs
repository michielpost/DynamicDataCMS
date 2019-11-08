using System;
using System.Collections.Generic;
using System.Text;

namespace QMS.Core.Models
{
    public class CmsTreeItem : CmsItem
    {
        public CmsTreeNode? Root { get; set; }
    }

    public class CmsTreeNode
    {
        public Guid NodeId { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }

        public Guid? CmsItemId { get; set; }
        public string? CmsItemType { get; set; }

        public List<CmsTreeNode> Children { get; set; } = new List<CmsTreeNode>();
    }
}
