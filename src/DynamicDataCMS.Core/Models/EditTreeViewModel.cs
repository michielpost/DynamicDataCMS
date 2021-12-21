using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicDataCMS.Core.Models
{
    public class EditTreeViewModel
    {
        public string CmsType { get; set; } = default!;
        public MenuItem MenuCmsItem { get; set; } = default!;
        public string? Language { get; set; }
        public string? TreeItemSchemaKey { get; set; }
        public Guid? TreeNodeId { get; set; }
    }
}
