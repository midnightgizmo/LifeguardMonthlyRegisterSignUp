using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Models
{
    /// <summary>
    /// Instance of the database table UsersInTrainingRegister
    /// </summary>
    public class UsersInTrainingRegister
    {

        public int userId {get;set;} = 0;

        public int trainingRegisterId {get;set;} = 0;
    }
}
