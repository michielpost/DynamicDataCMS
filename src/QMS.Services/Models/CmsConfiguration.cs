using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QMS.Services.Models
{
    public class CmsConfigLocation
    {
        public Uri Uri { get; set; }
    }

    public class CmsConfiguration
    {
        public List<string> Languages { get; set; }

        public List<SchemaLocation> Entities { get; set; }

        public bool IsInitialized => Entities?.Any() ?? false;
    }

    public class SchemaLocation
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public Uri Uri { get; set; }
        /// <summary>
        /// Indicates there can be only one instance
        /// TODO: Replace with Min / Max option like JsonSchema?
        /// </summary>
        public bool IsSingleton { get; set; }
        public string Schema { get; set; }

        public List<ListViewProperty> ListViewProperties { get; set; }

    }

    public class ListViewProperty
    {
        public string Key { get; set; }
        public string DisplayName { get; set; }
    }
}
