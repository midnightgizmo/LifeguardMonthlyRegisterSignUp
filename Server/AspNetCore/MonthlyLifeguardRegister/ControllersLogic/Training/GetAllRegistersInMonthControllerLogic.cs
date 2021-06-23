using MonthlyLifeguardRegister.Classess.Database;
using MonthlyLifeguardRegister.Classess.Database.Training;
using MonthlyLifeguardRegister.Models;
using MonthlyLifeguardRegister.Models.Training;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.ControllersLogic.Training
{
    public class GetAllRegistersInMonthControllerLogic
    {
        public List<TrainingRegister> GetAllRegistersInMonth_CreateLoginResponse(AppSettings appSettings, int year, int month)
        {
            SqLiteConnection sqlCon;
            dbSQLiteTrainingRegister dbTrainingRegister;

            List<TrainingRegister> listOfTrainingRegisters = new List<TrainingRegister>();

            // create a unix time stamp for the date at the begining of the month
            long startOfMonth = this.ConvertToUnixTimeStamp( this.GetDateAtBeginingOfMonth(year, month) );
            // create a unix time stamp for the date at the end of the month (last day of the month)
            long endOfMonth = this.ConvertToUnixTimeStamp( this.GetDateAtEndOfMonth(year, month) );


            // create a database connection and open it
            sqlCon = new SqLiteConnection();
            sqlCon.openConnection(appSettings.sqlConectionStringLocation);

            dbTrainingRegister = new dbSQLiteTrainingRegister(sqlCon);
            // get all registers between the start of the month and the end of the month
            listOfTrainingRegisters = dbTrainingRegister.select_dateTimeOfTraining_Between(startOfMonth,endOfMonth);
            // close the sqlite connection
            sqlCon.CloseConnection();

            // go through each register and populate it with any users it has in it.
            
            return null;
        }


        private DateTime GetDateAtBeginingOfMonth(int year, int month)
        {
            // set the culture to uk
            CultureInfo en = new CultureInfo("en-GB");
            // the format the date will be in when it gets converted to a DateTime
            string dateFormat = "dd/MM/yyyy hh:mm:ss";
            // create the date as a string
            string dateValue = $"01/{month.ToString("00")}/{year} 00:00:00";

            // parse the string into a DateTime object
            DateTime date = DateTime.ParseExact(dateValue, dateFormat, en);

            return date;
        }

        private DateTime GetDateAtEndOfMonth(int year, int month)
        {
            // find how many days are in the month
            int daysInMonth = DateTime.DaysInMonth(year, month);
            // create a new DateTime for the last day of the month
            return new DateTime(year, month, daysInMonth);
        }

        /// <summary>
        /// convert the DateTime object to a unix time stamp
        /// </summary>
        /// <param name="date">date to convert to unix time stamp</param>
        /// <returns>unix time stamp</returns>
        private long ConvertToUnixTimeStamp(DateTime date)
        {
            return ((DateTimeOffset)date).ToUnixTimeSeconds();
        }
    }
}
