using Microsoft.AspNetCore.Http;
using MonthlyLifeguardRegister.Classess.Security;
using MonthlyLifeguardRegister.Models;
using MonthlyLifeguardRegister.Models.Authentication;
using MonthlyLifeguardRegister.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.ControllersLogic.Authentication
{
    public class LoginControllerLogic
    {
        /// <summary>
        /// Authenticates the passed in credentions. if they are correct, creates a cookie with its value set to a jwt
        /// </summary>
        /// <param name="loginData">the credentials the client set to try and logo in</param>
        /// <param name="appSettings">appsettings.json file content</param>
        /// <param name="httpResponse">used to create a cookie to send back to the user</param>
        /// <returns>The response message to send back to the client to let them know if authentication was sucsefull</returns>
        public LoginAuthentication authenticate_CreateLoginResponse(LoginRequest loginData, AppSettings appSettings, HttpResponse httpResponse)
        {
            // JWT
            // https://jasonwatmore.com/post/2021/04/30/net-5-jwt-authentication-tutorial-with-example-api

            // the response data to the client
            LoginAuthentication loginResponseData = new LoginAuthentication();

            // will check if user name and password are correct
            Authorize authorize = new Authorize(appSettings);
            // the users details we get from the database if the login details they sent in are correct
            UserFullDetails userFullDetails = null;

            // check the login details the user sent in against the database to see if they are correct
            // Also return the users details if they are correct
            if (authorize.AuthorizeClientUser(loginData.userName,
                                             loginData.year,
                                             loginData.month,
                                             loginData.day,
                                             out userFullDetails) == true)
            {// credentials validated, log them in

                // create the jwt and set the cookie to send back to the user
                loginResponseData._jwt = this.createJwtAndSetCookie(userFullDetails, appSettings, httpResponse);
                loginResponseData._isLoggedIn = true;
                


            }
            else
            {// invalid credentials sent so can not log them in
                loginResponseData._isLoggedIn = false;
                loginResponseData._errorMessage = "invalid username and or password";
            }
            





            // converts the class to a json object and sends it back to the client
            return loginResponseData;
        }


        /// <summary>
        /// Creates a jwt with the users details and creates a cookie which will hold the created jwt
        /// </summary>
        /// <param name="userFullDetails">details of the user logging in</param>
        /// <param name="appSettings">Aappsettings.json file data</param>
        /// <param name="httpResponse">Allows us to set the cookie</param>
        /// <returns>the string version of the jwt</returns>
        private string createJwtAndSetCookie(UserFullDetails userFullDetails, AppSettings appSettings, HttpResponse httpResponse)
        {
            // set the cookie to expire in 30 days
            DateTime cookieExpireDate = DateTime.Now.AddDays(30);
            DateTimeOffset DateCookieExpires = new DateTimeOffset(DateTime.SpecifyKind(cookieExpireDate, DateTimeKind.Unspecified));


            // create the jwt string
            JsonWebToken jsonWebToken = new JsonWebToken(appSettings.jwtsecretKey);
            string jwt = jsonWebToken.createUserJWT(userFullDetails.id, userFullDetails.firstName, userFullDetails.surname, DateCookieExpires.DateTime);
            

            // create the cookie
            Cookies cookies = new Cookies(appSettings.DomainName);
            cookies.CreateCookie(httpResponse, Cookies.ClientCookieName, jwt, DateCookieExpires);

            return jwt;
        }
    }
}
