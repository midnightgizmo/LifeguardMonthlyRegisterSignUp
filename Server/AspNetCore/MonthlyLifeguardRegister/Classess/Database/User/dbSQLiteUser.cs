using Microsoft.Data.Sqlite;
using MonthlyLifeguardRegister.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Classess.Database.User
{
    /// <summary>
    /// CRUD methods for <see cref="UserFullDetails"/> in database
    /// </summary>
    public class dbSQLiteUser
    {
        private SqLiteConnection _con;

        /// <summary>
        /// Name of the table in the database 
        /// </summary>
        public string tableName = "User";

        /// <summary>
        /// Inishalized the class ready for interacting with the database.
        /// </summary>
        /// <param name="connection">SQLiteConnection to the database which should have allready been opened before passing in</param>
        public dbSQLiteUser(SqLiteConnection connection)
        {
            this._con = connection;// this should be an allready open connection
        }


        #region select functions
        

        /// <summary>
        /// select 1 User from the database
        /// </summary>
        /// <param name="id">the id of the User to look for</param>
        /// <returns>User an instance of User with data retreeved from database, or null if could not be found</returns>
        public UserFullDetails select(int id)
        {
            UserFullDetails userFullDetails = null;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT id, firstName, surname, password, isUserActive ");
            sb.Append($"FROM {this.tableName} ");
            sb.Append("WHERE id=:id;");

            SqliteParameter sqliteParameter = new SqliteParameter();
            sqliteParameter.ParameterName = ":id";
            sqliteParameter.Value = id;
            sqliteParameter.DbType = System.Data.DbType.Int32;

            SqliteDataReader rdr;
            rdr = this._con.ExecuteParameterizedSelectCommand(sb.ToString(), new SqliteParameter[] { sqliteParameter });

            if(rdr.Read())
            {
                userFullDetails = this.GetRowData(rdr);
            }
            rdr.Close();

            // retuns the requested user found in the database, or null if not found
            return userFullDetails;
        }

        /// <summary>
        /// selects all rows from the User table in the database and returns them as an array of User
        /// </summary>
        /// <returns>list of all User in database</returns>
        public List<UserFullDetails> selectAll()
        {

            // an array of User classes
            List<UserFullDetails> usersList = new List<UserFullDetails>();
        
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT id, firstName, surname, password, isUserActive ");
            sb.Append($"FROM {this.tableName} ");



            // execute the sql statment
            SqliteDataReader rdr;
            rdr = this._con.ExecuteSelectCommand(sb.ToString());
        
            // get each row
            while (rdr.Read()) 
            {
                // get the data from the row and add it to a Models.User class
                UserFullDetails aUser = this.GetRowData(rdr);
                // add the model class to the array
                usersList.Add(aUser);
            }
            rdr.Close();


            // a list of Models.User
            return usersList;

        }

        /// <summary>
        /// selects all rows from the User table in the database and returns them as an array of User
        /// </summary>
        /// <returns>list of all User in database</returns>
        public List<UserFullDetails> selectAllActiveUsers()
        {

            // an array of User classes
            List<UserFullDetails> usersList = new List<UserFullDetails>();

            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT id, firstName, surname, password, isUserActive ");
            sb.Append($"FROM {this.tableName} ");
            sb.Append("WHERE isUserActive=1;");

            // execute the sql statment
            SqliteDataReader rdr;
            rdr = this._con.ExecuteSelectCommand(sb.ToString());


            // get each row
            while (rdr.Read()) 
            {
                // get the data from the row and add it to a Models.User class
                UserFullDetails aUser = this.GetRowData(rdr);
                // add the model class to the array
                usersList.Add(aUser);
            }
            rdr.Close();


            // a list of Models.User
            return usersList;
            
        }

        /// <summary>
        /// gets the fist instance of a match for firstName and password
        /// </summary>
        /// <param name="firstName">persons first name for who to look for</param>
        /// <param name="password">the hashed password to look for</param>
        /// <returns>user object if found or null if not</returns>
        public UserFullDetails select_By_FirstNameAndPassword(string firstName, string password)
        {

            // found user, or null
            UserFullDetails usersInfo = null;

            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT id, firstName, surname, password, isUserActive ");
            sb.Append($"FROM {this.tableName} ");
            sb.Append("WHERE firstName = :firstName COLLATE NOCASE AND password = :password");


            List<SqliteParameter> parametersArray = new List<SqliteParameter>();
            SqliteParameter aParameter;
            
            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":firstName";
            aParameter.Value = firstName;
            aParameter.DbType = System.Data.DbType.String;

            parametersArray.Add(aParameter);


            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":password";
            aParameter.Value = password;
            aParameter.DbType = System.Data.DbType.String;

            parametersArray.Add(aParameter);


            // execute the sql statment
            SqliteDataReader rdr;
            rdr = this._con.ExecuteParameterizedSelectCommand(sb.ToString(), parametersArray.ToArray());
        

            // did we get any data from the database
            if(rdr.Read()) 
            {
                // get the data from the row and add it to a Models.User class
                usersInfo = this.GetRowData(rdr);
            }
            rdr.Close();




            // return the found user or null
            return usersInfo;
            
        }

        /// <summary>
        /// gets the fist instance of a match for surname and password
        /// </summary>
        /// <param name="surname">persons surname for who to look for</param>
        /// <param name="password">The hashed password to look for</param>
        /// <returns>user object if found or null if not</returns>
        public UserFullDetails select_By_SurnameAndPassword(string surname, string password)
        {

            // found user, or null
            UserFullDetails usersInfo = null;

            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT id, firstName, surname, password, isUserActive ");
            sb.Append($"FROM {this.tableName} ");
            sb.Append("WHERE surname = :surname COLLATE NOCASE AND password = :password");


            List<SqliteParameter> parametersArray = new List<SqliteParameter>();
            SqliteParameter aParameter;

            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":surname";
            aParameter.Value = surname;
            aParameter.DbType = System.Data.DbType.String;

            parametersArray.Add(aParameter);


            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":password";
            aParameter.Value = password;
            aParameter.DbType = System.Data.DbType.String;

            parametersArray.Add(aParameter);


            // execute the sql statment
            SqliteDataReader rdr;
            rdr = this._con.ExecuteParameterizedSelectCommand(sb.ToString(), parametersArray.ToArray());


            // did we get any data from the database
            if (rdr.Read())
            {
                // get the data from the row and add it to a Models.User class
                usersInfo = this.GetRowData(rdr);
            }
            rdr.Close();




            // return the found user or null
            return usersInfo;
        }

        /// <summary>
        /// Looks for a list of user matching the first name and surname
        /// </summary>
        /// <param name="firstName">users first name to look for</param>
        /// <param name="surname">users surname to look for</param>
        /// <returns>List of users that matched the input</returns>
        public List<UserFullDetails> select_By_FirstNameAndSurname(string firstName, string surname)
        {

            // will hold all users that match the query
            List<UserFullDetails> usersList = new List<UserFullDetails>();
            

            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT id, firstName, surname, password, isUserActive ");
            sb.Append($"FROM {this.tableName} ");
            sb.Append("WHERE surname = :surname AND password = :password");


            List<SqliteParameter> parametersArray = new List<SqliteParameter>();
            SqliteParameter aParameter;

            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":firstName";
            aParameter.Value = firstName;
            aParameter.DbType = System.Data.DbType.String;

            parametersArray.Add(aParameter);


            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":surname";
            aParameter.Value = surname;
            aParameter.DbType = System.Data.DbType.String;

            parametersArray.Add(aParameter);


            // execute the sql statment
            SqliteDataReader rdr;
            rdr = this._con.ExecuteParameterizedSelectCommand(sb.ToString(), parametersArray.ToArray());


            // did we get any data from the database
            while (rdr.Read())
            {
                UserFullDetails usersInfo;
                // get the data from the row and add it to a Models.User class
                usersInfo = this.GetRowData(rdr);

                usersList.Add(usersInfo);
            }
            rdr.Close();




            // return list of found users
            return usersList;
        }



        /// <summary>
        /// Gets all users in a given register
        /// </summary>
        /// <param name="regiserID">The register to get the users from</param>
        /// <returns>List of users that were found in the register</returns>
        public List<UserFullDetails> selectUsersInRegister(int regiserID)
        {
            // will hold all users that match the query
            List<UserFullDetails> usersList = new List<UserFullDetails>();

            StringBuilder sb = new StringBuilder();

            sb.Append($"SELECT {this.tableName}.ID, {this.tableName}.firstName, {this.tableName}.surname ");
            sb.Append($"FROM {this.tableName} ");
            sb.Append("INNER JOIN UsersInTrainingRegister ON ");
            sb.Append($"{this.tableName}.id = UsersInTrainingRegister.userId ") ;
            sb.Append("WHERE UsersInTrainingRegister.trainingRegisterId = :id ");
            sb.Append("ORDER BY UsersInTrainingRegister.trainingRegisterId;");

            List<SqliteParameter> parametersArray = new List<SqliteParameter>();
            SqliteParameter aParameter;

            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":id";
            aParameter.Value = regiserID;
            aParameter.DbType = System.Data.DbType.Int32;

            parametersArray.Add(aParameter);



            // // execute the sql statment
            SqliteDataReader rdr;
            rdr = this._con.ExecuteParameterizedSelectCommand(sb.ToString(), parametersArray.ToArray());


            // did we get any data from the database
            while (rdr.Read())
            {
                UserFullDetails usersInfo;
                // get the data from the row and add it to a Models.User class
                usersInfo = this.GetRowData(rdr);

                usersList.Add(usersInfo);
            }
            rdr.Close();

            return usersList;

        }

        #endregion

        #region Insert functions


        /// <summary>
        /// Inserts a new row into the database
        /// </summary>
        /// <param name="firstName">users first name</param>
        /// <param name="surname">users surname</param>
        /// <param name="password">password hashed password</param>
        /// <param name="isUserActive">true is user is currently working, else false</param>
        /// <returns>an instance of the newly created User or null if insert fails</returns>
        public UserFullDetails insert(string firstName, string surname, string password, bool isUserActive)
        {
            UserFullDetails userFullDetails = null;

            StringBuilder sb = new StringBuilder();

            sb.Append($"INSERT INTO {this.tableName} ");
            sb.Append("(firstName,surname,password,isUserActive) ");
            sb.Append("VALUES(");
            sb.Append(":firstName, :surname, :password, :isUserActive");
            sb.Append(");");


            List<SqliteParameter> parametersArray = new List<SqliteParameter>();
            SqliteParameter aParameter;

            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":firstName";
            aParameter.Value = firstName;
            aParameter.DbType = System.Data.DbType.String;

            parametersArray.Add(aParameter);



            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":surname";
            aParameter.Value = surname;
            aParameter.DbType = System.Data.DbType.String;

            parametersArray.Add(aParameter);



            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":password";
            aParameter.Value = password;
            aParameter.DbType = System.Data.DbType.String;

            parametersArray.Add(aParameter);




            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":isUserActive";
            aParameter.Value = isUserActive;
            aParameter.DbType = System.Data.DbType.Boolean;

            parametersArray.Add(aParameter);



            // execute the sql statment
            int  NumRowsEffected;
            NumRowsEffected = this._con.ExecuteParameterizedNoneReader(sb.ToString(), parametersArray.ToArray());

            // if greater than zero the row was inserted
            if(NumRowsEffected > 0)
            {
                // get the id of the just created row
                int lastInsertID = this._con.get_last_insert_id();
                // get the users details for the user that was just created
                userFullDetails = this.select(lastInsertID);
            }

            return userFullDetails;
        
        }
        #endregion

        #region update functions



        /// <summary>
        /// Updates a row with the passed in values
        /// </summary>
        /// <param name="id"></param>
        /// <param name="firstName">persons first name</param>
        /// <param name="surname">persons surname</param>
        /// <param name="password">users password in a SHA-2 hash</param>
        /// <param name="isUserActive">Indicates if user is active. An incative user may not be able to log in</param>
        /// <returns>a new model containing the updated users details</returns>
        public UserFullDetails updateAll(int id, string firstName, string surname, string password, bool isUserActive)
        {

            UserFullDetails userFullDetails = null;

            StringBuilder sb = new StringBuilder();

            sb.Append($"UPDATE {this.tableName} ");
            sb.Append("SET ");
            sb.Append("firstName=:firstName,");
            sb.Append("surname=:surname,");
            sb.Append("password=:password,");
            sb.Append("isUserActive=:isUserActive ");
            sb.Append("WHERE id=:id");



            List<SqliteParameter> parametersArray = new List<SqliteParameter>();
            SqliteParameter aParameter;

            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":firstName";
            aParameter.Value = firstName;
            aParameter.DbType = System.Data.DbType.String;

            parametersArray.Add(aParameter);



            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":surname";
            aParameter.Value = surname;
            aParameter.DbType = System.Data.DbType.String;

            parametersArray.Add(aParameter);



            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":password";
            aParameter.Value = password;
            aParameter.DbType = System.Data.DbType.String;

            parametersArray.Add(aParameter);




            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":isUserActive";
            aParameter.Value = isUserActive;
            aParameter.DbType = System.Data.DbType.Boolean;

            parametersArray.Add(aParameter);




            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":id";
            aParameter.Value = id;
            aParameter.DbType = System.Data.DbType.Int32;

            parametersArray.Add(aParameter);


            // execute the sql statment
            int NumRowsEffected;
            NumRowsEffected = this._con.ExecuteParameterizedNoneReader(sb.ToString(), parametersArray.ToArray());

            // if greater than zero the row was inserted
            if (NumRowsEffected > 0)
            {
                // get the users details for the user that was just created
                userFullDetails = this.select(id);
            }

            return userFullDetails;
        }



        /// <summary>
        /// Updates all columns except the password
        /// </summary>
        /// <param name="id">the row to update</param>
        /// <param name="firstName">persons first name</param>
        /// <param name="surname">persons surname</param>
        /// <param name="isUserActive">Indicates if user is active. An incative user may not be able to log in</param>
        /// <returns>a new model containing the updated users details</returns>
        public UserFullDetails updateDetails(int id, string firstName, string surname, bool isUserActive)
        {

            UserFullDetails userFullDetails = null;

            StringBuilder sb = new StringBuilder();

            sb.Append($"UPDATE {this.tableName} ");
            sb.Append("SET ");
            sb.Append("firstName=:firstName,");
            sb.Append("surname=:surname,");
            sb.Append("isUserActive=:isUserActive ");
            sb.Append("WHERE id=:id");



            List<SqliteParameter> parametersArray = new List<SqliteParameter>();
            SqliteParameter aParameter;

            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":firstName";
            aParameter.Value = firstName;
            aParameter.DbType = System.Data.DbType.String;

            parametersArray.Add(aParameter);



            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":surname";
            aParameter.Value = surname;
            aParameter.DbType = System.Data.DbType.String;

            parametersArray.Add(aParameter);




            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":isUserActive";
            aParameter.Value = isUserActive;
            aParameter.DbType = System.Data.DbType.Boolean;

            parametersArray.Add(aParameter);




            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":id";
            aParameter.Value = id;
            aParameter.DbType = System.Data.DbType.Int32;

            parametersArray.Add(aParameter);



            // execute the sql statment
            int NumRowsEffected;
            NumRowsEffected = this._con.ExecuteParameterizedNoneReader(sb.ToString(), parametersArray.ToArray());

            // if greater than zero the row was inserted
            if (NumRowsEffected > 0)
            {
                // get the users details for the user that was just created
                userFullDetails = this.select(id);
            }

            return userFullDetails;


        }




        /// <summary>
        /// Updates a users password to the new one passed in
        /// </summary>
        /// <param name="id">the row to update</param>
        /// <param name="password">users password in a SHA-2 hash</param>
        /// <returns>a new model containing the updated users details</returns>
        public UserFullDetails updatePassword(int id, string password)
        {
            UserFullDetails userFullDetails = null;

            StringBuilder sb = new StringBuilder();

            sb.Append($"UPDATE {this.tableName} ");
            sb.Append("SET ");
            sb.Append("password=:password ");
            sb.Append("WHERE id=:id");



            List<SqliteParameter> parametersArray = new List<SqliteParameter>();
            SqliteParameter aParameter;




            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":password";
            aParameter.Value = password;
            aParameter.DbType = System.Data.DbType.String;

            parametersArray.Add(aParameter);



            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":id";
            aParameter.Value = id;
            aParameter.DbType = System.Data.DbType.Int32;

            parametersArray.Add(aParameter);



            // execute the sql statment
            int NumRowsEffected;
            NumRowsEffected = this._con.ExecuteParameterizedNoneReader(sb.ToString(), parametersArray.ToArray());

            // if greater than zero the row was inserted
            if (NumRowsEffected > 0)
            {
                // get the users details for the user that was just created
                userFullDetails = this.select(id);
            }

            return userFullDetails;

        }




        #endregion


        #region delete functions


        /// <summary>
        /// Removes the selected row from the database
        /// </summary>
        /// <param name="id">the id of the row to be removed</param>
        /// <returns>true if sucsefull, else false</returns>
        public bool delete(int id)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"DELETE FROM {this.tableName} ");
            sb.Append("WHERE id = :id");


            List<SqliteParameter> parametersArray = new List<SqliteParameter>();
            SqliteParameter aParameter;


            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":id";
            aParameter.Value = id;
            aParameter.DbType = System.Data.DbType.Int32;

            parametersArray.Add(aParameter);


            // execute the sql statment
            int NumRowsEffected;
            NumRowsEffected = this._con.ExecuteParameterizedNoneReader(sb.ToString(), parametersArray.ToArray());

            // did the row get deleted
            if (NumRowsEffected > 0)
                // row got deleted
                return true;
            else
                // row did not get deleted
                return false;
            
        }
        #endregion




        /// <summary>
        /// Converts the database row into a <see cref="UserFullDetails"/>
        /// </summary>
        /// <param name="rdr">The database row that contains a users details</param>
        /// <returns></returns>
        private UserFullDetails GetRowData(SqliteDataReader rdr)
        {
            UserFullDetails userFullDetails = new UserFullDetails();

            userFullDetails.id = rdr.GetInt32(0);
            userFullDetails.firstName = rdr.GetString(1);
            userFullDetails.surname = rdr.GetString(2);
            userFullDetails.password = rdr.GetString(3);
            userFullDetails.isUserActive = rdr.GetBoolean(4);

            return userFullDetails;
        }

    }
}
