using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicDataCms.Core.Models
{
    public class EditTreeViewModel
    {
        public string CmsType { get; set; }
        public MenuItem MenuCmsItem { get; set; }
        public string? Language { get; set; }
        public string? TreeItemSchemaKey { get; set; }
        public Guid? TreeNodeId { get; set; }
    }
}
