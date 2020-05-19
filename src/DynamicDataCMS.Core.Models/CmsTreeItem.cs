using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicDataCMS.Core.Models
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

        private string? _fullSlug;
        public string GetSlug(List<CmsTreeNode> all)
        {
            if (_fullSlug == null)
            {
                var slug = this.GetSlugArray(all, new HashSet<Guid>());

                _fullSlug = "/" + string.Join("/", slug);
            }

            return _fullSlug;
        }

        public List<string?> GetSlugArray(List<CmsTreeNode> all, HashSet<Guid> visited)
        {
            List<string?> sb = new List<string?>();


            if (this.ParentId.HasValue && !visited.Contains(this.ParentId.Value))
            {
                visited.Add(this.ParentId.Value);
                var parent = all.Find(x => x.NodeId == this.ParentId);
                
                if (parent != null)
                {
                    var parentSlug = parent.GetSlugArray(all, visited);
                    sb.AddRange(parentSlug);
                }
                
            }

            if (this.Name != "/")
                sb.Add(this.Name);

            return sb;
        }
    }
}
