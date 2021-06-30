using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MonthlyLifeguardRegister.Attributes;
using MonthlyLifeguardRegister.ControllersLogic.Admin.Training;
using MonthlyLifeguardRegister.Models;
using MonthlyLifeguardRegister.Models.Training;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Controllers.Admin.Training
{
    [ApiController]
    [Route("API/admin/TrainingRegister")]
    public class RegisterAdminController : ControllerBase
    {

        public RegisterAdminController(IOptions<AppSettings> appSettings)
        {
            this.AppSettings = appSettings.Value;
        }

        /// <summary>
        /// The settings from the appsettings.json file
        /// </summary>
        public AppSettings AppSettings { get; }
        /// <summary>
        /// gets a list of all registers between the selected dates
        /// </summary>
        /// <param name="startDateDay"></param>
        /// <param name="startDateMonth">Value between 1 and 12</param>
        /// <param name="startDateYear"></param>
        /// <param name="endDateDay"></param>
        /// <param name="endDateMonth">Value between 1 and 12</param>
        /// <param name="endDateYear"></param>
        /// <returns></returns>
        [Authorize(AuthorizationType = AuthorizeType.Admin)]
        [HttpPost]
        [Route("getAllRegistersBetween.php")]
        [Produces("application/json")]
        public List<TrainingRegister> GetAllRegisterBetween([FromForm] int startDateDay, [FromForm] int startDateMonth, [FromForm] int startDateYear,
                                          [FromForm] int endDateDay, [FromForm] int endDateMonth, [FromForm] int endDateYear)
        {
            AdminGetAllRegisterBetweenControllerLogic adminGetAllRegisterBetweenControllerLogic;
            adminGetAllRegisterBetweenControllerLogic = new AdminGetAllRegisterBetweenControllerLogic();

            return adminGetAllRegisterBetweenControllerLogic.GetAllRegistersBetween_CreateResponse(startDateDay, startDateMonth, startDateYear,
                                                                                                        endDateDay, endDateMonth, endDateYear, this.AppSettings);
            
        }

        /// <summary>
        /// Delete the register matching the registerID from the database. Also removes any user who were in the register
        /// </summary>
        /// <param name="registerID"></param>
        /// <returns></returns>
        [Authorize(AuthorizationType = AuthorizeType.Admin)]
        [HttpPost]
        [Route("deleteRegister.php")]
        [Produces("application/json")]
        public bool DeleteRegister([FromForm] int registerID)
        {
            AdminDeleteRegisterControllerLogic adminDeleteRegisterControllerLogic;
            adminDeleteRegisterControllerLogic = new AdminDeleteRegisterControllerLogic();

            return adminDeleteRegisterControllerLogic.DeleteRegister_CreateResponse(registerID, this.AppSettings);
        }


        /// <summary>
        /// Returns the register that matches the passed in registerID. Returns blank register with id set to -1 if not found
        /// </summary>
        /// <param name="registerID"></param>
        /// <returns></returns>
        [Authorize(AuthorizationType = AuthorizeType.Admin)]
        [HttpPost]
        [Route("getRegister.php")]
        [Produces("application/json")]
        public TrainingRegister GetRegister([FromForm] int registerID)
        {
            AdminGetRegisterControllerLogic adminGetRegisterControllerLogic;
            adminGetRegisterControllerLogic = new AdminGetRegisterControllerLogic();

            return adminGetRegisterControllerLogic.GetRegister_CreateResponse(registerID, this.AppSettings);
        }
    }
}
