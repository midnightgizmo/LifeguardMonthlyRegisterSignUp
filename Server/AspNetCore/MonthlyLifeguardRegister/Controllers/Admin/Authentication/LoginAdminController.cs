using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MonthlyLifeguardRegister.ControllersLogic.Admin.Authentication;
using MonthlyLifeguardRegister.Models;
using MonthlyLifeguardRegister.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Controllers.Admin.Authentication
{
    [ApiController]
    [Route("API/admin/Authentication/Login.php")]
    public class LoginAdminController : ControllerBase
    {
        public LoginAdminController(IOptions<AppSettings> appSettings)
        {
            this.AppSettings = appSettings.Value;
        }

        /// <summary>
        /// The settings from the appsettings.json file
        /// </summary>
        public AppSettings AppSettings { get; }

        [HttpPost]
        [Produces("application/json")]
        public LoginAuthentication Index([FromForm] string userName, [FromForm] string password)
        {
            LoginAdminControllerLogic loginAdminControllerLogic = new LoginAdminControllerLogic();

            // checks if the passed in creditionsal are correct and creates a jwt if they are.
            // Returns a response to let the user know if they have been logged in.
            return loginAdminControllerLogic.AuthenticateAdmin_CreateLoginResponse(userName, password, AppSettings, this.HttpContext.Response);
        }
    }
}
