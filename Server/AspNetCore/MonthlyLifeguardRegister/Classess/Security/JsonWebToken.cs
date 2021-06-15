using Microsoft.IdentityModel.Tokens;
using MonthlyLifeguardRegister.Models.User;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Classess.Security
{
    public class JsonWebToken
    {
        /// <summary>
        /// The key used when creating the JWT
        /// </summary>
        private string _SecreteKey;

        /// <summary>
        /// inishalizes the JsonWebToken class, passing in the key used for creating the JWT
        /// </summary>
        /// <param name="secreteKey">key used to create the JWT</param>
        public JsonWebToken(string secreteKey)
        {
            this._SecreteKey = secreteKey;
        }


        /// <summary>
        /// Creates a jwt which is signed using the jwtsecretKey set in the appsettings.json file
        /// </summary>
        /// <param name="userID">Id of the user from the database that is logging in (added to the json token)</param>
        /// <param name="userFirstName">first name of the user logging in (added to the json token)</param>
        /// <param name="userSurname">surname of the user logging in (added to the json token)</param>
        /// <param name="dateWhenExpires">sets the date when the json web toekn expires</param>
        /// <returns></returns>
        public string createUserJWT(int userID, string userFirstName, string userSurname, DateTime dateWhenExpires)
        {

            
            JwtSecurityTokenHandler jwt = new JwtSecurityTokenHandler();
   
            SecurityTokenDescriptor tokenDescriptor = this.createTokenDescriptor(dateWhenExpires);
            // add the id, first name and surname
            tokenDescriptor.AdditionalHeaderClaims = new Dictionary<string, object>();
            tokenDescriptor.AdditionalHeaderClaims.Add("id", userID);
            tokenDescriptor.AdditionalHeaderClaims.Add("userFirstName", userFirstName);
            tokenDescriptor.AdditionalHeaderClaims.Add("userSurname", userSurname);

            // create the jwt
            SecurityToken token = jwt.CreateToken(tokenDescriptor);

            // convert jwt to string and return it
            return jwt.WriteToken(token);

        }

        /// <summary>
        /// Create a jwt for the admin user which is signed using the jwtsecretKey set int eh asppsettings.json file
        /// </summary>
        /// <param name="dateWhenExpires"></param>
        /// <returns>sets the date when the json web toekn expires</returns>
        public string createAdminJWT(DateTime dateWhenExpires)
        {
            JwtSecurityTokenHandler jwt = new JwtSecurityTokenHandler();

            SecurityTokenDescriptor tokenDescriptor = this.createTokenDescriptor(dateWhenExpires);
            // add the id, first name and surname
            tokenDescriptor.AdditionalHeaderClaims = new Dictionary<string, object>();
            tokenDescriptor.AdditionalHeaderClaims.Add("username", "admin");

            // create the jwt
            SecurityToken token = jwt.CreateToken(tokenDescriptor);

            // convert jwt to string and return it
            return jwt.WriteToken(token);
        }

        /// <summary>
        /// Checks the jwt to make sure it validates (no one has message around with it)
        /// </summary>
        /// <param name="jwt">The jwt string we want to validate</param>
        /// <returns>true if valid, else false</returns>
        public bool isJwtValid(string jwt)
        {
            // return false if null is returned, else return true;
            return this.getValidatedJWT(jwt) == null ? false : true;
        }

        /// <summary>
        /// Gets the client data from json web token
        /// </summary>
        /// <param name="jwt">the json web token string </param>
        /// <returns>returns null if validation values</returns>
        public JwtUser getClientDataFromJwt(string jwt)
        {
            JwtSecurityToken jwtSecurityToken;
            // the return value
            JwtUser jwtUser = null;

            // validate and parse the security token
            jwtSecurityToken = this.getValidatedJWT(jwt);

            // did validation fail
            if (jwtSecurityToken == null)
                return null;

            // will hold the data we want to return
            jwtUser = new JwtUser();
            
            // go through each claim 
            foreach(var claim in jwtSecurityToken.Claims)
            {
                // look for the id, first name and surname
                switch(claim.Type)
                {
                    case nameof(JwtUser.id):
                        jwtUser.id = int.Parse(claim.Value);
                        break;

                    case nameof(JwtUser.firstName):
                        jwtUser.firstName = claim.Value;
                        break;

                    case nameof(JwtUser.surname):
                        jwtUser.surname = claim.Value;
                        break;
                }
            }

            // return the users details from the jwt
            return jwtUser;

        }

        /// <summary>
        /// Atempts to validate the jwt and return it. Retunrs null if fails to validate
        /// </summary>
        /// <param name="jwt">The jwt string to validate</param>
        /// <returns>returns null if validation fails</returns>
        private JwtSecurityToken getValidatedJWT(string jwt)
        {
            // the return value (validated jwt, or null if fails to validate)
            JwtSecurityToken jwtToken = null;

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            // convert the key to a byte array 
            byte[] key = Encoding.ASCII.GetBytes(this._SecreteKey);
            // set the parameters for validating the jwt
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            };

            // try and valid date the token, if it fails it will through an execption
            try
            {
                // try and validate the token
                tokenHandler.ValidateToken(jwt, tokenValidationParameters, out SecurityToken validatedToken);

                // set the return valid to the valid jwt
                jwtToken = (JwtSecurityToken)validatedToken;

            }
            catch
            {
                // do nothing if jwt validation fails
            }

            // returns the validated jwt or null if validation failed
            return jwtToken;
        }


        /// <summary>
        /// Creates a Token Description with the Expires, NotBefore, IssedAt and SigningCredentials set
        /// </summary>
        /// <param name="dateWhenExpires">Date when token will expire</param>
        /// <returns></returns>
        private SecurityTokenDescriptor createTokenDescriptor(DateTime dateWhenExpires)
        {
            // convert the secrete key to a byte array
            byte[] key = Encoding.ASCII.GetBytes(this._SecreteKey);

            // create the payload
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Expires = dateWhenExpires,
                NotBefore = dateWhenExpires.Subtract(new TimeSpan(0, 0, 30)),
                IssuedAt = DateTime.Now,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenDescriptor;
        }


    }
}
