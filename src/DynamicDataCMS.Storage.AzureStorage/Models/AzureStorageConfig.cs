using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicDataCMS.Storage.AzureStorage.Models
{
    /// <summary>
    /// Configuration that is needed for DynamicDataCMS.Storage.AzureStorage package
    /// </summary>
    public class AzureStorageConfig
    {
        /// <summary>
        /// Azure Storage Connection String
        /// When null: uses development storage
        /// </summary>
        public string? ConnectionString { get; set; }
        public string ContainerName { get; set; }
        public string AssetsContainerName { get; set; }
        public AzureStorageLocation StorageLocation { get; set; } = AzureStorageLocation.Tables;

        /// <summary>
        /// Types that should not be saved by this provider
        /// </summary>
        public List<string> ExcludedTypes { get; set; } = new List<string>();

    }

    public enum AzureStorageLocation
    {
        Tables,
        Blob,
        Both
    }
}
