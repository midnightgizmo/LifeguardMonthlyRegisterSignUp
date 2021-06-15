using MonthlyLifeguardRegister.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Classess.Security
{
    /// <summary>
    /// Checks the passed in credentials to see if they are correct
    /// </summary>
    public class Authorize
    {

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
            // need to populate this with the user details we get from the database
            userFullDetails = new UserFullDetails();
            return true;
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
