using MonthlyLifeguardRegister.Classess.Database;
using MonthlyLifeguardRegister.Classess.Database.Training;
using MonthlyLifeguardRegister.Classess.Database.User;
using MonthlyLifeguardRegister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.ControllersLogic.Admin.Training
{
    public class AdminDeleteRegisterControllerLogic
    {
        /// <summary>
        /// Atempts to remove the register and all its users from the database. Returns true if sucsesfull
        /// </summary>
        /// <param name="registerID">The register to remove</param>
        /// <param name="appSettings"></param>
        /// <returns></returns>
        public bool DeleteRegister_CreateResponse(int registerID, AppSettings appSettings)
        {
            // remove any user from the register
            this.RemoveUsersFromRegister(registerID, appSettings.sqlConectionStringLocation);
            // delete the register
            return this.DeleteRegister(registerID, appSettings.sqlConectionStringLocation);
        }

        /// <summary>
        /// Removes any users within the specified register
        /// </summary>
        /// <param name="registerID">the register to remove users from </param>
        /// <param name="sqlConectionStringLocation">location of the database on disk</param>
        private void RemoveUsersFromRegister(int registerID, string sqlConectionStringLocation)
        {
            SqLiteConnection sqlCon;
            dbSQLiteUsersInTrainingRegister dbUserInTrainingRegister;

            sqlCon = new SqLiteConnection();
            sqlCon.OpenConnection(sqlConectionStringLocation);

            dbUserInTrainingRegister = new dbSQLiteUsersInTrainingRegister(sqlCon);
            dbUserInTrainingRegister.DeleteAllUsersFromRegister(registerID);

            sqlCon.CloseConnection();
        }

        /// <summary>
        /// Atempts to delete the register from the database. returns true if sucsefull
        /// </summary>
        /// <param name="registerID">The reigister to delete</param>
        /// <param name="sqlConectionStringLocation">location of the database on disk</param>
        /// <returns></returns>
        private bool DeleteRegister(int registerID, string sqlConectionStringLocation)
        {
            SqLiteConnection sqlCon;
            dbSQLiteTrainingRegister dbTrainingRegister;
            bool WasRegisterDeleted = false;

            sqlCon = new SqLiteConnection();
            sqlCon.OpenConnection(sqlConectionStringLocation);

            dbTrainingRegister = new dbSQLiteTrainingRegister(sqlCon);
            WasRegisterDeleted = dbTrainingRegister.Delete(registerID);

            sqlCon.CloseConnection();

            return WasRegisterDeleted;
        }
    }
}
