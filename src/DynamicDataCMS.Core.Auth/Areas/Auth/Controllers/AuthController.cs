using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DynamicDataCms.Core.Models;
using DynamicDataCms.Storage.Interfaces;
using System.Net.Http;
using System.Text.Json;
using DynamicDataCms.Core.Services;
using DynamicDataCms.Core.Auth.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using DynamicDataCms.Core.Services.Extensions;

namespace DynamicDataCms.Core.Auth.Controllers
{
    [Area("auth")]
    [Route("[area]")]
    public class AuthController : Controller
    {
        private readonly IReadCmsItem readCmsItemService;

        public AuthController(DataProviderWrapperService dataProviderService)
        {
            this.readCmsItemService = dataProviderService;
        }

        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var (users, total) = await readCmsItemService.List(CmsUser.DefaultCmsType, null, null, searchQuery: email);
            if (users.Any())
            {
                var user = (await readCmsItemService.Read<CmsItem>(CmsUser.DefaultCmsType, users.First().Id, null))?.ToObject<CmsUser>();
                if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordEncrypted))
                {
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.Email));
                    identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

                    identity.AddClaim(new Claim(ClaimTypes.Role, "cms"));

                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(3) } );

                    return RedirectToAction("Index", "Home", new { Area = "cms" });
                }
            }

            ModelState.AddModelError("email", "User not found");
            return View();

           
        }
        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }
    }
}
