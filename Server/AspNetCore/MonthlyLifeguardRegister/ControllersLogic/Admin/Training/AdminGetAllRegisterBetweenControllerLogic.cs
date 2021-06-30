using MonthlyLifeguardRegister.Classess.Database;
using MonthlyLifeguardRegister.Classess.Database.Training;
using MonthlyLifeguardRegister.Models;
using MonthlyLifeguardRegister.Models.Training;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.ControllersLogic.Admin.Training
{
    public class AdminGetAllRegisterBetweenControllerLogic
    {
        /// <summary>
        /// Gets a list of training registers from the database within the chosen dates. Returns empty list if anything goes wrong
        /// </summary>
        /// <param name="startDateDay"></param>
        /// <param name="startDateMonth">value between 1 and 12</param>
        /// <param name="startDateYear"></param>
        /// <param name="endDateDay"></param>
        /// <param name="endDateMonth">value betwen 1 and 12</param>
        /// <param name="endDateYear"></param>
        /// <param name="appSettings"></param>
        /// <returns>List of training registers</returns>
        public List<TrainingRegister> GetAllRegistersBetween_CreateResponse(int startDateDay, int startDateMonth, int startDateYear,
                                                               int endDateDay, int endDateMonth, int endDateYear,
                                                               AppSettings appSettings)
        {
            List<TrainingRegister> listOfRegister;
            DateTime startDate, endDate;
            long startDateAsUnixTimeStamp, endDateAsUnixTimeStamp;

            startDate = this.ConvertToDate(startDateYear, startDateMonth, startDateDay);
            endDate = this.ConvertToDate(endDateYear, endDateMonth, endDateDay);

            // check the dates were parsed ok.
            if (startDate == DateTime.MinValue || endDate == DateTime.MinValue)
                return this.SendBackEmptyArray();

            startDateAsUnixTimeStamp = this.ConvertToUnixTimeStamp(startDate);
            endDateAsUnixTimeStamp = this.ConvertToUnixTimeStamp(endDate);

            listOfRegister = this.GetAllRegistersBetweenDates(startDateAsUnixTimeStamp, endDateAsUnixTimeStamp, appSettings.sqlConectionStringLocation);

            return listOfRegister;
        }




        /// <summary>
        /// Converts the passed in parameters to a DateTime. If anything goes wrong, DateTime.MinValue is returned
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month">value between 1 and 12</param>
        /// <param name="day"></param>
        /// <returns>returns converted date or DateTime.Parse if anything goes wrong</returns>
        private DateTime ConvertToDate(int year, int month, int day)
        {
            if (month < 1 || month > 12)
                return DateTime.MinValue;


            // set the culture to uk
            CultureInfo en = new CultureInfo("en-GB");
            // the format the date will be in when it gets converted to a DateTime
            string dateFormat = "dd/MM/yyyy hh:mm:ss";
            // create the date as a string
            string dateValue = $"{day.ToString("00")}/{month.ToString("00")}/{year} 00:00:00";
            
            // Min value is what will be returned if the date can not be parsed.
            DateTime convertedDate = DateTime.MinValue;
            try
            {
                // parse the string into a DateTime object
                convertedDate = DateTime.ParseExact(dateValue, dateFormat, en);
            }
            catch
            {

            }

            // returns parsed date or DateTime.MinValue if anything went wrong
            return convertedDate;
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


        /// <summary>
        /// Gets a list of training register from the database that are running within the chosen dates passed in
        /// </summary>
        /// <param name="startDateAsUnixTimeStamp">Unix time stamp of when to start looking for register</param>
        /// <param name="endDateAsUnixTimeStamp">Unix time stamp fo when to stop looking for registers</param>
        /// <param name="sqlConectionStringLocation">Location of the database on disk</param>
        /// <returns></returns>
        private List<TrainingRegister> GetAllRegistersBetweenDates(long startDateAsUnixTimeStamp, long endDateAsUnixTimeStamp, string sqlConectionStringLocation)
        {
            List<TrainingRegister> trainingRegistersList;
            SqLiteConnection sqlCon;
            dbSQLiteTrainingRegister dbTrainingRegister;

            sqlCon = new SqLiteConnection();
            sqlCon.OpenConnection(sqlConectionStringLocation);

            dbTrainingRegister = new dbSQLiteTrainingRegister(sqlCon);
            trainingRegistersList = dbTrainingRegister.Select_DateTimeOfTraining_Between(startDateAsUnixTimeStamp, endDateAsUnixTimeStamp);

            sqlCon.CloseConnection();

            return trainingRegistersList;
        }

        /// <summary>
        /// An empty List to send back to the user. Normaly send this back because somthing went wrong
        /// </summary>
        /// <returns></returns>
        private List<TrainingRegister> SendBackEmptyArray()
        {
            return new List<TrainingRegister>();
        }
    }
}
