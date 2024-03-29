﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MonthlyLifeguardRegister.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Attributes
{
    /// <summary>
    /// This gets called when a controller or method has the [Authorize] attribute added to it.
    /// Will check if the user is authorized to access the contoller/meothod the attribute is attached too.
    /// Sends back to the client a 403 response if they are not authorized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        /// <summary>
        /// The type of Authorization the attribute is looking for (defaults to NormalUser)
        /// </summary>
        public AuthorizeType AuthorizationType = AuthorizeType.NormalUser;
        /// <summary>
        /// Check if user is Authorized by looking for the "user" in the HttpContent.Items
        /// </summary>
        /// <param name="context">Used to check if User exists in the HttpContext.Items</param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool isNormalUserSet = (this.AuthorizationType & AuthorizeType.NormalUser) == AuthorizeType.NormalUser;
            bool isAdminUserSet = (this.AuthorizationType & AuthorizeType.Admin) == AuthorizeType.Admin;

            if(isNormalUserSet)
            {
                JwtUser user = (JwtUser)context.HttpContext.Items["User"];
                if (user == null)
                {
                    // not logged in as normal user
                    context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status403Forbidden };
                }
            }

            if(isAdminUserSet)
            {
                object IsAdmin = context.HttpContext.Items["Admin"];
                if (IsAdmin == null)
                {
                    // not logged in as normal user
                    context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status403Forbidden };
                }
            }
            
        }
    }

    /// <summary>
    /// The types of user access that are avalable
    /// </summary>
    [Flags]
    public enum AuthorizeType
    {
        NormalUser = 1, // next number is double this number (2)
        Admin = 2 // next number is double this number (4)
    }
}
