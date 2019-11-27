using System;
using System.Collections.Generic;
using System.Text;

namespace QMS.Module.Micrio.Models
{
    public class MicrioRequest
    {
        public string ApiKey { get; set; }
        public string UserId { get; set; }
        public string ImageUrl { get; set; }
        public string? FolderShortId { get; set; }
        public string? CustomId { get; set; }
    }
}
