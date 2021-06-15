using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Models.User
{
    /// <summary>
    /// The user details that the client will need (not admin login)
    /// </summary>
    public class UserFullDetails : JwtUser
    {
        public string password { get; set; }

        public bool isUserActive { get; set; }
    }
}
