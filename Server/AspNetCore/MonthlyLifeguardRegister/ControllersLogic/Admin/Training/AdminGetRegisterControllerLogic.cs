using MonthlyLifeguardRegister.Classess.Database;
using MonthlyLifeguardRegister.Classess.Database.Training;
using MonthlyLifeguardRegister.Models;
using MonthlyLifeguardRegister.Models.Training;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.ControllersLogic.Admin.Training
{
    public class AdminGetRegisterControllerLogic
    {
        /// <summary>
        /// returns the register from the database or blank register with its id set to -1 if not found
        /// </summary>
        /// <param name="RegisterID">the register to look for</param>
        /// <param name="appSettings">holds the database location</param>
        /// <returns>instance of register from database or blank register with id set to -1 if could not be found</returns>
        public TrainingRegister GetRegister_CreateResponse(int RegisterID, AppSettings appSettings)
        {
            TrainingRegister trainingRegister;

            trainingRegister = this.GetTrainingRegister(RegisterID, appSettings.sqlConectionStringLocation);

            // if the register was not found, return a blank register with its id set to -1
            if (trainingRegister == null)
                return this.CreateBlankRegister();

            // return the register
            return trainingRegister;
        }




        /// <summary>
        /// Gets the selected register from the database. returns null if not found
        /// </summary>
        /// <param name="registerID">The register to look for</param>
        /// <param name="sqlConectionStringLocation">location of the database on disk</param>
        /// <returns>Returns the register from the database, or null if not found</returns>
        private TrainingRegister GetTrainingRegister(int registerID, string sqlConectionStringLocation)
        {
            SqLiteConnection sqlCon;
            dbSQLiteTrainingRegister dbTrainingRegister;
            TrainingRegister trainingRegister;

            sqlCon = new SqLiteConnection();
            sqlCon.OpenConnection(sqlConectionStringLocation);

            dbTrainingRegister = new dbSQLiteTrainingRegister(sqlCon);

            trainingRegister = dbTrainingRegister.Select(registerID);
            sqlCon.CloseConnection();

            return trainingRegister;

        }

        /// <summary>
        /// Creates a blank training register with its id set to -1;
        /// </summary>
        /// <returns></returns>
        private TrainingRegister CreateBlankRegister()
        {
            TrainingRegister trainingRegister = new TrainingRegister();
            trainingRegister.id = -1;

            return trainingRegister;
            
        }
    }
}
