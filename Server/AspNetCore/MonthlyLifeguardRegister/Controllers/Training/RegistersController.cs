using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MonthlyLifeguardRegister.Attributes;
using MonthlyLifeguardRegister.Classess.Database.Training;
using MonthlyLifeguardRegister.ControllersLogic.Training;
using MonthlyLifeguardRegister.Models;
using MonthlyLifeguardRegister.Models.Training;
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
    }
}
