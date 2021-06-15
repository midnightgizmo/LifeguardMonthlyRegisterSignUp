using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Middleware
{
    /// <summary>
    /// Checks the incoming request to see if they have brought with them a jwt cookie.
    /// If they have the jwt cookie, check to see to if the jwt cookie is valid. If it is,
    /// give them acess to the routes that require authorisation.
    /// </summary>
    public class AuthorisationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorisationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            //httpContext.Request.Cookies
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AuthorisationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthorisationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorsMiddleware>();
        }
    }
}
