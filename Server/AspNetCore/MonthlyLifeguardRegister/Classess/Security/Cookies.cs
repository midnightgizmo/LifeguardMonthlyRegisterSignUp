using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Classess.Security
{
    public class Cookies
    {
        public Cookies(string domainName)
        {
            this.DomainName = domainName;
        }

        /// <summary>
        /// The name we will give the cookie that holds the jwt string for when the user has logged in
        /// </summary>
        public static string ClientCookieName = "jwtCookie";
        /// <summary>
        /// The name we will give the cookie that holds the jwt string for when the admin user has logged in
        /// </summary>
        public static string AdminCookieName = "jwtAdminCookie";

        public string DomainName { get; }

        public void CreateCookie(HttpResponse Response, string key, string value, DateTimeOffset DateExpires)
        {
            CookieOptions cookieOptions = new CookieOptions();

            // set cookie to expire in 30 days
            cookieOptions.Expires = DateExpires;
            cookieOptions.Path = "/";
            cookieOptions.Domain = DomainName;
            cookieOptions.Secure = true;
            cookieOptions.HttpOnly = false;
            cookieOptions.SameSite = SameSiteMode.None;

            // create the cookie
            Response.Cookies.Append(key, value, cookieOptions);
        }

        /// <summary>
        /// Deletes the specified cookie as long as it exists by setting its expiry date to the past
        /// </summary>
        /// <param name="Request">Used to check if the cookie exits</param>
        /// <param name="Response">Used to create the cookie with its date set in the past</param>
        /// <param name="cookieName">The name of the cookie to look for and delete</param>
        /// <returns></returns>
        public bool DeleteCookie(HttpRequest Request, HttpResponse Response, string cookieName)
        {
            if (Request.Cookies[cookieName] == null)
                return false;

            // create a date that is 5 days in the past
            DateTime pastDate = DateTime.Now.Subtract(new TimeSpan(5, 0, 0, 0));
            // conver the past date into a DateTimeOfset as this is the format needed for creating a cookie expiry date
            DateTimeOffset dateTimeOffset = new DateTimeOffset(pastDate);

            // overwrite the cookie that is allready there and set its date to 5 days ago which will make
            // the cookie get deleted.
            this.CreateCookie(Response, cookieName, "-1", dateTimeOffset);

            return true;
        }
        

    }
}
