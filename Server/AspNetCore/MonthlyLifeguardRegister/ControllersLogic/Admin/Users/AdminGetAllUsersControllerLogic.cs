using MonthlyLifeguardRegister.Classess.Database;
using MonthlyLifeguardRegister.Classess.Database.User;
using MonthlyLifeguardRegister.Models;
using MonthlyLifeguardRegister.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.ControllersLogic.Admin.Users
{
    public class AdminGetAllUsersControllerLogic
    {
        /// <summary>
        /// Gets a list of all users in the database (active and inactive)
        /// </summary>
        /// <param name="appSettings">used to get the database location</param>
        /// <returns></returns>
        public List<UserFullDetails> GetAllUsers_CreateResponse(AppSettings appSettings)
        {
            return this.GetAllUsers(appSettings.sqlConectionStringLocation);
        }

        /// <summary>
        /// Gets a list of all users in the database (active and inactive)
        /// </summary>
        /// <param name="sqlConectionStringLocation">location of the database on disk</param>
        /// <returns></returns>
        private List<UserFullDetails> GetAllUsers(string sqlConectionStringLocation)
        {
            SqLiteConnection sqlCon;
            dbSQLiteUser dbUser;
            List<UserFullDetails> userFullDetailsList;

            sqlCon = new SqLiteConnection();
            sqlCon.OpenConnection(sqlConectionStringLocation);

            dbUser = new dbSQLiteUser(sqlCon);
            userFullDetailsList = dbUser.SelectAll();

            sqlCon.CloseConnection();

            // goes through each users and sets the password property to string.Empty
            this.RemovePassword(userFullDetailsList);

            return userFullDetailsList;
        }


        /// <summary>
        /// Removes the value from the password property of each user
        /// </summary>
        /// <param name="userFullDetailsList"></param>
        private void RemovePassword(List<UserFullDetails> userFullDetailsList)
        {
            // go through each user and set the password property to string.Empty
            foreach (UserFullDetails aUser in userFullDetailsList)
                aUser.password = string.Empty;
        }

    }
}
