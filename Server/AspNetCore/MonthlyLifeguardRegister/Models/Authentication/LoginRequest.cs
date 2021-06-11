using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Models.Authentication
{
    /// <summary>
    /// The login data sent from the client when requesting to login to the application
    /// used in API/Authentication/Login.php
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// username for the person logging in
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// Day of month user was born on
        /// </summary>
        public int day { get; set; }
        /// <summary>
        /// Month user was born on
        /// </summary>
        public int month { get; set; }
        /// <summary>
        /// Year user was born on
        /// </summary>
        public int year { get; set; }
    }
}
