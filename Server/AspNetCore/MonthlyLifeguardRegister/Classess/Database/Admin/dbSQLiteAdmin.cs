using Microsoft.Data.Sqlite;
using MonthlyLifeguardRegister.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Classess.Database.Admin
{
    public class dbSQLiteAdmin
    {
        /// <summary>
        /// CRUD methods for <see cref="UserFullDetails"/> in database
        /// </summary>
        private SqLiteConnection _con;

        /// <summary>
        /// Name of the table in the database 
        /// </summary>
        public string tableName = "Admin";

        /// <summary>
        /// Inishalized the class ready for interacting with the database.
        /// </summary>
        /// <param name="connection">SQLiteConnection to the database which should have allready been opened before passing in</param>
        public dbSQLiteAdmin(SqLiteConnection connection)
        {
            this._con = connection;// this should be an allready open connection
        }



        /// <summary>
        /// select the admin username and hashed password from the database
        /// </summary>
        /// <returns>Admin Credentials or null if not found</returns>
        public UserCredentials SelectUsernameAndPassword()
        {
            UserCredentials userCredentials = null;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT username, password ");
            sb.Append($"FROM {this.tableName} ");



            // execute the sql statment
            SqliteDataReader rdr;
            rdr = this._con.ExecuteSelectCommand(sb.ToString());

            // did we find the username and password in the database
            if(rdr.Read())
            {
                // get the details
                userCredentials = new UserCredentials();
                userCredentials.UserName = rdr.GetString(0);
                userCredentials.Password = rdr.GetString(1);
            }
            rdr.Close();

            // return the redentials or null if not found
            return userCredentials;

        
        }






    }
}
