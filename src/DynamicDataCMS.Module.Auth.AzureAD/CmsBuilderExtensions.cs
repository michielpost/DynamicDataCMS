using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using DynamicDataCMS.Core;
using DynamicDataCMS.Core.Models;
using DynamicDataCMS.Core.Services;
using System.Collections.Generic;
using DynamicDataCMS.Module.Auth.AzureAD.Models;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Threading.Tasks;
using System.Data;
using DynamicDataCMS.Storage.Interfaces;

namespace DynamicDataCMS.Module.Auth.AzureAD
{
    /// <summary>
    /// Configure the CMS
    /// </summary>
    public static class CmsBuilderExtensions
    {
        public static CmsBuilder ConfigureDynamicDataCmsAuthAzureAD(this CmsBuilder builder)
        {
            var Configuration = builder.Configuration;
            var services = builder.Services;

            // Enable cookie authentication
            //services.AddMicrosoftIdentityWebAppAuthentication(Configuration);
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(options =>
                {
                    Configuration.Bind("AzureAD", options);
                    options.Events ??= new OpenIdConnectEvents();
                    options.Events.OnTokenValidated += OnTokenValidatedFunc;
                });

            services.AddHttpContextAccessor();

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

        private static async Task OnTokenValidatedFunc(TokenValidatedContext context)
        {
            string? email = context.Principal.Identity.Name;
            var readCmsItemService = context.HttpContext.RequestServices.GetRequiredService<IReadCmsItem>();

            var (users, total) = await readCmsItemService.List(CmsUser.DefaultCmsType, null, null, searchQuery: email);

            if (users.Count == 0 || string.IsNullOrWhiteSpace(email))
                context.Fail($"User ({email}) not added to the CMS.");

            // Custom code here
            await Task.CompletedTask.ConfigureAwait(false);
        }

    }
}
