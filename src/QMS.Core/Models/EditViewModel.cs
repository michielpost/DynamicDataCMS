using QMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QMS.Core.Models
{
    public class EditViewModel
    {
        public string CmsType { get; set; }
        public Guid Id { get; set; }
        public SchemaLocation SchemaLocation { get; set; }
        public CmsItem? Data { get; set; }
        public CmsConfiguration CmsConfiguration { get; set; }
        public string? Language { get; set; }
    }
}
