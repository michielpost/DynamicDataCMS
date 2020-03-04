using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataCms.Core.Auth
{
    public class QmsAuthenticatationMiddleware
    {

        private readonly RequestDelegate next;

        public QmsAuthenticatationMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.ToString().ToLowerInvariant().StartsWith("/cms")
                || context.Request.Path.ToString().ToLowerInvariant().StartsWith("/cms/image")
                || context.Request.Path.ToString().ToLowerInvariant().StartsWith("/cms/api/schema"))
            {
                // Let the request pass for non-cms routes
                return next(context);
            }

            if (!context.User.Identity.IsAuthenticated)
            {
                // The user is not authenticated, redirect to default authetication
                return context.ChallengeAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            return next(context);
        }
    }
}
