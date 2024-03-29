﻿using Microsoft.AspNetCore.Mvc;
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


        /// <summary>
        /// Gets a list of all users in the database
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthorizationType = AuthorizeType.Admin)]
        [HttpGet]
        [Route("getAllUsers.php")]
        [Produces("application/json")]
        public List<UserFullDetails> GetAllUsers()
        {
            AdminGetAllUsersControllerLogic adminGetAllUsersControllerLogic;

            adminGetAllUsersControllerLogic = new AdminGetAllUsersControllerLogic();
            return adminGetAllUsersControllerLogic.GetAllUsers_CreateResponse(this.AppSettings);
        }

        /// <summary>
        /// Updates a users details with the ones passed in
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userFirstName"></param>
        /// <param name="userSurname"></param>
        /// <param name="usersIsActive"></param>
        /// <param name="userNewPassword"></param>
        /// <returns></returns>
        [Authorize(AuthorizationType = AuthorizeType.Admin)]
        [HttpPost]
        [Route("updateUser.php")]
        [Produces("application/json")]
        public UserFullDetails UpdateUsersDetails([FromForm] int userId, [FromForm] string userFirstName, [FromForm] string userSurname, 
                                                  [FromForm] bool usersIsActive, [FromForm] string userNewPassword)
        {
            AdminUpdateUsersDetailsControllerLogic adminUpdateUsersDetailsControllerLogic;
            UserFullDetails userFullDetails;

            adminUpdateUsersDetailsControllerLogic = new AdminUpdateUsersDetailsControllerLogic();
            // try and update the users details
            userFullDetails = adminUpdateUsersDetailsControllerLogic.UpdateUsersDetails_CreateResponse(userId, userFirstName, userSurname, usersIsActive, userNewPassword, this.AppSettings);

            // if the users details were not updated
            if (userFullDetails == null)
            {
                // send back a bad response code 400
                this.Response.StatusCode = 400;
                return null;
            }

            // all was ok and users deatils were updated so send back the new users details.
            return userFullDetails;

        }

        /// <summary>
        /// Removes the user from the database. They will on longer be able to log in. Also removes them from all registers
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [Authorize(AuthorizationType = AuthorizeType.Admin)]
        [HttpPost]
        [Route("deleteUser.php")]
        [Produces("application/json")]
        public bool DeleteUser([FromForm]int userID)
        {
            AdminDeleteUserControllerLogic adminDeleteUserControllerLogic;

            adminDeleteUserControllerLogic = new AdminDeleteUserControllerLogic();
            return adminDeleteUserControllerLogic.DeleteUser_CreateResponse(userID, this.AppSettings);
        }

        /// <summary>
        /// Creates a new user in the database and returns the details of the new created users. Returns null if anything goes wrong
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="surname"></param>
        /// <param name="isActive"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [Authorize(AuthorizationType = AuthorizeType.Admin)]
        [HttpPost]
        [Route("createUser.php")]
        [Produces("application/json")]
        public UserFullDetails AddNewUser([FromForm]string firstName, [FromForm] string surname, [FromForm] bool isActive, [FromForm] string password)
        {
            AdminAddNewUserControllerLogic adminAddNewUserControllerLogic;

            adminAddNewUserControllerLogic = new AdminAddNewUserControllerLogic();
            return adminAddNewUserControllerLogic.AddNewUser_CreateResponse(firstName, surname, isActive, password, this.AppSettings);
        }



    }
}
