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
    public class GetAllRegistersInMonthControllerLogic
    {
        /// <summary>
        /// Gets a list of all training registers with users in that exist within the given year and month
        /// </summary>
        /// <param name="appSettings">settings from the AppSettings file</param>
        /// <param name="year">The year we want to look for registers in</param>
        /// <param name="month">The month we want to look for registers in</param>
        /// <returns>List of registser with users in them</returns>
        public List<TrainingRegisterWithUsers> GetAllRegistersInMonth_CreateLoginResponse(AppSettings appSettings, int year, int month)
        {
            SqLiteConnection sqlCon;
            

            List<TrainingRegister> listOfTrainingRegisters = new List<TrainingRegister>();
            List<TrainingRegisterWithUsers> listofTrainingRegistersWithUsers;

            // create a unix time stamp for the date at the begining of the month
            long startOfMonth = this.ConvertToUnixTimeStamp( this.GetDateAtBeginingOfMonth(year, month) );
            // create a unix time stamp for the date at the end of the month (last day of the month)
            long endOfMonth = this.ConvertToUnixTimeStamp( this.GetDateAtEndOfMonth(year, month) );


            // create a database connection and open it
            sqlCon = new SqLiteConnection();
            sqlCon.openConnection(appSettings.sqlConectionStringLocation);

            
            // get all registers between the start of the month and the end of the month
            listOfTrainingRegisters = this.GetRegisterWithinDates(startOfMonth,endOfMonth,sqlCon);


            // go through each register and populate it with any users it has in it.
            listofTrainingRegistersWithUsers = this.ConvertToListOfRegistersWithUsers(sqlCon, listOfTrainingRegisters);


            // close the sqlite connection
            sqlCon.CloseConnection();
            
            return listofTrainingRegistersWithUsers;
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
        /// Gets a list of training registers that where training falls between the startDate and endDate
        /// </summary>
        /// <param name="startDate">unix time stamp</param>
        /// <param name="endDate">unix time stamp</param>
        /// <param name="sqlCon">an Open Connection to the database</param>
        /// <returns>List of training registers found between the 2 dates</returns>
        private List<TrainingRegister> GetRegisterWithinDates(long startDate, long endDate, SqLiteConnection sqlCon)
        {
            List<TrainingRegister> listOfTrainingRegisters;
            dbSQLiteTrainingRegister dbTrainingRegister;

            dbTrainingRegister = new dbSQLiteTrainingRegister(sqlCon);
            // get all registers between the start of the month and the end of the month
            listOfTrainingRegisters = dbTrainingRegister.select_dateTimeOfTraining_Between(startDate, endDate);

            return listOfTrainingRegisters;
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
    }
}
