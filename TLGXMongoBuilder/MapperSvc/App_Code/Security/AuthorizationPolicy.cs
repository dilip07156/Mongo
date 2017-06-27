using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IdentityModel.Policy;
using System.IdentityModel.Claims;
using System.Security.Principal;

namespace MapperSvc.App_Code.Security
{
    public class AuthorizationPolicy : IAuthorizationPolicy
    {
        public string Id
        {
            get
            {
                return Guid.NewGuid().ToString();
            }
        }

        public ClaimSet Issuer
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        // this method gets called after the authentication stage
        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            // get the authenticated client identity
            IIdentity client = GetClientIdentity(evaluationContext);
            // set the custom principal
            evaluationContext.Properties["Principal"] = new CustomPrincipal(client);
            return true;
        }

        private IIdentity GetClientIdentity(EvaluationContext evaluationContext)
        {
            object obj;
            if (!evaluationContext.Properties.TryGetValue("Identities", out obj))
                throw new Exception("No Identity found");
            IList<IIdentity> identities = obj as IList<IIdentity>;
            if (identities == null || identities.Count <= 0)
                throw new Exception("No Identity found");
            return identities[0];
        }
    }
}