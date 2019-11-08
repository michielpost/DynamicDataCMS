using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using QMS.Core;
using QMS.Core.Auth;
using QMS.Core.Auth.Models;
using QMS.Core.Models;
using QMS.Core.Services;
using System.Collections.Generic;

namespace QMS.Core.Auth
{
    /// <summary>
    /// Configure the CMS
    /// </summary>
    public static class CmsBuilderExtensions
    {
        public static CmsBuilder ConfigureQmsAuth(this CmsBuilder builder)
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
                var userSchema = JsonSchema.FromType(typeof(CmsUser), new NJsonSchema.Generation.JsonSchemaGeneratorSettings()
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
                    Key = CmsUser.DefaultCmsType,
                    Schema = userSchema.ToJson(),
                });

                x.MenuGroups.Add(new MenuGroup()
                {
                     Name = "CMS",
                     CmsItems = new System.Collections.Generic.List<MenuCmsItem>()
                     {
                         new MenuCmsItem()
                         {
                              Key = CmsUser.DefaultCmsType,
                              SchemaKey = CmsUser.DefaultCmsType,
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
