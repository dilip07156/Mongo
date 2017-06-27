using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.ServiceModel;
using System.ServiceModel.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BAL
{
    public class MemberAuthentication : IDisposable
    {
        public void Dispose()
        {
        }

        public static int TokenSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TokenSize"]);
        public static int DefaultSecondsUntilTokenExpires = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TokenExpirySeconds"]);

        public bool AuthenticateUser(DataContracts.Credentials creds)
        {
            return Membership.ValidateUser(creds.User, creds.Password);
        }

        public DataContracts.TokenContract GenerateToken(string User)
        {
            Guid UserId;
            MembershipUser memuser;
            memuser = Membership.GetUser(User);
            Guid.TryParse(memuser.ProviderUserKey.ToString(), out UserId);
            var Token = BuildSecureToken(TokenSize);
            using (DAL.AuthenticateDAL objMem = new DAL.AuthenticateDAL())
            {
                var nTokenRecord = new DataContracts.TokenContract()
                {
                    UserID = UserId.ToString(),
                    UserName = memuser.UserName,
                    Token = Token,
                    ExpiryDate = DateTime.Now.Add(new TimeSpan(0, 0, DefaultSecondsUntilTokenExpires))
                };

                if (!objMem.SaveToken(nTokenRecord))
                {
                    throw new WebFaultException<string>("Token Generation Failed.", System.Net.HttpStatusCode.InternalServerError);
                }
                memuser = null;
                return nTokenRecord;
            }
                
        }

        public bool ValidateToken(string Token)
        {
            if (Token == null)
            {
                return false;
            }
            else
            {
                using (DAL.AuthenticateDAL objMem = new DAL.AuthenticateDAL())
                {
                    var nTokenRecord = objMem.GetToken(Token);

                    if (nTokenRecord != null)
                    {
                        DateTime currentTimeUTC;
                        currentTimeUTC = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now, TimeZoneInfo.Local);
                        var span = currentTimeUTC - nTokenRecord.ExpiryDate.ToUniversalTime();
                        if (span.TotalSeconds > 0)
                        {
                            return false;
                        }
                        else
                        {
                            nTokenRecord.ExpiryDate = DateTime.Now.Add(new TimeSpan(0, 0, DefaultSecondsUntilTokenExpires));
                            return objMem.UpdateToken(nTokenRecord);
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public bool ValidateAccess(string User, string[] Role)
        {
            //List<DataContracts.UserRoles> RolesInUser = GetUserRoles(UserId);
            bool AccessGranted = false;
            MembershipUser memuser;
            memuser = Membership.GetUser(User);
            string[] userRoles = Roles.GetRolesForUser(memuser.UserName);
            foreach(var r in Role)
            {
                if(userRoles.Contains(r))
                {
                    AccessGranted = true;
                    break;
                }
            }
            return AccessGranted;
        }

        private string BuildSecureToken(int length)
        {
            var buffer = new byte[length];
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                rngCryptoServiceProvider.GetNonZeroBytes(buffer);
            }
            return Convert.ToBase64String(buffer);
        }

        //public List<DataContracts.UserRoles> GetUserRoles(Guid UserId)
        //{

        //    using (DAL.AuthenticateDAL objMem = new DAL.AuthenticateDAL())
        //    {
        //        return objMem.GetRoles(UserId);
        //    }
        //}
        
    }
}
