using QMS.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QMS.Core.Auth.Models
{
    public class CmsUser
    {
        public static string DefaultCmsType = "cmsUser";

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
