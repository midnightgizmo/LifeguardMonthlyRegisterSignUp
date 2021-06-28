using Microsoft.Data.Sqlite;
using MonthlyLifeguardRegister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Classess.Database.User
{
    public class dbSQLiteUsersInTrainingRegister
    {
        private SqLiteConnection _con;

        /// <summary>
        /// Name of the table in the database 
        /// </summary>
        public string tableName = "UsersInTrainingRegister";

        /// <summary>
        /// Inishalized the class ready for interacting with the database.
        /// </summary>
        /// <param name="connection">SQLiteConnection to the database which should have allready been opened before passing in</param>
        public dbSQLiteUsersInTrainingRegister(SqLiteConnection connection)
        {
            this._con = connection;// this should be an allready open connection
        }

      
        #region select methods
        /// <summary>
        /// select 1 UsersInTrainingRegister that match the input
        /// </summary>
        /// <param name="userId">The user id to look for in the database</param>
        /// <param name="trainingRegisterId">the training register id to look for in the database</param>
        /// <returns>an instance of UsersInTrainingRegister with data retreeved from database, or null if could not be found</returns>
        public UsersInTrainingRegister Select(int userId, int trainingRegisterId)
        {
            // holds the data we will get from the database (UsersInTrainingRegister model class)
            UsersInTrainingRegister usersInTrainingRegister = null;

            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT userId, trainingRegisterId ");
            sb.Append($"FROM {this.tableName} ");
            sb.Append("WHERE userId=:userId AND trainingRegisterId = :trainingRegisterId;");


            // holds a list of parameters to insert into the sql query
            List<SqliteParameter> parametersArray = new List<SqliteParameter>();
            SqliteParameter aParameter;

            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":userId";
            aParameter.Value = userId;
            aParameter.DbType = System.Data.DbType.Int32;

            // add the parameter to the parameter array
            parametersArray.Add(aParameter);


            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":trainingRegisterId";
            aParameter.Value = trainingRegisterId;
            aParameter.DbType = System.Data.DbType.Int32;

            // add the parameter to the parameter array
            parametersArray.Add(aParameter);

  

            // execute the sql statment
            SqliteDataReader rdr;
            rdr = this._con.ExecuteParameterizedSelectCommand(sb.ToString(), parametersArray.ToArray());


            // did we get any data from the database
            if (rdr.Read())
            {
                // get the data from the row 
                usersInTrainingRegister = this.GetRowData(rdr);
            }
            rdr.Close();



            // return the row found in the database, or null if not found
            return usersInTrainingRegister;
        }





        /// <summary>
        /// Gets all rows from the table
        /// </summary>
        /// <returns>an array of UsersInTrainingRegister</returns>
        public List<UsersInTrainingRegister> SelectAll()
        {
            // an array of UsersInTrainingRegister classes
            List<UsersInTrainingRegister> usersInTrainingRegisterList = new List<UsersInTrainingRegister>();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT userId, trainingRegisterId");
            sb.Append($"FROM {this.tableName} ");


            // execute the sql statment
            SqliteDataReader rdr;
            rdr = this._con.ExecuteSelectCommand(sb.ToString());

            // get ecah row
            while(rdr.Read())
            {
                UsersInTrainingRegister usersInTrainingRegister;
                // get the row details
                usersInTrainingRegister = this.GetRowData(rdr);

                // add to array
                usersInTrainingRegisterList.Add(usersInTrainingRegister);
            }

            rdr.Close();


            return usersInTrainingRegisterList;
        }


        /// <summary>
        /// Gets all rows where it finds $userID
        /// </summary>
        /// <param name="userId">the id we use to search for</param>
        /// <returns>list of rows that match the userID</returns>
        public List<UsersInTrainingRegister> Select_Where_UserID(int userId)
        {
            // an array of UsersInTrainingRegister classes
            List<UsersInTrainingRegister> usersInTrainingRegisterList = new List<UsersInTrainingRegister>();

            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT userId, trainingRegisterId ");
            sb.Append($"FROM {this.tableName} ");
            sb.Append("WHERE userId = :userId");

            // holds a list of parameters to insert into the sql query
            List<SqliteParameter> parametersArray = new List<SqliteParameter>();
            SqliteParameter aParameter;

            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":userId";
            aParameter.Value = userId;
            aParameter.DbType = System.Data.DbType.Int32;

            // add the parameter to the parameter array
            parametersArray.Add(aParameter);



            // execute the sql statment
            SqliteDataReader rdr;
            rdr = this._con.ExecuteParameterizedSelectCommand(sb.ToString(), parametersArray.ToArray());

            // get ecah row
            while (rdr.Read())
            {
                UsersInTrainingRegister usersInTrainingRegister;
                // get the row details
                usersInTrainingRegister = this.GetRowData(rdr);

                // add to array
                usersInTrainingRegisterList.Add(usersInTrainingRegister);
            }

            rdr.Close();


            return usersInTrainingRegisterList;
        }



        
        /// <summary>
        /// Gets all rows where it finds $trainingRegisterId
        /// </summary>
        /// <param name="trainingRegisterId">the id we use to search for</param>
        /// <returns>list of rows that match the trainingRegisterId</returns>
        public List<UsersInTrainingRegister> Select_Where_TrainingRegisterID(int trainingRegisterId)
        {
            // an array of UsersInTrainingRegister classes
            List<UsersInTrainingRegister> usersInTrainingRegisterList = new List<UsersInTrainingRegister>();

            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT userId, trainingRegisterId ");
            sb.Append($"FROM {this.tableName} ");
            sb.Append("WHERE trainingRegisterId = :trainingRegisterId;");


            // holds a list of parameters to insert into the sql query
            List<SqliteParameter> parametersArray = new List<SqliteParameter>();
            SqliteParameter aParameter;

            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":trainingRegisterId";
            aParameter.Value = trainingRegisterId;
            aParameter.DbType = System.Data.DbType.Int32;

            // add the parameter to the parameter array
            parametersArray.Add(aParameter);



            // execute the sql statment
            SqliteDataReader rdr;
            rdr = this._con.ExecuteParameterizedSelectCommand(sb.ToString(), parametersArray.ToArray());

            // get ecah row
            while (rdr.Read())
            {
                UsersInTrainingRegister usersInTrainingRegister;
                // get the row details
                usersInTrainingRegister = this.GetRowData(rdr);

                // add to array
                usersInTrainingRegisterList.Add(usersInTrainingRegister);
            }

            rdr.Close();


            return usersInTrainingRegisterList;
        }


        #endregion

        #region Insert methods


        /// <summary>
        /// Inserts a new row into the database
        /// </summary>
        /// <param name="userId">Users ID</param>
        /// <param name="trainingRegisterId">Training Registers ID</param>
        /// <returns>an instance of the newly created row or null if insert fails</returns>
        public UsersInTrainingRegister Insert(int userId, int trainingRegisterId)
        {
            // return value;
            UsersInTrainingRegister usersInTrainingRegister = null;
            StringBuilder sb = new StringBuilder();

            sb.Append($"INSERT INTO {this.tableName} ");
            sb.Append("(userId,trainingRegisterId) ");
            sb.Append("VALUES(");
            sb.Append(":userId, :trainingRegisterId");
            sb.Append(");");


            // holds a list of parameters to insert into the sql query
            List<SqliteParameter> parametersArray = new List<SqliteParameter>();
            SqliteParameter aParameter;

            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":userId";
            aParameter.Value = userId;
            aParameter.DbType = System.Data.DbType.Int32;

            // add the parameter to the parameter array
            parametersArray.Add(aParameter);

            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":trainingRegisterId";
            aParameter.Value = trainingRegisterId;
            aParameter.DbType = System.Data.DbType.Int32;

            // add the parameter to the parameter array
            parametersArray.Add(aParameter);


            // execute the sql statment
            int NumRowsEffected;
            NumRowsEffected = this._con.ExecuteParameterizedNoneReader(sb.ToString(), parametersArray.ToArray());

            // if greater than zero the row was inserted
            if (NumRowsEffected > 0)
            {
                // get the details for the database that was just created
                usersInTrainingRegister = this.Select(userId,trainingRegisterId);
            }

            return usersInTrainingRegister;
        }


        #endregion


        #region delete methods


        /// <summary>
        /// Removes the selected row from the database
        /// </summary>
        /// <param name="userId">the User id of the row to be removed</param>
        /// <param name="trainingRegisterId">the Training Register id of the row to be removed</param>
        /// <returns>true if sucsefull, else false</returns>
        public bool Delete(int userId, int trainingRegisterId)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"DELETE FROM { this.tableName} ");
            sb.Append("WHERE userId = :userId AND trainingRegisterId = :trainingRegisterId");



            // holds a list of parameters to insert into the sql query
            List<SqliteParameter> parametersArray = new List<SqliteParameter>();
            SqliteParameter aParameter;

            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":userId";
            aParameter.Value = userId;
            aParameter.DbType = System.Data.DbType.Int32;

            // add the parameter to the parameter array
            parametersArray.Add(aParameter);

            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":trainingRegisterId";
            aParameter.Value = trainingRegisterId;
            aParameter.DbType = System.Data.DbType.Int32;

            // add the parameter to the parameter array
            parametersArray.Add(aParameter);


            // execute the sql statment
            int NumRowsEffected;
            NumRowsEffected = this._con.ExecuteParameterizedNoneReader(sb.ToString(), parametersArray.ToArray());

            // if greater than zero the row was inserted
            if (NumRowsEffected > 0)
                return true;
            else
                return false;

        }



        /// <summary>
        /// removes the user from any register they are currently in
        /// </summary>
        /// <param name="userId">the id of the user to look for</param>
        /// <returns>true if sucsefull, else false</returns>
        public bool DeleteUserFromAllRegisters(int userId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"DELETE FROM {this.tableName} ");
            sb.Append("WHERE userId = :userId;");


            // holds a list of parameters to insert into the sql query
            List<SqliteParameter> parametersArray = new List<SqliteParameter>();
            SqliteParameter aParameter;

            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":userId";
            aParameter.Value = userId;
            aParameter.DbType = System.Data.DbType.Int32;

            // add the parameter to the parameter array
            parametersArray.Add(aParameter);


            // add the parameter to the parameter array
            parametersArray.Add(aParameter);


            // execute the sql statment
            int NumRowsEffected;
            NumRowsEffected = this._con.ExecuteParameterizedNoneReader(sb.ToString(), parametersArray.ToArray());

            // if greater than zero the row was inserted
            if (NumRowsEffected > 0)
                return true;
            else
                return false;
        }


        /// <summary>
        /// removes all users from a single register
        /// </summary>
        /// <param name="registerID">the register id we want to remove all users from</param>
        /// <returns>true if any rows were removed, false if no rows were removed</returns>
        public bool DeleteAllUsersFromRegister(int registerID)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"DELETE FROM {this.tableName} ");
            sb.Append("WHERE trainingRegisterId = :trainingRegisterId;");



            // holds a list of parameters to insert into the sql query
            List<SqliteParameter> parametersArray = new List<SqliteParameter>();
            SqliteParameter aParameter;


            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":trainingRegisterId";
            aParameter.Value = registerID;
            aParameter.DbType = System.Data.DbType.Int32;

            // add the parameter to the parameter array
            parametersArray.Add(aParameter);


            // execute the sql statment
            int NumRowsEffected;
            NumRowsEffected = this._con.ExecuteParameterizedNoneReader(sb.ToString(), parametersArray.ToArray());

            // if greater than zero the row was inserted
            if (NumRowsEffected > 0)
                return true;
            else
                return false;
        }





        #endregion




        /// <summary>
        /// Converts the database row into a <see cref="UserFullDetails"/>
        /// </summary>
        /// <param name="rdr">The database row that contains a users details</param>
        /// <returns></returns>
        private UsersInTrainingRegister GetRowData(SqliteDataReader rdr)
        {
            UsersInTrainingRegister usersInTrainingRegister = new UsersInTrainingRegister();

            usersInTrainingRegister.userId = rdr.GetInt32(0);
            usersInTrainingRegister.trainingRegisterId = rdr.GetInt32(1);

            return usersInTrainingRegister;
        }
    }
}
