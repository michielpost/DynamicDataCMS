using DynamicDataCMS.Core.Models;
using System;
using System.Threading.Tasks;

namespace DynamicDataCMS.Storage.Interfaces
{
    /// <summary>
    /// Interface that defines Read operations on files
    /// </summary>
    public interface IReadFile
    {
        Task<CmsFile?> ReadFile(string fileName);
    }
}
