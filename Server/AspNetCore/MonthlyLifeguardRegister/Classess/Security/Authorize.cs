using MonthlyLifeguardRegister.Classess.Database;
using MonthlyLifeguardRegister.Classess.Database.User;
using MonthlyLifeguardRegister.Models;
using MonthlyLifeguardRegister.Models.User;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Classess.Security
{
    /// <summary>
    /// Checks the passed in credentials to see if they are correct
    /// </summary>
    public class Authorize
    {
        private AppSettings _AppSettings;
        public Authorize(AppSettings appSettings)
        {
            this._AppSettings = appSettings;
        }
        /// <summary>
        /// Checks the passed in clients credentials to see if they are correct
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns>True if correct, else False</returns>
        public bool AuthorizeClientUser(string userName, int year, int month, int day, out UserFullDetails userFullDetails)
        {
            string password;
            SqLiteConnection sqlCon;
            dbSQLiteUser dbUser;

            // create a unix time stamp from the passed in date and then convert it to a sha256 hash string
            password = this.HashString(this.CreateUnixTimeStampFromDate(year, month, day).ToString());
            
            // create a database connection and open it
            sqlCon = new SqLiteConnection();
            sqlCon.OpenConnection(this._AppSettings.sqlConectionStringLocation);

            // try and find the users surname and password in the database
            dbUser = new dbSQLiteUser(sqlCon);
            userFullDetails = dbUser.Select_By_SurnameAndPassword(userName, password);

            // close the database connection
            sqlCon.CloseConnection();

            // if we did not find a match, return false to inidcate the user is not authorized
            if (userFullDetails == null)
                return false;
            // we did find users details, to they are authorized, so return true
            else
                return true;
            
        }
        /// <summary>
        /// Creates a unix time stamp from the past in date
        /// </summary>
        /// <param name="year">Year part of date</param>
        /// <param name="month">Month part of date</param>
        /// <param name="day">Day part of date</param>
        /// <returns>Date represted as a unix time stamp</returns>
        private long CreateUnixTimeStampFromDate(int year, int month, int day)
        {

            // set the culture to uk
            CultureInfo en = new CultureInfo("en-GB");
            // the format the date will be in when it gets converted to a DateTime
            string dateFormat = "dd/MM/yyyy hh:mm:ss";
            // create the date as a string
            string dateValue = $"{day.ToString("00")}/{month.ToString("00")}/{year} 00:00:00";
            // parse the string into a DateTime object
            DateTime date = DateTime.ParseExact(dateValue, dateFormat, en);

            // convert the DateTime object to a unix time stamp
            long unixTimeStamp = ((DateTimeOffset)date).ToUnixTimeSeconds();

            // return the unix time stamp
            return unixTimeStamp;
        }

        /// <summary>
        /// Converts the passed in string to a sha256 hashed string
        /// </summary>
        /// <param name="textToHash">the string to be hashed</param>
        /// <returns>the hashed string in lower case</returns>
        private string HashString(string textToHash)
        {
            //convert the text to a byte array
            byte[] textByteData = System.Text.Encoding.UTF8.GetBytes(textToHash);
            byte[] hashedByteArray;

            // set up the SA256 so we can hash
            using (SHA256 mySHA256 = SHA256.Create())
            {
                // hash the byte array
                hashedByteArray = mySHA256.ComputeHash(textByteData);
            }
            // convert the hashed byte array back to a string
            return BitConverter.ToString(hashedByteArray).Replace("-", String.Empty).ToLower();
        }

        /// <summary>
        /// Checks the passed in admins credentials to see if they are correct
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns>True if correct, else False</returns>
        public bool AuthorizeAdminUser(string userName, string password)
        {
            return false;
        }
    }
}
