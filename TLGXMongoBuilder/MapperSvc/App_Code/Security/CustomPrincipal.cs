using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Web.Security;

namespace MapperSvc.App_Code.Security
{
    public class CustomPrincipal : IPrincipal
    {

        private IIdentity _identity;
        private string[] _roles;

        public CustomPrincipal(IIdentity client)
        {
            _identity = client;
        }

        public IIdentity Identity
        {
            get
            {
                return _identity;
            }
        }

        public bool IsInRole(string role)
        {
           return Roles.IsUserInRole(_identity.Name, role);
        }
    }
}