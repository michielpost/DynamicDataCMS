using DynamicDataCMS.Core.Models;
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
        public string ContainerName { get; set; } = default!;
        public string AssetsContainerName { get; set; } = default!;
        public AzureStorageLocation StorageLocation { get; set; } = AzureStorageLocation.Tables;

        /// <summary>
        /// Types that should not be saved by this provider
        /// </summary>
        public List<CmsType> ExcludedTypes { get; set; } = new List<CmsType>();

        public bool GenerateUniqueFileName { get; set; } = true;
    }

    public enum AzureStorageLocation
    {
        Tables,
        Blob,
        Both
    }
}
