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

        public void UpdateActivityCategoryTypes()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.UpdateActivityCategoryTypes();
            }
        }

        public void LoadActivityMasters()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadActivityMasters();
            }
        }

        public void LoadCountryMaster(string LogId)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadCountryMaster(LogId);
            }
        }

        public void LoadCityMaster(string LogId)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadCityMaster(LogId);
            }
        }

        public void LoadSupplierMaster(string LogId)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadSupplierMaster(LogId);
            }
        }

        public void LoadCountryMapping(string LogId)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadCountryMapping(LogId);
            }
        }

        public void LoadCityMapping(string LogId)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadCityMapping(LogId);
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

        public void LoadActivityMapping(string LogId)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadActivityMapping(LogId);
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


        public void LoadStates(string LogId)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadStates(LogId);
            }
        }

        public void LoadPorts(string LogId)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadPorts(LogId);
            }
        }


        public void LoadAccoStaticData()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadAccoStaticData();
            }
        }

        public void LoadHotelMapping(string LogId)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadHotelMapping(LogId);
            }
        }

    }
}