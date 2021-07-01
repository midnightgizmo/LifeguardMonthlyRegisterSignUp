using MonthlyLifeguardRegister.Classess.Database;
using MonthlyLifeguardRegister.Classess.Database.User;
using MonthlyLifeguardRegister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.ControllersLogic.Admin.Training
{
    public class AdminAddUserToRegisterControllerLogic
    {
        public bool AddUserToRegister_CreateResponse(int userID, int registerID, AppSettings appSettings)
        {
            SqLiteConnection sqlCon;
            bool wasUserAdded = false;

            sqlCon = new SqLiteConnection();

            sqlCon.OpenConnection(appSettings.sqlConectionStringLocation);

            // check to see if the user is allready in the register we are about to add too
            if(this.DoesUserExistInRegister(userID,registerID,sqlCon) == true)
            {// user is allready in register so we don't want to add them again
                sqlCon.CloseConnection();
                // return false to indicate they were not added.
                return false;
            }

            // try and add the user to the register
            wasUserAdded = this.AddUserToRegister(userID, registerID, sqlCon);

            sqlCon.CloseConnection();


            // return true if was added, else false
            return wasUserAdded;
        }



        /// <summary>
        /// Checks to see if user exists inside the training register. True if they do, else false
        /// </summary>
        /// <param name="userID">the user to look for in the reigster</param>
        /// <param name="RegisterID">the register to look in</param>
        /// <param name="openConnection">an open connection to the database</param>
        /// <returns>returns true if user exists else false</returns>
        private bool DoesUserExistInRegister(int userID, int RegisterID, SqLiteConnection openConnection)
        {
            dbSQLiteUsersInTrainingRegister dbUsersInTrainingRegister;
            UsersInTrainingRegister usersInTrainingRegister;

            dbUsersInTrainingRegister = new dbSQLiteUsersInTrainingRegister(openConnection);
            usersInTrainingRegister = dbUsersInTrainingRegister.Select(userID, RegisterID);

            if (usersInTrainingRegister == null)
                return false;
            else
                return true;
        }


        /// <summary>
        /// Attempts to add the user to the register. Returns true if added, else false
        /// </summary>
        /// <param name="userID">User to add to register</param>
        /// <param name="registerID">Register to add user too</param>
        /// <param name="openConnection">An open connection to the database</param>
        /// <returns></returns>
        private bool AddUserToRegister(int userID, int registerID, SqLiteConnection openConnection)
        {
            dbSQLiteUsersInTrainingRegister dbUsersInTrainingRegister;
            UsersInTrainingRegister usersInTrainingRegister;

            dbUsersInTrainingRegister = new dbSQLiteUsersInTrainingRegister(openConnection);
            usersInTrainingRegister = dbUsersInTrainingRegister.Insert(userID, registerID);

            if (usersInTrainingRegister == null)
                return false;
            else
                return true;

        }


    }
}
