using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MonthlyLifeguardRegister.Attributes;
using MonthlyLifeguardRegister.Classess.Database.Training;
using MonthlyLifeguardRegister.ControllersLogic.Training;
using MonthlyLifeguardRegister.Models;
using MonthlyLifeguardRegister.Models.Training;
using MonthlyLifeguardRegister.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Controllers.Training
{
    /// <summary>
    /// Deals with users logging into the app
    /// </summary>
    [ApiController]
    [Route("API/Training/TrainingRegister")]
    public class RegistersController : ControllerBase
    {
        public RegistersController(IOptions<AppSettings> appSettings)
        {
            this.AppSettings = appSettings.Value;
        }

        /// <summary>
        /// The settings from the appsettings.json file
        /// </summary>
        public AppSettings AppSettings { get; }


        [HttpGet]
        [Authorize]
        [Route("getUpcomingTrainingRegisters.php")]
        [Produces("application/json")]
        public List<TrainingRegister> GetUpcomingRegisters()
        {
            GetUpcomingRegistersControllerLogic getUpcomingRegistersControllerLogic = new GetUpcomingRegistersControllerLogic();
            
            return getUpcomingRegistersControllerLogic.GetUpcomingRegisters_CreateLoginResponse(this.AppSettings);
        }

        [HttpPost]
        [Authorize]
        [Route("getAllRegistersInMonth.php")]
        [Produces("application/json")]
        public List<TrainingRegisterWithUsers> GetAllRegistersInMonth([FromForm]int year, [FromForm]int month)
        {
            GetAllRegistersInMonthControllerLogic getAllRegistersInMonthControllerLogic;

            getAllRegistersInMonthControllerLogic = new GetAllRegistersInMonthControllerLogic();
            
            return getAllRegistersInMonthControllerLogic.GetAllRegistersInMonth_CreateLoginResponse(this.AppSettings, year, ++month);
        }

        [HttpPost]
        [Authorize]
        [Route("addUserToRegister.php")]
        [Produces("application/json")]
        public JwtUser AddUserToRegister([FromForm] int userID, [FromForm]int registerID)
        {
            

            AddUserToRegisterControllerLogic addUserToRegisterControllerLogic;

            // get the users details from the jwt cookie they sent along with the request
            JwtUser userMakingRequest = (JwtUser)this.HttpContext.Items["User"];
            


            addUserToRegisterControllerLogic = new AddUserToRegisterControllerLogic();
            // Attemt to add the user to the register. will return the user that was added or
            // a black user with id set to -1 if user does not get added
            return addUserToRegisterControllerLogic.AddUserToRegister_CreateResponse(userMakingRequest, userID, registerID, this.AppSettings);
        }
    }
}
