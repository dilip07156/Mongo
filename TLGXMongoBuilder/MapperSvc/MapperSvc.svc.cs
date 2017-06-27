using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using DataContracts;
using Newtonsoft.Json;
using ServiceContracts;
using System.Security.Permissions;
using MongoDB.Bson;

namespace MapperSvc
{
    public partial class MapperSvc : ServiceContracts.IMapSvs
    {
        DataContracts.TokenContract IMapSvs.AuthenicateUser(Credentials creds)
        {
            if (creds.User == null || creds.Password == null)
            {
                throw new WebFaultException<string>("Please provide user name and password.", System.Net.HttpStatusCode.BadRequest);
            }
            else
            {
                using (BAL.MemberAuthentication objAuth = new BAL.MemberAuthentication())
                {
                    if (objAuth.AuthenticateUser(creds))
                    {
                        DataContracts.TokenContract objToken = new TokenContract();
                        objToken = objAuth.GenerateToken(creds.User);
                        return objToken;
                    }
                    else
                    {
                        throw new WebFaultException<string>("Invalid user name or password.", System.Net.HttpStatusCode.Unauthorized);
                    }
                }
            }
        }

        //public DataContracts.ProductDetails GetProductDetails(string ID)
        //{
        //    string[] RoleAccess = new string[] { "Administrator", "Agent" };
        //    using (BAL.MemberAuthentication objAuth = new BAL.MemberAuthentication())
        //    {
        //        if (!objAuth.ValidateToken(WebOperationContext.Current.IncomingRequest.Headers["Token"]))
        //        {
        //            throw new WebFaultException<string>("UnAuthorized Access (Invalid Token)", System.Net.HttpStatusCode.Unauthorized);
        //        }

        //        if (!objAuth.ValidateAccess(WebOperationContext.Current.IncomingRequest.Headers["User"], RoleAccess))
        //        {
        //            throw new WebFaultException<string>("Access Denied!", System.Net.HttpStatusCode.Unauthorized);
        //        }
        //    }


        //    try
        //    {
        //        using (BAL.ProductBL objBL = new BAL.ProductBL())
        //        {
        //            return objBL.GetProductDetails(ID);
        //        }
        //    }
        //    catch (FaultException<DataContracts.ErrorNotifier> ex)
        //    {
        //        throw new WebFaultException<string>(ex.Detail.ErrorMessage, ex.Detail.ErrorStatusCode);
        //    }


        //}

    }
}
