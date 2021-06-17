using Microsoft.AspNetCore.Http;
using MonthlyLifeguardRegister.Classess.Database;
using MonthlyLifeguardRegister.Classess.Database.Training;
using MonthlyLifeguardRegister.Models;
using MonthlyLifeguardRegister.Models.Training;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.ControllersLogic.Training
{
    public class GetUpcomingRegistersControllerLogic
    {
        /// <summary>
        /// Gets a list of training registers that are avalable between the first of this month and six months from start of this month
        /// </summary>
        /// <param name="appSettings">appsettings.json file content</param>
        /// <returns></returns>
        public List<TrainingRegister> GetUpcomingRegisters_CreateLoginResponse(AppSettings appSettings)
        {
            SqLiteConnection sqlCon;
            dbSQLiteTrainingRegister dbTrainingRegister;
            // create a date that will be the first of the current month
            DateTime dateAtStartOfMonth = this.createDateAtBeginingOfMonth();
            // create a date six months on from the above date
            DateTime dateInSixMonthsTime = dateAtStartOfMonth.AddMonths(6);
            List<TrainingRegister> listOfTrainingRegisters = new List<TrainingRegister>();

            // create a database connection and open it
            sqlCon = new SqLiteConnection();
            sqlCon.openConnection(appSettings.sqlConectionStringLocation);

            dbTrainingRegister = new dbSQLiteTrainingRegister(sqlCon);
            // get all registers between the start of the month and six months on from now
            listOfTrainingRegisters = dbTrainingRegister.select_dateTimeWhenRegisterIsActive_Between(this.convertToUnixTimeStamp(dateAtStartOfMonth),this.convertToUnixTimeStamp(dateInSixMonthsTime));
            // close the sqlite connection
            sqlCon.CloseConnection();

            return listOfTrainingRegisters;
        }

        /// <summary>
        /// Creates a DateTime that is set to the first day of current year/month
        /// </summary>
        /// <returns>DateTime set to first day of month</returns>
        private DateTime createDateAtBeginingOfMonth()
        {
            DateTime now = DateTime.Now;
            DateTime startOfMonth = new DateTime(now.Year, now.Month, 1);

            return startOfMonth;
        }



        /// <summary>
        /// Converts the passed in DateTime to a unix time stamp
        /// </summary>
        /// <param name="date">Date to be converted</param>
        /// <returns></returns>
        private long convertToUnixTimeStamp(DateTime date)
        {
            return ((DateTimeOffset)date).ToUnixTimeSeconds();
        }
    }
}
