using System;
using System.Collections.Generic;
using System.Text;

namespace QMS.Module.Micrio.Models
{
    public class MicrioConfig
    {
        public string ApiKey { get; set; }
        public string UserId { get; set; }
        public string FolderShortId { get; set; }


        public bool IsValid()
        {
            return !string.IsNullOrEmpty(ApiKey)
                && !string.IsNullOrEmpty(UserId)
                && !string.IsNullOrEmpty(FolderShortId);
        }
    }
}
