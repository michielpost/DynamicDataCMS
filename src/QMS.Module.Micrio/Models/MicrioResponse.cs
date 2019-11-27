using System;
using System.Collections.Generic;
using System.Text;

namespace QMS.Module.Micrio.Models
{
    public class MicrioResponse
    {
        public string? Id { get; set; }
        public string? CustomId { get; set; }
        public string? ShortId { get; set; }
        public string? IsFinished { get; set; }
        public string? HasError { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
