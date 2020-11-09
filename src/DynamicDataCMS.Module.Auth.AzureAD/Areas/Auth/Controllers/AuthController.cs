using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DynamicDataCMS.Core.Models;
using DynamicDataCMS.Storage.Interfaces;
using System.Net.Http;
using System.Text.Json;
using DynamicDataCMS.Core.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using DynamicDataCMS.Core.Services.Extensions;
using DynamicDataCMS.Module.Auth.AzureAD.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace DynamicDataCMS.Module.Auth.AzureAD
{
    [Area("auth")]
    [Route("[area]")]
    public class AuthController : Controller
    {
       
        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            return SignOut(new AuthenticationProperties(),
                     CookieAuthenticationDefaults.AuthenticationScheme,
                     OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
