using DynamicDataCMS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicDataCMS.Editors.JsonEditor.Models
{
    public class EditViewModel
    {
        public string CmsType { get; set; } = default!;
        public Guid Id { get; set; }
        public MenuItem MenuCmsItem { get; set; } = default!;
        public SchemaLocation SchemaLocation { get; set; } = default!;
        public CmsItem? Data { get; set; }
        public CmsConfiguration CmsConfiguration { get; set; } = default!;
        public string? Language { get; set; }
        public List<CmsTreeNode> Nodes { get; set; } = new List<CmsTreeNode>();
        public string? TreeItemSchemaKey { get; set; }
        public Guid? TreeNodeId { get; set; }
    }
}
