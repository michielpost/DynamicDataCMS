using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace QMS.Services.Models
{
    public class JsonSchemaConfig
    {
        public List<SchemaLocation> SchemaLocations { get; set; }
    }

    public class SchemaLocation
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public Uri Uri { get; set; }
        public string Schema { get; set; }
    }
}
