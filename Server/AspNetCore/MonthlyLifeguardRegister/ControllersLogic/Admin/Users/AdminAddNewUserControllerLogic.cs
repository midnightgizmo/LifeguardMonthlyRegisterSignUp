using MonthlyLifeguardRegister.Classess.Database;
using MonthlyLifeguardRegister.Classess.Database.User;
using MonthlyLifeguardRegister.Classess.Security;
using MonthlyLifeguardRegister.Models;
using MonthlyLifeguardRegister.Models.User;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.ControllersLogic.Admin.Users
{
    public class AdminAddNewUserControllerLogic
    {
        /// <summary>
        /// Creates a new user and returns that new users details. returns null if somthing went wrong
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="surname"></param>
        /// <param name="isActive"></param>
        /// <param name="password"></param>
        /// <param name="appSettings"></param>
        /// <returns></returns>
        public UserFullDetails AddNewUser_CreateResponse(string firstName, string surname, bool isActive, string password, AppSettings appSettings)
        {
            return CreateNewUser(firstName, surname, isActive, password, appSettings.sqlConectionStringLocation);
        }

        /// <summary>
        /// Creates a new user and returns that new users details
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="surname"></param>
        /// <param name="isActive"></param>
        /// <param name="dateOfBirth">must be in the format "yyyy-MM-dd"</param>
        /// <param name="sqlConectionStringLocation">the location of the database on the disk</param>
        /// <returns></returns>
        private UserFullDetails CreateNewUser(string firstName, string surname, bool isActive, string dateOfBirth, string sqlConectionStringLocation)
        {
            SqLiteConnection sqlCon;
            dbSQLiteUser dbUser;
            UserFullDetails userFullDetails;

            sqlCon = new SqLiteConnection();
            sqlCon.OpenConnection(sqlConectionStringLocation);

            dbUser = new dbSQLiteUser(sqlCon);
            userFullDetails = dbUser.Insert(firstName, surname, Authorize.HashString(this.CreateUnixTimeStampFromDate(dateOfBirth).ToString()), isActive);


            sqlCon.CloseConnection();

            return userFullDetails;
        }



        /// <summary>
        /// Creates a unix time stamp from the past in date
        /// </summary>
        /// <param name="date">must be in a format of "yyyy-MM-dd"</param>
        /// <returns>Date represted as a unix time stamp</returns>
        private long CreateUnixTimeStampFromDate(string dateToConvert)
        {

            // set the culture to uk
            CultureInfo en = new CultureInfo("en-GB");
            // the format the date will be in when it gets converted to a DateTime
            string dateFormat = "yyyy-MM-dd hh:mm:ss";
            // create the date as a string
            string dateValue = $"{dateToConvert} 00:00:00";
            // parse the string into a DateTime object
            DateTime date = DateTime.ParseExact(dateValue, dateFormat, en);

            // convert the DateTime object to a unix time stamp
            long unixTimeStamp = ((DateTimeOffset)date).ToUnixTimeSeconds();

            // return the unix time stamp
            return unixTimeStamp;
        }
    }
}
