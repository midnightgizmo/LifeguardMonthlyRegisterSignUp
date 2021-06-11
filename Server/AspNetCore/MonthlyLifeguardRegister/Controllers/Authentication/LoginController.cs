
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Cors.Infrastructure;
//using System.Web.Http.Cors;
using MonthlyLifeguardRegister.Models.Authentication;

namespace MonthlyLifeguardRegister.Controllers.Authentication
{
    /// <summary>
    /// Deals with users logging into the app
    /// </summary>
    [ApiController]
    [Route("API/Authentication/Login.php")]
    public class LoginController : Controller
    {
  
        /// <summary>
        /// When the user sends login details to loginto the app
        /// </summary>
        /// <param name="loginData">user name nad password sent from client</param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        public LoginAuthentication authenticate([FromForm] LoginRequest loginData)
        {
            LoginAuthentication loginResponseData = new LoginAuthentication();
            loginResponseData._isLoggedIn = false;
            loginResponseData._errorMessage = "Not yet implemented on server side";


            // converts the class to a json object and sends it back to the client
            return loginResponseData;
        }
        
    }

}
