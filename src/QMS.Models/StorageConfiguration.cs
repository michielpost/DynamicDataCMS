using System;
using System.Collections.Generic;
using System.Text;

namespace QMS.Models
{
    public class StorageConfiguration
    {
        public bool ReadFiles { get; set; }

        /// <summary>
        /// Indicates this storage provider should write files. Default true.
        /// </summary>
        public bool WriteFiles { get; set; } = true;

        public bool ReadCmsItems { get; set; }

        /// <summary>
        /// Indicates this storage provider should write CmsItems. Default true.
        /// </summary>
        public bool WriteCmsItems { get; set; } = true;
    }
}
