using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using QMS.Core;
using QMS.Core.Auth;
using QMS.Core.Auth.Models;
using QMS.Core.Models;
using System.Collections.Generic;

namespace QMS.Core.Services
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

                x.EntityGroups.Add(new EntityGroupConfiguration()
                {
                     Name = "CMS",
                     Entities = new System.Collections.Generic.List<SchemaLocation>()
                     {
                         new SchemaLocation()
                         {
                              Key = Auth.Controllers.AuthController.UserType,
                              ListViewProperties = new List<ListViewProperty>() {
                                  new ListViewProperty() { DisplayName = nameof(CmsUser.Email), Key = nameof(CmsUser.Email) }
                              },
                              Name = "Users",
                              Schema = userSchema.ToJson()
                         }
                     }
                });
            });

            return builder;

        }
      
    }
}
