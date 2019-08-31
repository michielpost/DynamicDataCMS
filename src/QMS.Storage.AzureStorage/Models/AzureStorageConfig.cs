using System;
using System.Collections.Generic;
using System.Text;

namespace QMS.Storage.AzureStorage.Models
{
    /// <summary>
    /// Configuration that is needed for QMS.Storage.AzureStorage package
    /// </summary>
    public class AzureStorageConfig
    {
        public string StorageAccount { get; set; }
        public string ContainerName { get; set; }
        public string AssetsContainerName { get; set; }
    }
}
