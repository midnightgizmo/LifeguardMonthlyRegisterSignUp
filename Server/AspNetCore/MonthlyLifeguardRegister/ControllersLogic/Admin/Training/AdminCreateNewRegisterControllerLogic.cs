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
    public class AdminCreateNewRegisterControllerLogic
    {
        /// <summary>
        /// Creates a new register on the server. returns true if sucsefull
        /// </summary>
        /// <param name="dateOfTraining">Date format: yyyy-MM-dd HH:mm</param>
        /// <param name="dateWhenUserCanSeeRegister">Date format: yyyy-MM-dd HH:mm</param>
        /// <param name="dateWhenUserCanJoinRegister">Date format: yyyy-MM-dd HH:mm</param>
        /// <param name="maxNumberOfCandidatesAllowedOnRegister">max number of people allowed on register before its full</param>
        /// <param name="appSettings">used to get location of database on disk</param>
        /// <returns>returns true if scusefull, else false</returns>
        public bool CreateNewRegister_CreateResponse(string dateOfTraining, string dateWhenUserCanSeeRegister, string dateWhenUserCanJoinRegister, int maxNumberOfCandidatesAllowedOnRegister, AppSettings appSettings )
        {
            return this.CreateRegister(
                        this.ConvertDateStringToUnixTimeStamp(dateOfTraining),
                        this.ConvertDateStringToUnixTimeStamp(dateWhenUserCanSeeRegister),
                        this.ConvertDateStringToUnixTimeStamp(dateWhenUserCanJoinRegister),
                        maxNumberOfCandidatesAllowedOnRegister,
                        appSettings.sqlConectionStringLocation
                );
        }

        /// <summary>
        /// Creates a new Training reigser on the database and returns true if sucsefull
        /// </summary>
        /// <param name="dateOfTraining">Unix time stamp</param>
        /// <param name="dateWhenUserCanSeeRegister">Unix time stamp</param>
        /// <param name="dateWhenUserCanJoinReigister">Unix time stamp</param>
        /// <param name="maxNumberOfCandidatesAllowedOnRegister">max number of people allowed on register before its full</param>
        /// <param name="sqlConnectionString">location of database on disk</param>
        /// <returns>returns true if created, else false</returns>
        private bool CreateRegister(long dateOfTraining, long dateWhenUserCanSeeRegister, long dateWhenUserCanJoinReigister, int maxNumberOfCandidatesAllowedOnRegister, string sqlConnectionString)
        {
            SqLiteConnection sqlCon;
            dbSQLiteTrainingRegister dbTrainingRegister;
            TrainingRegister trainingRegister;

            sqlCon = new SqLiteConnection();
            sqlCon.OpenConnection(sqlConnectionString);

            dbTrainingRegister = new dbSQLiteTrainingRegister(sqlCon);
            trainingRegister = dbTrainingRegister.Insert(dateOfTraining, dateWhenUserCanJoinReigister, dateWhenUserCanJoinReigister, maxNumberOfCandidatesAllowedOnRegister);

            sqlCon.CloseConnection();

            if (trainingRegister == null)
                return false;
            else
                return true;
            
        }

        /// <summary>
        /// Converts the date from a string to a unix time stamp
        /// </summary>
        /// <param name="dateTime">Date format: yyyy-MM-dd HH:mm</param>
        /// <returns></returns>
        private long ConvertDateStringToUnixTimeStamp(string dateTime)
        {
            return this.ConvertToUnixTimeStamp(this.ParseDate(dateTime));
        }
        /// <summary>
        /// Converts the passed in string to a DateTime object with its cultre set to en-GB
        /// </summary>
        /// <param name="date">should be in the format yyyy-MM-dd HH:mm</param>
        /// <returns></returns>
        private DateTime ParseDate(string date)
        {
            // set the culture to uk
            CultureInfo en = new CultureInfo("en-GB");
            // the format the date will be in when it gets converted to a DateTime
            string dateFormat = "yyyy-MM-dd HH:mm";

            // parse the string into a DateTime object
            return DateTime.ParseExact(date, dateFormat, en);

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
