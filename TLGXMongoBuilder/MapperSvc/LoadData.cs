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

        #region Product Mapping Push
        public void LoadProductMapping(string LogId, string MapId)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadProductMapping(LogId,MapId);
            }
        }

        public void LoadProductMappingLite(string LogId, string MapId)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadProductMappingLite(LogId, MapId);
            }
        }
        #endregion

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


        public void LoadAccoStaticData(string log_id, string supplier_id)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadAccoStaticData(log_id, supplier_id);
            }
        }

        public void LoadHotelMapping(string LogId)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadHotelMapping(LogId);
            }
        }

        public void UpdateAccoStaticDataSingleColumn()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.UpdateAccoStaticDataSingleColumn();
            }
        }


        #region ZoneMaster
        public void LoadZoneMaster(string LogId)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadZoneMaster(LogId);
            }
        }
        #endregion

        #region ZoneType Master
        public void LoadZoneTypeMaster(string LogId)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadZoneTypeMaster(LogId);
            }
        }
        #endregion
    }
}