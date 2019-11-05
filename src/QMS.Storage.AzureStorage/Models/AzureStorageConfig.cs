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
        /// <summary>
        /// Shared Access Signature (use Azure Portal to create a new SAS token)
        /// When null: uses development storage
        /// </summary>
        public string? SharedAccessSignature { get; set; }
        public string ContainerName { get; set; }
        public string AssetsContainerName { get; set; }
        public AzureStorageLocation StorageLocation { get; set; } = AzureStorageLocation.Tables;

    }

    public enum AzureStorageLocation
    {
        Tables,
        Blob,
        Both
    }
}
