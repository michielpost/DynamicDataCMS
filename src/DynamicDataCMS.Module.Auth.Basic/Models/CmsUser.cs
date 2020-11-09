using DynamicDataCMS.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DynamicDataCMS.Module.Auth.Basic.Models
{
    public class CmsUser
    {
        public static CmsType DefaultCmsType = "cmsUser";
        public static CmsSchemaType DefaultCmsSchemaType = "cmsUser";

        [Display(Name = "Email")]
        [Required] 
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [Required]
        [MinLength(3, ErrorMessage = "Minimum of 3 characters")]
        [PasswordPropertyText]
        public string? Password { get; set; }

        [ReadOnly(true)]
        public string? PasswordEncrypted { get; set; }
    }
}
