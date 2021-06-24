using Microsoft.Data.Sqlite;
using MonthlyLifeguardRegister.Models.Training;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Classess.Database.Training
{
    public class dbSQLiteTrainingRegister
    {
        /// <summary>
        /// CRUD methods for <see cref="UserFullDetails"/> in database
        /// </summary>
        private SqLiteConnection _con;

        /// <summary>
        /// Name of the table in the database 
        /// </summary>
        public string tableName = "TrainingRegister";

        /// <summary>
        /// Inishalized the class ready for interacting with the database.
        /// </summary>
        /// <param name="connection">SQLiteConnection to the database which should have allready been opened before passing in</param>
        public dbSQLiteTrainingRegister(SqLiteConnection connection)
        {
            this._con = connection;// this should be an allready open connection
        }



        #region Select functions


        /// <summary>
        /// select 1 TrainingRegister from the database
        /// </summary>
        /// <param name="ID">the id of te TrainingRegister to look for</param>
        /// <returns>an instance of TrainingRegister with data retreeved from database, or null if could not be found</returns>
        public TrainingRegister Select(int ID)
        {
            // holds the data we will get from the database
            TrainingRegister trainingRegister = null;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT id, dateTimeOfTraining, dateTimeWhenRegisterIsActive, dateTimeFromWhenRegisterCanBeUsed, maxNoCandidatesAllowed ");
            sb.Append($"FROM {this.tableName} ");
            sb.Append("WHERE id=:id");

            // holds a list of parameters to insert into the sql query
            List<SqliteParameter> parametersArray = new List<SqliteParameter>();
            SqliteParameter aParameter;

            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":id";
            aParameter.Value = ID;
            aParameter.DbType = System.Data.DbType.Int32;

            // add the parameter to the parameter array
            parametersArray.Add(aParameter);


            SqliteDataReader rdr;
            rdr = this._con.ExecuteParameterizedSelectCommand(sb.ToString(), parametersArray.ToArray());

            // did we get any data from the database
            if (rdr.Read())
            {
                // get the data from the row and add it to a Models.TrainingRegister class
                trainingRegister = this.GetRowData(rdr);
            }
            rdr.Close();

            // retuns the requested TrainingRegister found in the database, or null if not found
            return trainingRegister;
    
        }



        /// <summary>
        /// selects all rows from the TrainingRegister table in the database and returns them as an array of TrainingRegister
        /// </summary>
        /// <returns>list of all TrainingRegister in database</returns>
        public List<TrainingRegister> SelectAll()
        {
            // an array of TrainingRegister classes
            List<TrainingRegister> TrainingRegisterArray = new List<TrainingRegister>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT id, dateTimeOfTraining, dateTimeWhenRegisterIsActive, dateTimeFromWhenRegisterCanBeUsed, maxNoCandidatesAllowed ");
            sb.Append($"FROM {this.tableName} ");


            // execute the sql statment
            SqliteDataReader rdr;
            rdr = this._con.ExecuteSelectCommand(sb.ToString());

            // get each row
            while (rdr.Read())
            {
                // get the data from the row
                TrainingRegister aTrainingRegister = this.GetRowData(rdr);
                // add the model class to the array
                TrainingRegisterArray.Add(aTrainingRegister);
            }
            rdr.Close();


            // a list of TrainingRegister
            return TrainingRegisterArray;


        }

        /// <summary>
        /// selects all rows from the TrainingRegister table in the database where dateTimeOfTraining falls between 2 dates
        /// </summary>
        /// <param name="from">Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.</param>
        /// <param name="to">Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.</param>
        /// <returns>list of all TrainingRegister in database that match the query</returns>
        public List<TrainingRegister> Select_DateTimeOfTraining_Between(long from, long to)
        {
            List<TrainingRegister> TrainingRegisterArray = new List<TrainingRegister>();
            StringBuilder sb = new StringBuilder();
        

            sb.Append("SELECT id, dateTimeOfTraining, dateTimeWhenRegisterIsActive, dateTimeFromWhenRegisterCanBeUsed, maxNoCandidatesAllowed ");
            sb.Append($"FROM {this.tableName} ");
            sb.Append("WHERE dateTimeOfTraining >= :from AND dateTimeOfTraining <= :to ");
            sb.Append("ORDER BY DateTimeOfTraining;");

            // executes the database query and returns a a list of Models.Training.TraingRegister
            return this.ExecuteSelectBetweenQuery(sb.ToString(), to, from);

        }



        /// <summary>
        /// selects all rows from the TrainingRegister table in the database where dateTimeWhenRegisterIsActive falls between 2 dates
        /// </summary>
        /// <param name="from">Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.</param>
        /// <param name="to">Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.</param>
        /// <returns>list of all TrainingRegister in database that match the query</returns>
        public List<TrainingRegister> Select_DateTimeWhenRegisterIsActive_Between(long from, long to)
        {
            List<TrainingRegister> TrainingRegisterArray = new List<TrainingRegister>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT id, dateTimeOfTraining, dateTimeWhenRegisterIsActive, dateTimeFromWhenRegisterCanBeUsed, maxNoCandidatesAllowed ");
            sb.Append($"FROM {this.tableName} ");
            sb.Append("WHERE dateTimeOfTraining >= :from AND dateTimeWhenRegisterIsActive <= :to ");
            sb.Append("ORDER BY dateTimeOfTraining");


            // executes the database query and returns a a list of Models.Training.TraingRegister
            return this.ExecuteSelectBetweenQuery(sb.ToString(), to, from);
        }


 
        /// <summary>
        /// selects all rows from the TrainingRegister table in the database where dateTimeFromWhenRegisterCanBeUsed falls between 2 dates
        /// </summary>
        /// <param name="from">Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.</param>
        /// <param name="to">Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.</param>
        /// <returns>list of all TrainingRegister in database that match the query</returns>
        public List<TrainingRegister> Select_DateTimeFromWhenRegisterCanBeUsed_Between(long from, long to)
        {
            List<TrainingRegister> TrainingRegisterArray = new List<TrainingRegister>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT id, dateTimeOfTraining, dateTimeWhenRegisterIsActive, dateTimeFromWhenRegisterCanBeUsed, maxNoCandidatesAllowed ");
            sb.Append($"FROM {this.tableName} ");
            sb.Append("WHERE dateTimeFromWhenRegisterCanBeUsed >= :from AND dateTimeFromWhenRegisterCanBeUsed <= :to;");


            // executes the database query and returns a a list of Models.Training.TraingRegister
            return this.ExecuteSelectBetweenQuery(sb.ToString(), to, from);
        }


        #endregion


        #region Insert functions


        /// <summary>
        /// Inserts a new row into the database and returns the new row (or null if fails)
        /// </summary>
        /// <param name="DateTimeOfTraining"> unix time stamp</param>
        /// <param name="DateTimeWhenRegisterIsActive"> unix time stamp</param>
        /// <param name="DateTimeFromWhenRegisterCanBeUsed"> unix time stamp</param>
        /// <param name="MaxNoCandidatesAllowed">Max numbe of candidates allowed on register</param>
        /// <returns>an instance of the newly created row or null if insert fails</returns>
        public TrainingRegister Insert(long DateTimeOfTraining, long DateTimeWhenRegisterIsActive, long DateTimeFromWhenRegisterCanBeUsed, int MaxNoCandidatesAllowed)
        {
            // holds the data we will get from the database
            TrainingRegister trainingRegister = null;
            StringBuilder sb = new StringBuilder();

            sb.Append($"INSERT INTO {this.tableName} ");
            sb.Append("(DateTimeOfTraining,DateTimeWhenRegisterIsActive,DateTimeFromWhenRegisterCanBeUsed,MaxNoCandidatesAllowed) ");
            sb.Append("VALUES(");
            sb.Append(":DateTimeOfTraining, :DateTimeWhenRegisterIsActive, :DateTimeFromWhenRegisterCanBeUsed, :MaxNoCandidatesAllowed");
            sb.Append(");");


            // holds a list of parameters to insert into the sql query
            List<SqliteParameter> parametersArray = new List<SqliteParameter>();
            SqliteParameter aParameter;

            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":DateTimeOfTraining";
            aParameter.Value = DateTimeOfTraining;
            aParameter.DbType = System.Data.DbType.Int64;

            // add the parameter to the parameter array
            parametersArray.Add(aParameter);



            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":DateTimeWhenRegisterIsActive";
            aParameter.Value = DateTimeWhenRegisterIsActive;
            aParameter.DbType = System.Data.DbType.Int64;

            // add the parameter to the parameter array
            parametersArray.Add(aParameter);



            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":DateTimeFromWhenRegisterCanBeUsed";
            aParameter.Value = DateTimeFromWhenRegisterCanBeUsed;
            aParameter.DbType = System.Data.DbType.Int64;

            // add the parameter to the parameter array
            parametersArray.Add(aParameter);



            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":MaxNoCandidatesAllowed";
            aParameter.Value = MaxNoCandidatesAllowed;
            aParameter.DbType = System.Data.DbType.Int32;

            // add the parameter to the parameter array
            parametersArray.Add(aParameter);



            // execute the sql statment
            int NumRowsEffected;
            NumRowsEffected = this._con.ExecuteParameterizedNoneReader(sb.ToString(), parametersArray.ToArray());

            // if greater than zero the row was inserted
            if (NumRowsEffected > 0)
            {
                // get the id of the just created row
                int lastInsertID = this._con.Get_Last_Insert_Id();
                // get the details for the database that was just created
                trainingRegister = this.Select(lastInsertID);
            }

            // return the new inserted row, or null if it failed
            return trainingRegister;
        }



        #endregion



        #region Update functions


 
        /// <summary>
        /// updates a row with the passed in values
        /// </summary>
        /// <param name="id">the row to update</param>
        /// <param name="DateTimeOfTraining">unix time stamp</param>
        /// <param name="DateTimeWhenRegisterIsActive">unix time stamp</param>
        /// <param name="DateTimeFromWhenRegisterCanBeUsed">unix time stamp</param>
        /// <param name="MaxNoCandidatesAllowed">Max numbe of candidates allowed on register</param>
        /// <returns>instance of the row that was just updated or null if update fails</returns>
        public TrainingRegister Update(int id, long DateTimeOfTraining, long DateTimeWhenRegisterIsActive, long DateTimeFromWhenRegisterCanBeUsed, int MaxNoCandidatesAllowed)
        {
            // holds the data we will get from the database
            TrainingRegister trainingRegister = null;
            StringBuilder sb = new StringBuilder();

            sb.Append($"UPDATE {this.tableName} ");
            sb.Append("SET ");
            sb.Append("DateTimeOfTraining=:DateTimeOfTraining,");
            sb.Append("DateTimeWhenRegisterIsActive=:DateTimeWhenRegisterIsActive,");
            sb.Append("DateTimeFromWhenRegisterCanBeUsed=:DateTimeFromWhenRegisterCanBeUsed,");
            sb.Append("MaxNoCandidatesAllowed=:MaxNoCandidatesAllowed ");
            sb.Append("WHERE id=:id");


            // holds a list of parameters to insert into the sql query
            List<SqliteParameter> parametersArray = new List<SqliteParameter>();
            SqliteParameter aParameter;

            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":DateTimeOfTraining";
            aParameter.Value = DateTimeOfTraining;
            aParameter.DbType = System.Data.DbType.Int64;

            // add the parameter to the parameter array
            parametersArray.Add(aParameter);



            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":DateTimeWhenRegisterIsActive";
            aParameter.Value = DateTimeWhenRegisterIsActive;
            aParameter.DbType = System.Data.DbType.Int64;

            // add the parameter to the parameter array
            parametersArray.Add(aParameter);



            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":DateTimeFromWhenRegisterCanBeUsed";
            aParameter.Value = DateTimeFromWhenRegisterCanBeUsed;
            aParameter.DbType = System.Data.DbType.Int64;

            // add the parameter to the parameter array
            parametersArray.Add(aParameter);



            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":MaxNoCandidatesAllowed";
            aParameter.Value = MaxNoCandidatesAllowed;
            aParameter.DbType = System.Data.DbType.Int32;

            // add the parameter to the parameter array
            parametersArray.Add(aParameter);



            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":id";
            aParameter.Value = id;
            aParameter.DbType = System.Data.DbType.Int32;

            // add the parameter to the parameter array
            parametersArray.Add(aParameter);

            // execute the sql statment
            int NumRowsEffected;
            NumRowsEffected = this._con.ExecuteParameterizedNoneReader(sb.ToString(), parametersArray.ToArray());

            // if greater than zero the row was inserted
            if (NumRowsEffected > 0)
            {
                // get the id of the just created row
                int lastInsertID = this._con.Get_Last_Insert_Id();
                // get the details for the database that was just created
                trainingRegister = this.Select(lastInsertID);
            }

            // return the new inserted row, or null if it failed
            return trainingRegister;
        }

        #endregion


        #region Delete functions

        /**
         * Removes the selected row from the database
         * @param id the id of the row to be removed
         * @return bool true if sucsefull, else false
         */
        public bool Delete(int id)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.Append($"DELETE FROM {this.tableName} ");
            sb.Append("WHERE id = :id");

            // holds a list of parameters to insert into the sql query
            List<SqliteParameter> parametersArray = new List<SqliteParameter>();
            SqliteParameter aParameter;

            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":DateTimeOfTraining";
            aParameter.Value = id;
            aParameter.DbType = System.Data.DbType.Int32;

            // add the parameter to the parameter array
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
        /// Converts the database row into a <see cref="TrainingRegister"/>
        /// </summary>
        /// <param name="rdr">The database row that contains a TrainingRegister details</param>
        /// <returns></returns>
        private TrainingRegister GetRowData(SqliteDataReader rdr)
        {
            TrainingRegister trainingRegister = new TrainingRegister();

            trainingRegister.id = rdr.GetInt32(0);
            trainingRegister.dateTimeOfTraining = rdr.GetInt64(1);
            trainingRegister.dateTimeWhenRegisterIsActive = rdr.GetInt64(2);
            trainingRegister.dateTimeFromWhenRegisterCanBeUsed = rdr.GetInt64(3);
            trainingRegister.maxNoCandidatesAllowed = rdr.GetInt32(4);

            return trainingRegister;
        }


        /**
         * All the select between funcions were simlar, so extracted the code from
         * them that was the same and put it all into this function
         * @param query the sql query to run
         * @param to unix time stamp (number)
         * @param from unix time stamp (number)
         * @return Models.Training.TrainingRegister list of all TrainingRegister in database that match the query
         */
        private List<TrainingRegister> ExecuteSelectBetweenQuery(string query, long to, long from)
        {
            // an array of TrainingRegister classes
            List<TrainingRegister> TrainingRegisterArray = new List<TrainingRegister>();

            // holds a list of parameters to insert into the sql query
            List<SqliteParameter> parametersArray = new List<SqliteParameter>();
            SqliteParameter aParameter;

            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":from";
            aParameter.Value = from;
            aParameter.DbType = System.Data.DbType.Int64;

            // add the parameter to the parameter array
            parametersArray.Add(aParameter);

            aParameter = new SqliteParameter();
            aParameter.ParameterName = ":to";
            aParameter.Value = to;
            aParameter.DbType = System.Data.DbType.Int64;

            // add the parameter to the parameter array
            parametersArray.Add(aParameter);

            SqliteDataReader rdr;
            // execute the sql statment
            rdr = this._con.ExecuteParameterizedSelectCommand(query, parametersArray.ToArray());


            // get each row
            while (rdr.Read())
            {
                // get the data from the row 
                TrainingRegister aTrainingRegister = this.GetRowData(rdr);
                // add the model class to the array
                TrainingRegisterArray.Add(aTrainingRegister);
            }
            rdr.Close();
        
            // return the array
            return TrainingRegisterArray;
        }
    }
}
