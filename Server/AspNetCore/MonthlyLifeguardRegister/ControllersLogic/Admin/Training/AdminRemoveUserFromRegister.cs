using MonthlyLifeguardRegister.Classess.Database;
using MonthlyLifeguardRegister.Classess.Database.User;
using MonthlyLifeguardRegister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.ControllersLogic.Admin.Training
{
    public class AdminRemoveUserFromRegister
    {
        /// <summary>
        /// Removes the passed in user from the passed in register. returns true if sucsefull, else false
        /// </summary>
        /// <param name="userID">User to remove from register</param>
        /// <param name="registerID">register to remove user from</param>
        /// <param name="appSettings"></param>
        /// <returns>true if sucsefull, else false</returns>
        public bool RemoveUserFromRegister_CreateResponse(int userID, int registerID, AppSettings appSettings)
        {
            return this.RemoveUserFromRegister(userID,registerID,appSettings.sqlConectionStringLocation);
        }


        /// <summary>
        /// Removes the passed in user from the passed in register. returns true if sucsefull, else false
        /// </summary>
        /// <param name="userID">User to remove from register</param>
        /// <param name="registerID">register to remove user from</param>
        /// <param name="sqlLocation">location of databsae on disk</param>
        /// <returns>true if sucsefull, else false</returns>
        private bool RemoveUserFromRegister(int userID, int registerID, string sqlLocation)
        {
            SqLiteConnection sqlCon;
            dbSQLiteUsersInTrainingRegister dbUsersInTrainingRegister;
            bool wasSucsefull = false;

            sqlCon = new SqLiteConnection();
            sqlCon.OpenConnection(sqlLocation);

            dbUsersInTrainingRegister = new dbSQLiteUsersInTrainingRegister(sqlCon);
            wasSucsefull = dbUsersInTrainingRegister.Delete(userID, registerID);

            sqlCon.CloseConnection();

            return wasSucsefull;
        }
    }
}
