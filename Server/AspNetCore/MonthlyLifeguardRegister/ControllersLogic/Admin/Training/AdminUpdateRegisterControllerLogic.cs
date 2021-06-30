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
    public class AdminUpdateRegisterControllerLogic
    {

        public bool UpdateRegister_CreateResponse(int registerID, string dateOfTraining,
                                   string dateWhenUserCanSeeRegister, string dateWhenUserCanJoinRegister,
                                   int maxNumberOfCandidatesAllowedOnRegister, AppSettings appSettings)
        {
            long unixTimeStamp_dateOfTraining;
            long unixTimeStamp_dateWhenUserCanSeeRegister;
            long unixTimeStamp_dateWhenUserCanJoinRegister;

            // parse all the dates to a DateTime and then Convert them to a unix time stamp
            unixTimeStamp_dateOfTraining = this.ConvertToUnixTimeStamp(this.ParseDate(dateOfTraining));
            unixTimeStamp_dateWhenUserCanSeeRegister = this.ConvertToUnixTimeStamp(this.ParseDate(dateWhenUserCanSeeRegister));
            unixTimeStamp_dateWhenUserCanJoinRegister = this.ConvertToUnixTimeStamp(this.ParseDate(dateWhenUserCanJoinRegister));

            
            return this.UpdateRegister(registerID, unixTimeStamp_dateOfTraining, unixTimeStamp_dateWhenUserCanSeeRegister,
                                       unixTimeStamp_dateWhenUserCanJoinRegister, maxNumberOfCandidatesAllowedOnRegister, appSettings.sqlConectionStringLocation);
        }



        /// <summary>
        /// Converts the passed in string to a DateTime object with its cultre set to en-GB
        /// </summary>
        /// <param name="date">should be in the format yyyy-mm-dd hh:mm/param>
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

        /// <summary>
        /// Updates the register with the new values. Returns true if sucsefull
        /// </summary>
        /// <param name="registerID"></param>
        /// <param name="unixTimeStamp_dateOfTraining"></param>
        /// <param name="unixTimeStamp_dateWhenUserCanSeeRegister"></param>
        /// <param name="unixTimeStamp_dateWhenUserCanJoinRegister"></param>
        /// <param name="maxNumberOfCandidatesAllowedOnRegister"></param>
        /// <param name="sqlConectionStringLocation"></param>
        /// <returns>Returns true if updated, else returns false</returns>
        private bool UpdateRegister(int registerID, long unixTimeStamp_dateOfTraining, long unixTimeStamp_dateWhenUserCanSeeRegister, long unixTimeStamp_dateWhenUserCanJoinRegister, int maxNumberOfCandidatesAllowedOnRegister, string sqlConectionStringLocation)
        {
            SqLiteConnection sqlCon;
            dbSQLiteTrainingRegister dbTrainingRegister;
            TrainingRegister trainingRegister;

            sqlCon = new SqLiteConnection();
            sqlCon.OpenConnection(sqlConectionStringLocation);

            dbTrainingRegister = new dbSQLiteTrainingRegister(sqlCon);

            trainingRegister = dbTrainingRegister.Update(registerID,unixTimeStamp_dateOfTraining,unixTimeStamp_dateWhenUserCanSeeRegister,unixTimeStamp_dateWhenUserCanJoinRegister,maxNumberOfCandidatesAllowedOnRegister);
            sqlCon.CloseConnection();

            // if the register was not updated, return false
            if (trainingRegister == null)
                return false;
            // if the register was updated, return true
            else
                return true;
            
        }
    }
}
