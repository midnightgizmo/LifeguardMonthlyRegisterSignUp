using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Models.User
{
    public class JwtUser
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string surname { get; set; }
    }
}
