using DynamicDataCms.Core.Auth.Models;
using DynamicDataCms.Core.Models;
using DynamicDataCms.Core.Services.Extensions;
using DynamicDataCms.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataCms.Core.Auth
{
    public class EncryptPasswordInterceptor : IWriteCmsItemInterceptor
    {
        public Task<T> InterceptAsync<T>(T item, string cmsType, Guid id, string? lang, string? currentUser) where T : CmsItem
        {
            //Only do this for the auth controller
            if(cmsType == CmsUser.DefaultCmsType)
            {
                var cmsUser = item.ToObject<CmsUser>();

                //Encrypt password
                cmsUser.PasswordEncrypted = BCrypt.Net.BCrypt.HashPassword(cmsUser.Password);
                cmsUser.Password = string.Empty; //Do not store original password

                item = cmsUser.ToObject<T>();

                return Task.FromResult(item);
            }

            return Task.FromResult(item);
        }
    }
}
