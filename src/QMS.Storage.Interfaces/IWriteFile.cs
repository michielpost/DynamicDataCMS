using QMS.Models;
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
        /// <summary>
        /// Write a file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="cmsType"></param>
        /// <param name="id"></param>
        /// <param name="fieldName"></param>
        /// <param name="lang"></param>
        /// <returns>Return the filename</returns>
        Task<string> WriteFile(CmsFile file, string cmsType, Guid id, string fieldName, string? lang);
    }
}
