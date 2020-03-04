using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicDataCMS.Core.Models
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
        public List<string> EditScripts { get; set; } = new List<string>();
        public List<string> Scripts { get; set; } = new List<string>();
        public List<string> Styles { get; set; } = new List<string>();

        public List<SchemaLocation> Schemas { get; set; } = new List<SchemaLocation>();
        public IEnumerable<SchemaLocation> SchemasInitialized => Schemas.Where(x => !string.IsNullOrEmpty(x.Schema));

        public List<MenuGroup> MenuGroups { get; set; } = new List<MenuGroup>();
        public IEnumerable<MenuItem> MenuItems => MenuGroups.SelectMany(x => x.MenuItems);

    }

    public class MenuGroup
    {
        public string Name { get; set; }
        public int Order { get; set; }

        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }

    public class MenuItem
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string? SchemaKey { get; set; }

        public List<string> SchemaKeys { get; set; } = new List<string>();
        public bool IsTree => SchemaKeys.Any();

        /// <summary>
        /// Indicates there can be only one instance
        /// TODO: Replace with Min / Max option like JsonSchema?
        /// </summary>
        public bool IsSingleton { get; set; }

        /// <summary>
        /// Page size for overview page, default 20
        /// </summary>
        public int PageSize { get; set; } = 20;

        public List<ListViewProperty> ListViewProperties { get; set; } = new List<ListViewProperty>();



    }

    public class SchemaLocation
    {
        public string Key { get; set; }
        public Uri? Uri { get; set; }
        public string? FileLocation { get; set; }

        public string? Schema { get; set; }
    }

    public class ListViewProperty
    {
        public string Key { get; set; }
        public string DisplayName { get; set; }
    }
}
