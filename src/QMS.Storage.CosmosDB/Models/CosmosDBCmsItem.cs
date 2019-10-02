using QMS.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QMS.Storage.CosmosDB.Models
{
    /// <summary>
    /// Core CmsItem, holds the default language version and all translations
    /// </summary>
    internal class CosmosDBCmsItem : CmsItem
    {
        public Dictionary<string, CmsItem> Translations { get; set; } = new Dictionary<string, CmsItem>();
    }
}
