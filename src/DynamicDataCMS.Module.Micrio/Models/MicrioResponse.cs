using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicDataCMS.Module.Micrio.Models
{
    public class MicrioResponse
    {
        public string? Id { get; set; }
        public string? CustomId { get; set; }
        public string? ShortId { get; set; }
        public bool IsFinished { get; set; }
        public bool HasError { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
