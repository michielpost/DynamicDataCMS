using DynamicDataCMS.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DynamicDataCMS.Module.Auth.AzureAD.Models
{
    public record CmsUser
    {
        public static CmsType DefaultCmsType = "cmsUser";
        public static CmsSchemaType DefaultCmsSchemaType = "cmsUser";

        [Display(Name = "Email")]
        [Required] 
        [EmailAddress]
        public string Email { get; set; } = default!;
    }
}
