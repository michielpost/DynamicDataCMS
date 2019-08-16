using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Storage.Interfaces
{
    public interface IWriteFile
    {
        Task<Uri> WriteFile(byte[] bytes, string mimeType, string cmsType, string id, string fieldName, string lang);
    }
}
