using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Models
{
    public class AppSettings
    {
        /// <summary>
        /// Domain name the client is running on (e.g. localhost)
        /// </summary>
        public string DomainName { get; set; }
        /// <summary>
        ///  The key used when creating the jwt
        /// </summary>
        public string jwtsecretKey { get; set; }

        /// <summary>
        /// Physical location on disk to where the database is
        /// </summary>
        public string sqlConectionStringLocation { get; set; }
    }
}
