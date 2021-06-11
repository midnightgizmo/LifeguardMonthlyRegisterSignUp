using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Models.Authentication
{
    public class LoginAuthentication
    {
        public bool _isLoggedIn { get; set; } = false;
    

        // if isLoggedIn is false, errorMessage will contain
        // information about why they are not logged in.
        public string _errorMessage { get; set; } = "";

        // the json web token used to authenticate the user
        public string _jwt { get; set; } = "";
    }
}
