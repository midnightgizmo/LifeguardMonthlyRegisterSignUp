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
    public class AdminGetAllActiveUsersControllerLogic
    {
        /// <summary>
        /// Gets all users from the database who have been set as active
        /// </summary>
        /// <param name="appSettings">Used to get the database location</param>
        /// <returns>List of active users</returns>
        public List<UserFullDetails> GetAllActiveUsers_CreateResponse(AppSettings appSettings)
        {
            return this.GetAllActiveUsers(appSettings.sqlConectionStringLocation);
        }

        /// <summary>
        /// Gets all active users from the database
        /// </summary>
        /// <param name="ConnectionString">physical location of database on disk</param>
        /// <returns></returns>
        public List<UserFullDetails> GetAllActiveUsers(string ConnectionStringLocation)
        {
            SqLiteConnection sqlCon;
            dbSQLiteUser dbUser;
            List<UserFullDetails> userFullDetailsList;

            sqlCon = new SqLiteConnection();
            sqlCon.OpenConnection(ConnectionStringLocation);

            dbUser = new dbSQLiteUser(sqlCon);
            userFullDetailsList = dbUser.SelectAllActiveUsers();

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
