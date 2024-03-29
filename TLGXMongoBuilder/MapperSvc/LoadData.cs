﻿using System;
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

        public void UpdateVisaDefinition(string LogId)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.UpdateVisaDefinition(LogId);
            }

        }

        public void UpdateHolidayMapping(string LogId)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.UpdateHolidayMapping(LogId);
            }

        }

        public void LoadActivityDefinition()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadActivityDefinition();
            }
        }
        public void LoadActivityDefinitionBySupplier(string log_id, string suppliername)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadActivityDefinitionBySupplier(log_id, suppliername);
            }

        }
        public void UpdateActivityCategoryTypes()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.UpdateActivityCategoryTypes();
            }
        }

        public void UpdateActivityInterestType()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.UpdateActivityInterestType();
            }
        }

        public void UpdateActivityDOW()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.UpdateActivityDOW();
            }
        }

        public void UpdateActivitySpecial()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.UpdateActivitySpecial();
            }
        }

        public void UpdateActivityPrices()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.UpdateActivityPrices();
            }
        }

        public void UpdateActivityDescription()
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.UpdateActivityDescription();
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

        public void LoadObjectMapping(string Entity, string EntityMappingID)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadObjectMapping(Entity, EntityMappingID);
            }
        }

        public void LoadCountryMappingBySupplier(string LogId, string Supplier_ID)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadCountryMapping(LogId, Supplier_ID);
            }
        }

        public void LoadCityMapping(string LogId)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadCityMapping(LogId);
            }
        }

        public void LoadCityMappingBySupplier(string LogId, string Supplier_ID)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadCityMapping(LogId, Supplier_ID);
            }
        }

        #region Product Mapping Push
        public void LoadProductMapping(string LogId, string MapId)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadProductMapping(LogId, MapId);
            }
        }

        public void LoadProductMappingBySupplier(string LogId, string Supplier_ID)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadProductMappingBySupplier(LogId, Supplier_ID);
            }
        }

        public void LoadProductMappingLite(string LogId, string MapId)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadProductMappingLite(LogId, MapId);
            }
        }

        public void LoadCompanyAccommodationProductMappingOnSave(string LogId, string MapId)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadCompanyAccommodationProductMappingOnSave(LogId, MapId);
            }
        }
        

        public void LoadProductMappingLiteBySupplier(string LogId, string Supplier_ID)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadProductMappingLiteBySupplier(LogId, Supplier_ID);
            }
        }

        public void LoadCompanyAccommodationProductMapping(string LogId,string Supplier_ID)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadCompanyAccommodationProductMapping(LogId, Supplier_ID);
            }
        }

        public void LoadCompanyAccommodationCountryWiseProductMapping(string LogId, string Supplier_ID, string Country_ID)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadCompanyAccommodationCountryWiseProductMapping(LogId, Supplier_ID, Country_ID);
            }
        }

        public void LoadCompanyAccommodationProductMappingCrossVersion(string LogId, string Supplier_ID)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadCompanyAccommodationProductMappingCrossVersion(LogId, Supplier_ID);
            }
        }

        public void LoadCompanyAccommodationProductMappingCrossVersion_CountryWise(string LogId, string Supplier_ID, string Country_Id)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadCompanyAccommodationProductMappingCrossVersion_CountryWise(LogId, Supplier_ID, Country_Id);
            }
        }

        public void LoadMCONBySupplier(string LogId, string Supplier_ID = "00000000-0000-0000-0000-000000000000")
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadMCONBySupplier(LogId, Supplier_ID);
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

        #region SupplierZoneMaster
        public void LoadSupplierZoneMaster(string LogId)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadSupplierZoneMaster(LogId);
            }
        }
        #endregion

        public void UpdateHotelRoomTypeMapping(string LogId, string Supplier_Id)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.UpdateHotelRoomTypeMapping(LogId, Supplier_Id);
            }
        }

        public void UpdateComanySpecificHotelRoomTypeMapping(string LogId, string Supplier_Id)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.UpdateComanySpecificHotelRoomTypeMapping(LogId, Supplier_Id);
            }
        }

        #region ZoneType Master
        public void LoadZoneTypeMaster(string LogId)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadZoneTypeMaster(LogId);
            }
        }
        #endregion

        public void LoadMasterAccommodation(string LogId, string Accommodation_Id)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadMasterAccommodation(LogId, Accommodation_Id);
            }
        }


        public void LoadMasterAccommodationRoomInfo(string LogId, string Accommodation_Id)
        {
            using (BAL.BL_LoadData objBL = new BAL.BL_LoadData())
            {
                objBL.LoadMasterAccommodationRoomInfo(LogId, Accommodation_Id);
            }
        }
    }
}