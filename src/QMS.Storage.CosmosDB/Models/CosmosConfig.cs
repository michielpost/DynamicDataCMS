using System;
using System.Collections.Generic;
using System.Text;

namespace QMS.Storage.CosmosDB.Models
{
    /// <summary>
    /// Configuration that is needed for QMS.Storage.CosmosDB package
    /// </summary>
    public class CosmosConfig
    {
        public string Endpoint { get; set; }
        public string Key { get; set; }
        public string DatabaseId { get; set; } = "qmsdb";
        public string ContainerId { get; set; } = "qms-container";

        /// <summary>
        /// Types that should not be saved by this provider
        /// </summary>
        public List<string> ExcludedTypes { get; set; } = new List<string>();
    }
}
