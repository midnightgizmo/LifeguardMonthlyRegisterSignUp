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
    public class AdminUpdateUsersDetailsControllerLogic
    {
        /// <summary>
        /// Updates the users deatils with the passed in values. If userNewPassword is empty it will not get updated. May return null if update is not sucsefull
        /// </summary>
        /// <param name="userId">the user we want to update</param>
        /// <param name="userFirstName"></param>
        /// <param name="userSurname"></param>
        /// <param name="usersIsActive"></param>
        /// <param name="userNewPassword">the new password (leave blank if do not want password to be updated)</param>
        /// <param name="appSettings"></param>
        /// <returns>Returns the users new details or null if update was not sucsefull</returns>
        public UserFullDetails UpdateUsersDetails_CreateResponse(int userId, string userFirstName, string userSurname,
                                                                 bool usersIsActive, string userNewPassword,
                                                                 AppSettings appSettings)
        {
            SqLiteConnection sqlCon;
            UserFullDetails userFullDetails;

            sqlCon = new SqLiteConnection();
            sqlCon.OpenConnection(appSettings.sqlConectionStringLocation);


            // do we need to update the password
            if (userNewPassword != null && userNewPassword.Trim().Length > 0)
                // attempt to update the users details including the password
                userFullDetails = this.UpdateUsersDetails(userId, userFirstName, userSurname, usersIsActive, userNewPassword, sqlCon);
            else
                // attempt to update the users details (without the password)
                userFullDetails = this.UpdateUsersDetails(userId, userFirstName, userSurname, usersIsActive, sqlCon);



            sqlCon.CloseConnection();

            // if userFullDetails is null, it means the users details were not updated
            if (userFullDetails == null)
                // return null to indicate somthing went wrong
                return null;
            else
            {// users details were updated.

                // set the password text to blank before sending the data back to the user
                userFullDetails.password = string.Empty;
                // all is good, return the users new details
                return userFullDetails;
            }

            
        }

        

        private UserFullDetails UpdateUsersDetails(int userId, string userFirstName, string userSurname, bool usersIsActive, SqLiteConnection sqlCon)
        {
            // make sure to remove the password when we sent back the data to the user
            dbSQLiteUser dbUser;
            UserFullDetails userFullDetails;

            dbUser = new dbSQLiteUser(sqlCon);
            userFullDetails = dbUser.UpdateDetails(userId, userFirstName, userSurname, usersIsActive);

            return userFullDetails;
        }


        /// <summary>
        /// Updates the user with the new passed in details. returns the new user details or null if was not updated
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userFirstName"></param>
        /// <param name="userSurname"></param>
        /// <param name="usersIsActive"></param>
        /// <param name="usersPassword">new password in plain text</param>
        /// <param name="sqlCon">Open connection to the database</param>
        /// <returns></returns>
        private UserFullDetails UpdateUsersDetails(int userId, string userFirstName, string userSurname, bool usersIsActive, string usersPassword, SqLiteConnection sqlCon)
        {
            dbSQLiteUser dbUser;
            UserFullDetails userFullDetails;
            string hashedPassword;

            // convert the date to a unix time stamp and then hash it
            hashedPassword = Authorize.HashString(this.CreateUnixTimeStampFromDate(usersPassword.Trim()).ToString());

            dbUser = new dbSQLiteUser(sqlCon);
            userFullDetails = dbUser.UpdateAll(userId, userFirstName, userSurname, hashedPassword, usersIsActive);

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
