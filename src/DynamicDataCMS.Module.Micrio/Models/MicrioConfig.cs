using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicDataCMS.Module.Micrio.Models
{
    public class MicrioConfig
    {
        public string ApiKey { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public string FolderShortId { get; set; } = default!;


        public bool IsValid()
        {
            return !string.IsNullOrEmpty(ApiKey)
                && !string.IsNullOrEmpty(UserId)
                && !string.IsNullOrEmpty(FolderShortId);
        }
    }
}
