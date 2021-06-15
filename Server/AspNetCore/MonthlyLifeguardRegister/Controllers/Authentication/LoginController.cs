
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Cors.Infrastructure;
//using System.Web.Http.Cors;
using MonthlyLifeguardRegister.Models.Authentication;
using Microsoft.Extensions.Options;
using MonthlyLifeguardRegister.Models;
using MonthlyLifeguardRegister.Classess.Security;
using MonthlyLifeguardRegister.ControllersLogic.Authentication;

namespace MonthlyLifeguardRegister.Controllers.Authentication
{
    /// <summary>
    /// Deals with users logging into the app
    /// </summary>
    [ApiController]
    [Route("API/Authentication/Login.php")]
    public class LoginController : Controller
    {
        public LoginController(IOptions<AppSettings> appSettings)
        {
            this.AppSettings = appSettings.Value;
        }

        /// <summary>
        /// The settings from the appsettings.json file
        /// </summary>
        public AppSettings AppSettings { get; }
  
        /// <summary>
        /// When the user sends login details to loginto the app
        /// </summary>
        /// <param name="loginData">user name nad password sent from client</param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        public LoginAuthentication authenticate([FromForm] LoginRequest loginData)
        {
            // the response message to send back to the client
            LoginAuthentication loginResponseData;
            
            LoginControllerLogic loginControllerLogic = new LoginControllerLogic();
            
            // check the login details to see if they are correct. if they are create a jwt cookie to send back to the user
            loginResponseData = loginControllerLogic.authenticate_CreateLoginResponse(loginData, this.AppSettings, this.HttpContext.Response);
            
            // the response we will send back to the client to let them know if they have sucsefully logged in or not
            return loginResponseData;

            
        }
        
    }

}
