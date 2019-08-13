using QMS.Models;
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
        public CmsDataItem Data { get; set; }
        public CmsConfiguration CmsConfiguration { get; set; }
        public string Language { get; set; }
    }
}
