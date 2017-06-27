using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IdentityModel.Selectors;
using System.Web.Security;
using System.IdentityModel.Tokens;

namespace MapperSvc.App_Code.Security
{
    public class CustomUserNamePasswordValidator:UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {

            if( !Membership.ValidateUser(userName, password))
            {
                throw new SecurityTokenException("Unknown username or password");
            }

            //// validate arguments
            //if (string.IsNullOrEmpty(userName))
            //    throw new ArgumentNullException("userName");
            //if (string.IsNullOrEmpty(password))
            //    throw new ArgumentNullException("password");

            // check the user credentials from database
            //int userid = 0;
            //CheckUserNameAndPassword(userName, password, out userid);
            //if (0 == userid)
            //throw new SecurityTokenException("Unknown username or password");
        }
    }
}