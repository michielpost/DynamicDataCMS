using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using DynamicDataCMS.Core;
using DynamicDataCMS.Core.Models;
using DynamicDataCMS.Core.Services;
using System.Collections.Generic;
using DynamicDataCMS.Module.Auth.Basic.Models;

namespace DynamicDataCMS.Module.Auth.Basic
{
    /// <summary>
    /// Configure the CMS
    /// </summary>
    public static class CmsBuilderExtensions
    {
        public static CmsBuilder ConfigureDynamicDataCmsAuthBasic(this CmsBuilder builder)
        {
            var Configuration = builder.Configuration;
            var services = builder.Services;

            // Enable cookie authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(x => x.LoginPath = "/auth/login");

            services.AddHttpContextAccessor();

            builder.AddInterceptor<EncryptPasswordInterceptor>();

            //Add CmsUser to CmsConfiguration
            services.PostConfigure<CmsConfiguration>(x =>
            {
                var userSchema = JsonSchema.FromType(typeof(CmsUser), new NJsonSchema.Generation.SystemTextJsonSchemaGeneratorSettings()
                {
                    //AllowReferencesWithProperties = true,
                    //AlwaysAllowAdditionalObjectProperties = true,
                    DefaultReferenceTypeNullHandling = NJsonSchema.Generation.ReferenceTypeNullHandling.NotNull,
                    GenerateAbstractProperties = true,
                    FlattenInheritanceHierarchy = true,
                });
                userSchema.Properties[nameof(CmsUser.Password)].Format = "password";
                userSchema.Properties.Remove(nameof(CmsUser.PasswordEncrypted));

                x.Schemas.Add(new SchemaLocation
                {
                    Key = CmsUser.DefaultCmsSchemaType.ToString(),
                    Schema = userSchema.ToJson(),
                });

                x.MenuGroups.Add(new MenuGroup()
                {
                     Name = "CMS",
                     MenuItems = new System.Collections.Generic.List<MenuItem>()
                     {
                         new MenuItem()
                         {
                              Key = CmsUser.DefaultCmsType.ToString(),
                              SchemaKey = CmsUser.DefaultCmsSchemaType.ToString(),
                              ListViewProperties = new List<ListViewProperty>() {
                                  new ListViewProperty() { DisplayName = nameof(CmsUser.Email), Key = nameof(CmsUser.Email) }
                              },
                              Name = "Users",
                         }
                     }
                });
            });

            return builder;

        }
      
    }
}
