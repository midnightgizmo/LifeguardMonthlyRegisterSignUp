using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Models
{
    public class AppSettings
    {
        public string DomainName { get; set; }
        /// <summary>
        ///  The key used when creating the jwt
        /// </summary>
        public string jwtsecretKey { get; set; }
    }
}
