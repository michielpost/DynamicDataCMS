using System;
using System.Threading.Tasks;

namespace QMS.Storage.Interfaces
{
    public interface IReadFile
    {
        Task<byte[]> ReadFile(string cmsType, string id, string fieldName, string lang);
    }

   
}
