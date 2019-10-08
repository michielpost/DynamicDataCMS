using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QMS.Models
{
    /// <summary>
    /// Holds all CMS Configuration (schemas)
    /// </summary>
    public class CmsConfiguration
    {
        /// <summary>
        /// CMS Title to show in layout
        /// </summary>
        public string Title { get; set; }

        public List<string> Languages { get; set; } = new List<string>();

        public List<EntityGroupConfiguration> EntityGroups { get; set; } = new List<EntityGroupConfiguration>();

        public IEnumerable<SchemaLocation> Entities => EntityGroups.SelectMany(x => x.Entities);

        public bool IsInitialized => EntityGroups.SelectMany(x => x.Entities).Any();
    }

    public class EntityGroupConfiguration
    {
        public string Name { get; set; }
        public int Order { get; set; }

        public List<SchemaLocation> Entities { get; set; } = new List<SchemaLocation>();

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

        /// <summary>
        /// Page size for overview page, default 20
        /// </summary>
        public int PageSize { get; set; } = 20;

        public string Schema { get; set; }

        public List<ListViewProperty> ListViewProperties { get; set; } = new List<ListViewProperty>();

    }

    public class ListViewProperty
    {
        public string Key { get; set; }
        public string DisplayName { get; set; }
    }
}
