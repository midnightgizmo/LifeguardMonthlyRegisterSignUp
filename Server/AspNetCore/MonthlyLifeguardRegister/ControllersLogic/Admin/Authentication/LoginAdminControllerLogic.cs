using Microsoft.AspNetCore.Http;
using MonthlyLifeguardRegister.Classess.Database;
using MonthlyLifeguardRegister.Classess.Database.Admin;
using MonthlyLifeguardRegister.Classess.Security;
using MonthlyLifeguardRegister.Models;
using MonthlyLifeguardRegister.Models.Authentication;
using MonthlyLifeguardRegister.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.ControllersLogic.Admin.Authentication
{
    public class LoginAdminControllerLogic
    {
        public LoginAuthentication AuthenticateAdmin_CreateLoginResponse(string userName, string password, AppSettings appSettings, HttpResponse httpResponse)
        {
            LoginAuthentication loginAuthentication;
            Authorize authorize;

            authorize = new Authorize(appSettings);

            // check the credentials the user sent against the ones in the database
            if (authorize.AuthorizeAdminUser(userName, password) == false)
                return this.CreateBadLogin();
            
            // create a sucsefull response & create the json web token & create the cookie that
            // lets the admin user stay logged in
            loginAuthentication = this.CreateSucsessLoginResponse(appSettings,httpResponse);

            // return the sucsefull response.
            return loginAuthentication;
        }



        /// <summary>
        /// Creates a sucess response and creates and sets the jwt cookie needed for the admin to be logged in
        /// </summary>
        /// <param name="appSettings">needed when creating the json web token and cookie</param>
        /// <param name="httpResponse">Used to create the cookie</param>
        /// <returns>response to send back to the client to indicate login was sucsesfull</returns>
        private LoginAuthentication CreateSucsessLoginResponse(AppSettings appSettings, HttpResponse httpResponse)
        {
            LoginAuthentication loginAuthentication;

            loginAuthentication = new LoginAuthentication();

            // set to true to indicate login was sucsefull
            loginAuthentication._isLoggedIn = true;
            // string version of the json web stoken
            loginAuthentication._jwt = this.CreateJwtAndSetCookie(appSettings, httpResponse);

            return loginAuthentication;
        }


        /// <summary>
        /// Creates a jwt with the users details and creates a cookie which will hold the created jwt
        /// </summary>
        /// <param name="userFullDetails">details of the user logging in</param>
        /// <param name="appSettings">Aappsettings.json file data</param>
        /// <param name="httpResponse">Allows us to set the cookie</param>
        /// <returns>the string version of the jwt</returns>
        private string CreateJwtAndSetCookie(AppSettings appSettings, HttpResponse httpResponse)
        {
            // set the cookie to expire in 30 days
            DateTime cookieExpireDate = DateTime.Now.AddDays(30);
            DateTimeOffset DateCookieExpires = new DateTimeOffset(DateTime.SpecifyKind(cookieExpireDate, DateTimeKind.Unspecified));


            // create the jwt string
            JsonWebToken jsonWebToken = new JsonWebToken(appSettings.jwtsecretKey);
            string jwt = jsonWebToken.createAdminJWT(DateCookieExpires.DateTime);


            // create the cookie
            Cookies cookies = new Cookies(appSettings.DomainName);
            cookies.CreateCookie(httpResponse, Cookies.AdminCookieName, jwt, DateCookieExpires);

            return jwt;
        }


        /// <summary>
        /// Creates response message to let the client know they were not logged in.
        /// </summary>
        /// <returns></returns>
        private LoginAuthentication CreateBadLogin()
        {
            LoginAuthentication loginAuthentication = new LoginAuthentication();
            loginAuthentication._errorMessage = "Invalid Login";
            loginAuthentication._isLoggedIn = false;

            return loginAuthentication;
        }

    }
}
