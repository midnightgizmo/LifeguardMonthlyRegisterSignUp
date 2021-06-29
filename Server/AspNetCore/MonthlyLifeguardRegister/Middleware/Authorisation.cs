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
            // check to see if the user has normal user access
            // (they have sent the normal user cookie with its value set to jwt)
            this.CheckIfNormalUserAuthorisation(httpContext);

            // check to see if the user has admin access.
            // (theyu have sent the admin user cookie with its value set to jwt)
            this.CheckIfAdminUserAuthorisation(httpContext);
            
            await _next(httpContext);
        }

        /// <summary>
        /// Looks to see if the normal user cookie exists. Then checks its value to see if its a valid
        /// json web token. If checks out, will create an "User" item on httpContext.Items to indicate
        /// user has normal user access
        /// </summary>
        /// <param name="httpContext"></param>
        private void CheckIfNormalUserAuthorisation(HttpContext httpContext)
        {
            // try and find the cookie that would say the user is logged in
            string? cookieValue = httpContext.Request.Cookies[Cookies.ClientCookieName];

            // cookie does not exist so user is not logged in.
            if (cookieValue == null)
                return;

            // validate the cookie and get the data stored in it
            JsonWebToken jsonWebToken = new JsonWebToken(this.AppSettings.jwtsecretKey);
            JwtUser userInfo = jsonWebToken.getClientDataFromJwt(cookieValue);

            // if its an invalid jwt
            if (userInfo == null)
                return;// should we delete the cookie?


            // cookie has passed validation, user has a good jwt cookie
            // create an item called User and set the userInfo as its value.
            // if the User item exists, it means the user is logged in.
            // if teh user item does not exist, it means the user is not logged in.
            httpContext.Items["User"] = userInfo;

            
        }

        /// <summary>
        /// Looks to see if the admin cookie exists. Then checks its value to see if its a valid
        /// json web token. If checks out, will create an "admin" item on httpContext.Items to indicate
        /// user has admin access
        /// </summary>
        /// <param name="httpContext"></param>
        private void CheckIfAdminUserAuthorisation(HttpContext httpContext)
        {
            // try and find the cookie that would say the user is logged in
            string? cookieValue = httpContext.Request.Cookies[Cookies.AdminCookieName];

            // cookie does not exist so user is not logged in.
            if (cookieValue == null)
                return;
            

            // validate the cookie and get the data stored in it
            JsonWebToken jsonWebToken = new JsonWebToken(this.AppSettings.jwtsecretKey);
            // check if its an invalid jwt
            if (jsonWebToken.isJwtValid(cookieValue) == false)
                return; // should we now delete the cookie?

            

            // cookie has passed validation, user has a good admin jwt cookie
            // create an item called Admin. if the Admin item exists it means user is logged in as an admin
            httpContext.Items["Admin"] = true;
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
