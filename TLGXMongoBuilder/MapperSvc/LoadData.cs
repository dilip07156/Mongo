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
using System.IO;

namespace MapperSvc
{
    public partial class MapperSvc : IMapSvs
    {
        public void LoadHotelDefinition()
        {
            //string[] RoleAccess = new string[] { "Administrator" };
            //using (BAL.MemberAuthentication objAuth = new BAL.MemberAuthentication())
            //{
            //    if (!objAuth.ValidateToken(WebOperationContext.Current.IncomingRequest.Headers["Token"]))
            //    {
            //        throw new WebFaultException<string>("UnAuthorized Access (Invalid Token)", System.Net.HttpStatusCode.Unauthorized);
            //    }

            //    if (!objAuth.ValidateAccess(WebOperationContext.Current.IncomingRequest.Headers["User"], RoleAccess))
            //    {
            //        throw new WebFaultException<string>("Access Denied!", System.Net.HttpStatusCode.Unauthorized);
            //    }
            //}

            using (BAL.ProductBL objBL = new BAL.ProductBL())
            {
                objBL.LoadHotelDefinitions();
            }
        }

        public void LoadActivityDefinition()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadActivityDefinition();
            }
        }

        public void LoadActivityMasters()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadActivityMasters();
            }
        }

        public void LoadCountryMaster()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadCountryMaster();
            }
        }

        public void LoadCityMaster()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadCityMaster();
            }
        }

        public void LoadSupplierMaster()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadSupplierMaster();
            }
        }

        public void LoadCountryMapping()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadCountryMapping();
            }
        }

        public void LoadCityMapping()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadCityMapping();
            }
        }

        public void LoadProductMapping()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadProductMapping();
            }
        }

        public void LoadProductMappingLite()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadProductMappingLite();
            }
        }

        public void LoadActivityMapping()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadActivityMapping();
            }
        }

        public void LoadActivityMappingLite()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadActivityMappingLite();
            }
        }

        public void LoadRoomTypeMapping()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadRoomTypeMapping();
            }
        }

        public void LoadKeywords()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadKeywords();
            }
        }

    }
}