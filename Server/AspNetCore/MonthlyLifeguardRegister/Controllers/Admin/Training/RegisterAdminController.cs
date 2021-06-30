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
        /// 
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
    }
}
