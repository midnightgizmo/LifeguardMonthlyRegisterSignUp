using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MonthlyLifeguardRegister.Classess.Security;
using MonthlyLifeguardRegister.Models;
using MonthlyLifeguardRegister.Models.User;
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

        public AuthorisationMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            this.AppSettings = appSettings.Value;
        }

        public AppSettings AppSettings { get; }

        public async Task Invoke(HttpContext httpContext)
        {
            // try and find the cookie that would say the user is logged in
            string? cookieValue = httpContext.Request.Cookies[Cookies.ClientCookieName];

            // cookie does not exist so user is not logged in.
            if (cookieValue == null)
            {
                await _next(httpContext);
                return;
            }

            // validate the cookie and get the data stored in it
            JsonWebToken jsonWebToken = new JsonWebToken(this.AppSettings.jwtsecretKey);
            JwtUser userInfo = jsonWebToken.getClientDataFromJwt(cookieValue);

            // if its an invalid jwt
            if(userInfo == null)
            {// should we delete the cookie?

                await _next(httpContext);
                return;
            }

            // cookie has passed validation, user has a good jwt cookie
            // create an item called User and set the userInfo as its value.
            // if the User item exists, it means the user is logged in.
            // if teh user item does not exist, it means the user is not logged in.
            httpContext.Items["User"] = userInfo;

            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AuthorisationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthorisationJWTMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthorisationMiddleware>();
        }
    }
}
