using DynamicDataCMS.Core.Models;
using DynamicDataCMS.Core.Services.Extensions;
using DynamicDataCMS.Module.Auth.Basic.Models;
using DynamicDataCMS.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataCMS.Module.Auth.Basic
{
    public class EncryptPasswordInterceptor : IWriteCmsItemInterceptor
    {
        public bool HandlesType(CmsType cmsType)
        {
            //Only do this for the auth controller
            return CmsUser.DefaultCmsType == cmsType;
        }

        public Task<T> InterceptAsync<T>(T item, CmsType cmsType, Guid id, string? lang, string? currentUser) where T : CmsItem
        {
            var cmsUser = item.ToObject<CmsUser>();
            if (cmsUser == null)
                throw new Exception($"Unable to convert item to {cmsType}");

            //Encrypt password
            cmsUser.PasswordEncrypted = BCrypt.Net.BCrypt.HashPassword(cmsUser.Password);
            cmsUser.Password = string.Empty; //Do not store original password

            var modifiedItem = cmsUser.ToObject<T>();
            if (modifiedItem == null)
                throw new Exception($"Item was null after converting ({cmsType}).");

            return Task.FromResult(modifiedItem);
        }
    }
}
