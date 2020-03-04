using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicDataCMS.Core.Models
{
    public class EditViewModel
    {
        public string CmsType { get; set; }
        public Guid Id { get; set; }
        public MenuItem MenuCmsItem { get; set; }
        public SchemaLocation SchemaLocation { get; set; }
        public CmsItem? Data { get; set; }
        public CmsConfiguration CmsConfiguration { get; set; }
        public string? Language { get; set; }
        public List<CmsTreeNode> Nodes { get; set; } = new List<CmsTreeNode>();
        public string? TreeItemSchemaKey { get; set; }
        public Guid? TreeNodeId { get; set; }
    }
}
