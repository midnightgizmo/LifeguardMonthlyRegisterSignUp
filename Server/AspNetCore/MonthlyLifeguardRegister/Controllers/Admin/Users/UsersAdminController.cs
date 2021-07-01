using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MonthlyLifeguardRegister.Attributes;
using MonthlyLifeguardRegister.ControllersLogic.Admin.Users;
using MonthlyLifeguardRegister.Models;
using MonthlyLifeguardRegister.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Controllers.Admin.Users
{
    [ApiController]
    [Route("API/admin/Users")]
    public class UsersAdminController : ControllerBase
    {

        public UsersAdminController(IOptions<AppSettings> appSettings)
        {
            this.AppSettings = appSettings.Value;
        }

        /// <summary>
        /// The settings from the appsettings.json file
        /// </summary>
        public AppSettings AppSettings { get; }


        /// <summary>
        /// Gets a list of all active users in the database and returns them as json
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthorizationType = AuthorizeType.Admin)]
        [HttpGet]
        [Route("getAllActiveUsers.php")]
        [Produces("application/json")]
        public List<UserFullDetails> GetAllActiveUsers()
        {
            AdminGetAllActiveUsersControllerLogic adminGetAllActiveUsersControllerLogic;

            adminGetAllActiveUsersControllerLogic = new AdminGetAllActiveUsersControllerLogic();

            return adminGetAllActiveUsersControllerLogic.GetAllActiveUsers_CreateResponse(this.AppSettings);
        }
    }
}
