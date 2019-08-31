using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Storage.Interfaces
{
    /// <summary>
    /// Interface that defines write operations on files
    /// </summary>
    public interface IWriteFile
    {
        Task<Uri> WriteFile(byte[] bytes, string mimeType, string cmsType, string id, string fieldName, string lang);
    }
}
