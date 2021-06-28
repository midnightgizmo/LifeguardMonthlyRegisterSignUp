using MonthlyLifeguardRegister.Classess.Database;
using MonthlyLifeguardRegister.Classess.Database.User;
using MonthlyLifeguardRegister.Models;
using MonthlyLifeguardRegister.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.ControllersLogic.Training
{
    public class RemoveUserFromRegisterControllerLogic
    {
        public JwtUser RemoveUserFromRegister_CreateResponse(JwtUser currentUserInfo, int dataSentFromClient_UserID, int dataSentFromClient_RegisterID, AppSettings appSettings)
        {
            JwtUser jwtUser;
            SqLiteConnection sqlCon;


            // make sure the userID input parameter that was sent from the client is the same userID 
            // of the logged in user (a user can not add another user to the register, they can only add them self)
            if (this.IsUserRemovingThemSelfToRegister(currentUserInfo.id, dataSentFromClient_UserID) == false)
                return this.CreateEmptyUser();



            sqlCon = new SqLiteConnection();
            sqlCon.OpenConnection(appSettings.sqlConectionStringLocation);


            // try and find the user in the database we want to add to the register
            jwtUser = this.FindUser(dataSentFromClient_UserID, sqlCon);
            // if we could not find the user, send back an empty user to let client know somthing was not write
            if (jwtUser == null)
                return this.CreateEmptyUser();


            // remove the user from the table in the database
            if (this.RemoveUserFromRegister(dataSentFromClient_UserID, dataSentFromClient_RegisterID, sqlCon) == false)
                return this.CreateEmptyUser();

            // close the database connection
            sqlCon.CloseConnection();


            // user has been removed so return user that was removed
            return jwtUser;
        }





        /// <summary>
        /// Checks the passed in ids to see if they match. return true if they do, else false
        /// </summary>
        /// <param name="loggedInUserId">The id of the person makeing the request to this server backend</param>
        /// <param name="userIdSentFromClientParameter">The id of the user the person wants to add to the register</param>
        /// <returns>true if they are both the same id</returns>
        private bool IsUserRemovingThemSelfToRegister(int loggedInUserId, int userIdSentFromClientParameter)
        {
            // if the ids dont match, return false. if they do match return true
            return loggedInUserId != userIdSentFromClientParameter ? false : true;

        }




        /// <summary>
        /// Trys to find users details in the database baed on the userID passed in. Returns null if not found
        /// </summary>
        /// <param name="userID">if of the user we are looking for</param>
        /// <param name="sqlCon">an open connection to the database</param>
        /// <returns>returns users details or null if not found</returns>
        private JwtUser FindUser(int userID, SqLiteConnection sqlCon)
        {

            dbSQLiteUser dbUser = new dbSQLiteUser(sqlCon);
            UserFullDetails userFullDetails = null;

            // try and find the users details.
            userFullDetails = dbUser.Select(userID);

            // if we could not find the users details
            if (userFullDetails == null)
                return null;

            // return the users details as a JwtUser as we don't need all the details
            return userFullDetails;
        }



        /// <summary>
        /// Atempts to remove the user from the register.
        /// </summary>
        /// <param name="dataSentFromClient_UserID">User to remove from register</param>
        /// <param name="dataSentFromClient_RegisterID">Register to remove user from</param>
        /// <param name="sqlCon">Open connection to the database</param>
        /// <returns>Returns true if user removed from register, else false</returns>
        private bool RemoveUserFromRegister(int dataSentFromClient_UserID, int dataSentFromClient_RegisterID, SqLiteConnection sqlCon)
        {
            dbSQLiteUsersInTrainingRegister dbTrainingRegister;

            dbTrainingRegister = new dbSQLiteUsersInTrainingRegister(sqlCon);

            // return true if deleted, else false
            return dbTrainingRegister.Delete(dataSentFromClient_UserID, dataSentFromClient_RegisterID);
        }


        /// <summary>
        /// Creates an empty JwtUser with its id set to -1
        /// </summary>
        /// <returns></returns>
        private JwtUser CreateEmptyUser()
        {
            JwtUser jwtUser = new JwtUser();
            jwtUser.id = -1;
            jwtUser.firstName = "";
            jwtUser.surname = "";

            return jwtUser;
        }






    }
}
