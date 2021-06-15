﻿using Microsoft.AspNetCore.Http;
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
    }
}
