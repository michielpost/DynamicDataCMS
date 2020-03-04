using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DynamicDataCMS.Storage.AzureStorage.Models
{
    public class CmsItemTableEntity : TableEntity
    {
        public CmsItemTableEntity()
        {

        }

        public CmsItemTableEntity(string id, string cmsType)
        {
            PartitionKey = cmsType;
            RowKey = id.ToString();
        }

        public string? JsonData { get; set; }
    }
}
