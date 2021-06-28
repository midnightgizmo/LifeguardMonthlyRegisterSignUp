using MonthlyLifeguardRegister.Classess.Database;
using MonthlyLifeguardRegister.Classess.Database.Training;
using MonthlyLifeguardRegister.Classess.Database.User;
using MonthlyLifeguardRegister.Models;
using MonthlyLifeguardRegister.Models.Training;
using MonthlyLifeguardRegister.Models.User;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.ControllersLogic.Training
{
    public class AddUserToRegisterControllerLogic
    {
        /// <summary>
        /// Adds the passed in user to the passed in register (as long as they pass all the checks).
        /// </summary>
        /// <param name="currentUserInfo">Used to get the user id of the person making the request to the server</param>
        /// <param name="dataSentFromClient_UserID">ID of the user to add to register (must be same as the id of the person making the call to the server sotred in json web token)</param>
        /// <param name="dataSentFromClient_RegisterID">Register to add user too</param>
        /// <param name="appSettings">used to get sql connection string</param>
        /// <returns>Returns the user that was added to the register if sucsefull, else a user with id set to -1 if was not added</returns>
        public JwtUser AddUserToRegister_CreateResponse(JwtUser currentUserInfo, int dataSentFromClient_UserID, int dataSentFromClient_RegisterID, AppSettings appSettings)
        {
            JwtUser jwtUser;
            SqLiteConnection sqlCon;
            // the register to add the user too
            TrainingRegister trainingRegister;
            // all the registers in the given month
            List<TrainingRegister> allRegisterInMonthList;
            // all the registers in the given month with user
            List<TrainingRegisterWithUsers> registersWithUsersList;



            // make sure the userID input parameter that was sent from the client is the same userID 
            // of the logged in user (a user can not add another user to the register, they can only add them self)
            if ( this.IsUserAddingThemSelfToRegister(currentUserInfo.id, dataSentFromClient_UserID) == false)
                return this.CreateEmptyUser();



            sqlCon = new SqLiteConnection();
            sqlCon.OpenConnection(appSettings.sqlConectionStringLocation);




            // try and find the user in the database we want to add to the register
            jwtUser = this.FindUser(dataSentFromClient_UserID, sqlCon);
            // if we could not find the user, send back an empty user to let client know somthing was not write
            if (jwtUser == null)
                return this.CreateEmptyUser();



            // try and find the register we want to add the user too.
            trainingRegister = this.GetRegisterDetails(dataSentFromClient_RegisterID, sqlCon);
            // if we could not find the register, send back an empty user to let the client know somthing was not write
            if (trainingRegister == null)
                return this.CreateEmptyUser();

            // get the year and month of the training register. (this is so we can get all registers in that month
            (int Year, int Month) YearAndMonth = this.GetYearAndMonthFromUnixTimeStamp(trainingRegister.dateTimeOfTraining);


            // find all register in the month of the register the user wants to be added too.
            allRegisterInMonthList = this.FindAllRegistersInMonth(YearAndMonth.Year, YearAndMonth.Month, sqlCon);


            // go through each register and populate it with any users it has in it.
            registersWithUsersList = this.ConvertToListOfRegistersWithUsers(sqlCon,allRegisterInMonthList);


            // does user exist in any of the registers. If they do, they can't be added to the register
            // because user can only be present in one of the registers in any given month
            if (this.DoesUserExistInRegisters(registersWithUsersList, currentUserInfo.id) == true)
                return this.CreateEmptyUser();


            // is the user allowed to add them self to this register (make sure dates are corrrect for editing register)
            if (this.AreDatesOkForUserToBeAddedToRegister(registersWithUsersList, trainingRegister) == false)
                return this.CreateEmptyUser();


            // make sure there is space on the register for the user to be added to it
            if (this.isThereEnoughSpaceOnRegister(registersWithUsersList.First(r => r.id == trainingRegister.id)) == false)
                return this.CreateEmptyUser();

            // add the user to the register
            if (this.AddUserToRegister(trainingRegister.id, dataSentFromClient_UserID, sqlCon) == false)
                return this.CreateEmptyUser();

            // close the database connection
            sqlCon.CloseConnection();

            // if we get this far the user was added to the register. send back the user that was added
            // to indicate they were added.
            return jwtUser;
        }

     

        /// <summary>
        /// Checks the passed in ids to see if they match. return true if they do, else false
        /// </summary>
        /// <param name="loggedInUserId">The id of the person makeing the request to this server backend</param>
        /// <param name="userIdSentFromClientParameter">The id of the user the person wants to add to the register</param>
        /// <returns>true if they are both the same id</returns>
        private bool IsUserAddingThemSelfToRegister(int loggedInUserId, int userIdSentFromClientParameter)
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
        /// Trys to get the training register for the passed in registerID. returns null if not found.
        /// </summary>
        /// <param name="registerID">ID of the register to look for</param>
        /// <param name="sqlCon">An open connection to the database</param>
        /// <returns>returns training register model or null if not found</returns>
        private TrainingRegister GetRegisterDetails(int registerID, SqLiteConnection sqlCon)
        {
            dbSQLiteTrainingRegister dbTrainingRegister = new dbSQLiteTrainingRegister(sqlCon);
            TrainingRegister trainingRegister;

            trainingRegister = dbTrainingRegister.Select(registerID);

            return trainingRegister;
        }





        /// <summary>
        /// Gets the Year and month from the passed in unix time stamp
        /// </summary>
        /// <param name="dateTimeOfTraining">unix time stamp</param>
        /// <returns></returns>
        private (int, int) GetYearAndMonthFromUnixTimeStamp(long dateTimeOfTraining)
        {
            int year, month;

            System.DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            date = date.AddSeconds(dateTimeOfTraining).ToLocalTime();
            year = date.Year;
            month = date.Month;

            return (year, month);
        }



        /// <summary>
        /// Finds all training registers for the given month
        /// </summary>
        /// <param name="year">The year we are looking for training registers</param>
        /// <param name="month">The month we are looking for training registers</param>
        /// <param name="sqlCon">Open connection to the database</param>
        /// <returns>List of training registers for the given month</returns>
        private List<TrainingRegister> FindAllRegistersInMonth(int year, int month, SqLiteConnection sqlCon)
        {
            List<TrainingRegister> trainingRegistersInMonthList;
            // will hold unix time stamps
            long dateAtBeginingOfMonth, dateAtEndOfMonth;
            dbSQLiteTrainingRegister dbTrainingRegister;

            // get the unix time stamp for the date at the begining of the month
            dateAtBeginingOfMonth = this.ConvertToUnixTimeStamp( this.GetDateAtBeginingOfMonth(year, month) );
            // get the unix time stamp for the date at the end of the month
            dateAtEndOfMonth = this.ConvertToUnixTimeStamp( this.GetDateAtEndOfMonth(year, month) );


            dbTrainingRegister = new dbSQLiteTrainingRegister(sqlCon);
            // get all the training register that are running in the given month
            trainingRegistersInMonthList = dbTrainingRegister.Select_DateTimeOfTraining_Between(dateAtBeginingOfMonth, dateAtEndOfMonth);


            // return the traning registers list
            return trainingRegistersInMonthList;
        }




        /// <summary>
        /// Creates a date at the begining of the month based on the year and month passed in
        /// </summary>
        /// <param name="year">full year</param>
        /// <param name="month">Month starting at 1</param>
        /// <returns>DateTime object set to the begining of the month for the chossen year, month</returns>
        private DateTime GetDateAtBeginingOfMonth(int year, int month)
        {
            // set the culture to uk
            CultureInfo en = new CultureInfo("en-GB");
            // the format the date will be in when it gets converted to a DateTime
            string dateFormat = "dd/MM/yyyy hh:mm:ss";
            // create the date as a string
            string dateValue = $"01/{month.ToString("00")}/{year} 00:00:00";

            // parse the string into a DateTime object
            DateTime date = DateTime.ParseExact(dateValue, dateFormat, en);

            return date;
        }

        /// <summary>
        /// Creates a Date at the end of the month based on the year and month passed in
        /// </summary>
        /// <param name="year">full year</param>
        /// <param name="month">Month starting at 1</param>
        /// <returns>DateTime object set to the end of the month for the chosen year, month</returns>
        private DateTime GetDateAtEndOfMonth(int year, int month)
        {
            // find how many days are in the month
            int daysInMonth = DateTime.DaysInMonth(year, month);
            // create a new DateTime for the last day of the month
            return new DateTime(year, month, daysInMonth);
        }

        /// <summary>
        /// convert the DateTime object to a unix time stamp
        /// </summary>
        /// <param name="date">date to convert to unix time stamp</param>
        /// <returns>unix time stamp</returns>
        private long ConvertToUnixTimeStamp(DateTime date)
        {
            return ((DateTimeOffset)date).ToUnixTimeSeconds();
        }





        /// <summary>
        /// gets the users within each register and converts each register to a <see cref="TrainingRegisterWithUsers"/> so it can hold the users
        /// </summary>
        /// <param name="sqlCon">an open sql connection</param>
        /// <param name="listOfTrainingRegisters">list of register where we will look to see if they have any users</param>
        /// <returns>list of register with any users (if any have been put on the registers)</returns>
        private List<TrainingRegisterWithUsers> ConvertToListOfRegistersWithUsers(SqLiteConnection sqlCon, List<TrainingRegister> listOfTrainingRegisters)
        {
            // the value we will be returning, a list of registers with users in them
            List<TrainingRegisterWithUsers> trainingRegisterWithUsersList = new List<TrainingRegisterWithUsers>();

            // go through each register
            foreach (TrainingRegister aRegister in listOfTrainingRegisters)
            {
                // find all users within the current register
                List<JwtUser> usersList = this.GetUsersInRegister(aRegister.id, sqlCon);

                // crate a register than can hold users
                TrainingRegisterWithUsers registerWithUsers = new TrainingRegisterWithUsers();
                // add the users to the register
                registerWithUsers.usersInTrainingSessionArray = usersList;
                // copy the details from the register we got from the foreach loop into the new register that can hold users.
                registerWithUsers.id = aRegister.id;
                registerWithUsers.dateTimeFromWhenRegisterCanBeUsed = aRegister.dateTimeFromWhenRegisterCanBeUsed;
                registerWithUsers.dateTimeOfTraining = aRegister.dateTimeOfTraining;
                registerWithUsers.dateTimeWhenRegisterIsActive = aRegister.dateTimeWhenRegisterIsActive;
                registerWithUsers.maxNoCandidatesAllowed = aRegister.maxNoCandidatesAllowed;

                // add the user to the register
                trainingRegisterWithUsersList.Add(registerWithUsers);
            }

            // return a list of registers with users in them.
            return trainingRegisterWithUsersList;
        }


        /// <summary>
        /// Gets a list of all users for the passed in registerID
        /// </summary>
        /// <param name="registerID">The register to look in to find the users</param>
        /// <param name="openConnection">a connection to the database that has allready been opened</param>
        /// <returns>List of user in register</returns>
        private List<JwtUser> GetUsersInRegister(int registerID, SqLiteConnection openConnection)
        {
            dbSQLiteUser dbUser;
            List<JwtUser> userFullDetailsList;
            dbUser = new dbSQLiteUser(openConnection);

            // get all users that are currently in the given register
            userFullDetailsList = dbUser.SelectUsersInRegister(registerID);

            return userFullDetailsList;
        }


        /// <summary>
        /// Checks the the passed in userID exists in any of the registers passed in
        /// </summary>
        /// <param name="registersWithUsersList">The registers to look through to see if user is in any of them</param>
        /// <param name="userID">The user to look for in the registers passed in</param>
        /// <returns>returns true if user found in registes, otherwise false</returns>
        private bool DoesUserExistInRegisters(List<TrainingRegisterWithUsers> registersWithUsersList, int userID)
        {
            // go through each register
            foreach (TrainingRegisterWithUsers aRegister in registersWithUsersList) 
            {
                // go through each user that is in the register
                foreach (JwtUser eachUser in aRegister.usersInTrainingSessionArray)
                {
                    // see if the user wanting to add them self to one of the registers
                    // in the given month exists in this register.
                    // If they do exist, it means the user can not be added to any register
                    // in the month because a user can only appear on one register in any 
                    // given month
                    if (eachUser.id == userID)
                    {
                        // user has been found in one of the registers so return true
                        return true;
                    }
                }
            }

            // user has not been found in any of the registers, so return false;
            return false;


        }

        /// <summary>
        /// Checks dates to see if this register can be accessed.
        /// </summary>
        /// <param name="registersWithUsersList">All registers in the month</param>
        /// <param name="trainingRegister">The regiseter we want to add too</param>
        /// <returns>True if dates are ok, else false</returns>
        private bool AreDatesOkForUserToBeAddedToRegister(List<TrainingRegisterWithUsers> registersWithUsersList, TrainingRegister trainingRegister)
        {
            DateTime UtcdateTimeNow;
            DateTime ukDateTime;
            long DateTimeNow_UnixTimeStamp;

            // get the current date time as a UTC DateTime
            UtcdateTimeNow = DateTime.UtcNow;
            // convert the UTC time to UK Time
            ukDateTime = this.ConvertUtcTimeToBritishStandardTime(UtcdateTimeNow);
            // convert the ukDateTime to a unix time stamp
            DateTimeNow_UnixTimeStamp  = this.ConvertToUnixTimeStamp(ukDateTime);

            // get the first register in the month and covert it from Unix time stamp to DateTime and then to UK Time
            //DateTime dateOfFirstRegister = this.ConvertUtcTimeToBritishStandardTime(this.ConvertUnixTimeStampToDateTime(registersWithUsersList[0].dateTimeOfTraining));
            DateTime dateOfFirstRegister = this.ConvertUnixTimeStampToDateTime(registersWithUsersList[0].dateTimeOfTraining);
            // take 5 days off the date of first register to give us the date from when registers can no longer be editable
            DateTime dateWhenAllRegistersInMonthAreNoLongerEditable = dateOfFirstRegister.Subtract(new TimeSpan(5, 0, 0, 0));

            // don't allow user to add or remove them self from this months register if todays date
            // is greater than the first register in the list, minus 5 days
            if (ukDateTime >= dateWhenAllRegistersInMonthAreNoLongerEditable)
                return false;

            // if we have set the editable date to the future (past todays date) the user can't edit it
            if (trainingRegister.dateTimeFromWhenRegisterCanBeUsed > DateTimeNow_UnixTimeStamp)
                return false;

            // if the start date for the register has pased todays date, they can't edit it
            if (trainingRegister.dateTimeOfTraining < DateTimeNow_UnixTimeStamp)
                return false;

            // dates ok for user to be added to register.
            return true;


        }

        /// <summary>
        /// Checks to make sure the register is not allready full
        /// </summary>
        /// <param name="trainingRegister">The register to add too</param>
        /// <returns>True if there is enough space to add another person, else false</returns>
        private bool isThereEnoughSpaceOnRegister(TrainingRegisterWithUsers trainingRegister)
        {
            if (trainingRegister.usersInTrainingSessionArray.Count < trainingRegister.maxNoCandidatesAllowed)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Attempts to add the user to the register
        /// </summary>
        /// <param name="registerID">Register to add user too</param>
        /// <param name="UserID">User to add to register</param>
        /// <param name="openConnection">An open connection to the database</param>
        /// <returns>returns true if user has been added to register, else false</returns>
        private bool AddUserToRegister(int registerID, int UserID, SqLiteConnection openConnection)
        {
            dbSQLiteUsersInTrainingRegister dbTrainingRegister;

            dbTrainingRegister = new dbSQLiteUsersInTrainingRegister(openConnection);

            if (dbTrainingRegister.Insert(UserID, registerID) != null)
                return true;
            else
                return false;
            
        }




        /// <summary>
        /// Converts a unixtime stamp to DateTime
        /// </summary>
        /// <param name="unixTimeStamp">the unix time stamp to convert</param>
        /// <returns></returns>
        private DateTime ConvertUnixTimeStampToDateTime(long unixTimeStamp)
        {
            System.DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            date = date.AddSeconds(unixTimeStamp).ToLocalTime();

            return date;
        }

        /// <summary>
        /// Converts a UTC DateTime to a uk DateTime
        /// </summary>
        /// <param name="UtcDateTime"></param>
        /// <returns></returns>
        private DateTime ConvertUtcTimeToBritishStandardTime(DateTime UtcDateTime)
        {
            TimeZoneInfo ukTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

            DateTime convertedDate = DateTime.SpecifyKind(UtcDateTime, DateTimeKind.Utc);

            DateTime ukDateTime = TimeZoneInfo.ConvertTimeFromUtc(convertedDate, ukTimeZone);

            return ukDateTime;
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
