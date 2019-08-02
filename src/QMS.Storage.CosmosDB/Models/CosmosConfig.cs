using System;
using System.Collections.Generic;
using System.Text;

namespace QMS.Storage.CosmosDB.Models
{
    public class CosmosConfig
    {
        public string Endpoint { get; set; }
        public string Key { get; set; }
        public string DatabaseId { get; set; } = "qmsdb";
        public string ContainerId { get; set; } = "qms-container";
    }
}
