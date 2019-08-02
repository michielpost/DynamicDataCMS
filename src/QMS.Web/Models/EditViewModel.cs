using QMS.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QMS.Web.Models
{
    public class EditViewModel
    {
        public string CmsType { get; set; }
        public string Id { get; set; }
        public SchemaLocation SchemaLocation { get; set; }
        public string Data { get; set; }
    }
}
