using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QMS.Core.Models
{
    public class CmsTreeItem : CmsItem
    {
        public List<CmsTreeNode> Nodes { get; set; } = new List<CmsTreeNode>();
    }

    public class CmsTreeNode
    {
        public Guid NodeId { get; set; } = Guid.NewGuid();
        public Guid? ParentId { get; set; }

        public string? Name { get; set; }

        public Guid? CmsItemId { get; set; }
        public string? CmsItemType { get; set; }

        public IEnumerable<CmsTreeNode> Children(List<CmsTreeNode> all)
        {
            return all.Where(x => x.ParentId == this.NodeId);
        }

        public string GetSlug(List<CmsTreeNode> all)
        {
            var slug = this.GetSlugArray(all);

            return string.Join("/", slug);
        }

        public List<string?> GetSlugArray(List<CmsTreeNode> all)
        {
            List<string?> sb = new List<string?>();

            if (this.ParentId.HasValue)
            {
                var parent = all.Find(x => x.NodeId == this.ParentId);
                if (parent != null)
                {
                    var parentSlug = parent.GetSlugArray(all);
                    sb.AddRange(parentSlug);
                }
                
            }

            if (this.Name == "/")
                sb.Add(string.Empty);
            else
                sb.Add(this.Name);

            return sb;
        }
    }
}
