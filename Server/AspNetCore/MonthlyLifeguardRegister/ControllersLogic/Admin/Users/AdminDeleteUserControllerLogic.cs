using MonthlyLifeguardRegister.Classess.Database;
using MonthlyLifeguardRegister.Classess.Database.User;
using MonthlyLifeguardRegister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.ControllersLogic.Admin.Users
{
    public class AdminDeleteUserControllerLogic
    {
        /// <summary>
        /// Removes the user from the database, including any registers they were in
        /// </summary>
        /// <param name="userID">person to remove</param>
        /// <param name="appSettings">needed for the location to the database</param>
        /// <returns></returns>
        public bool DeleteUser_CreateResponse(int userID, AppSettings appSettings)
        {
            SqLiteConnection sqlCon;
            bool wasUserDeleted = false;

            sqlCon = new SqLiteConnection();
            sqlCon.OpenConnection(appSettings.sqlConectionStringLocation);

            // remove user from all registers
            this.RemoveUserFromAllRegisters(userID, sqlCon);

            // delete the user
            wasUserDeleted = this.RemoveUserFromDatabase(userID, sqlCon);

            sqlCon.CloseConnection();

            return wasUserDeleted;
        }

  
        /// <summary>
        /// Removes the user from any reigister they are curreently in
        /// </summary>
        /// <param name="userID">user to remove</param>
        /// <param name="sqlCon">Open connection to the database</param>
        private void RemoveUserFromAllRegisters(int userID, SqLiteConnection sqlCon)
        {
            dbSQLiteUsersInTrainingRegister dbUserInTrainingRegister;

            dbUserInTrainingRegister = new dbSQLiteUsersInTrainingRegister(sqlCon);

            dbUserInTrainingRegister.DeleteUserFromAllRegisters(userID);

            return;
        }

        /// <summary>
        /// Atempts to remove the user from the database, returns true if sucsefull
        /// </summary>
        /// <param name="userID">user to remove</param>
        /// <param name="sqlCon">open connection to the database</param>
        /// <returns>returns true if sucsefull</returns>
        private bool RemoveUserFromDatabase(int userID, SqLiteConnection sqlCon)
        {
            dbSQLiteUser dbUser;

            dbUser = new dbSQLiteUser(sqlCon);
            return dbUser.Delete(userID);
        }
    }
}
