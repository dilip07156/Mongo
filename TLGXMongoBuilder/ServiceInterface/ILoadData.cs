﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataContracts;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using Newtonsoft.Json;
using MongoDB.Bson;

namespace ServiceContracts
{
    [ServiceContract]
    public interface ILoadData
    {
        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/HotelDefinition", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadHotelDefinition();

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/ActivityDefinition", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadActivityDefinition();

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/VisaMapping/{LogId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void UpdateVisaDefinition(string LogId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/HolidayMapping/{LogId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void UpdateHolidayMapping(string LogId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/ActivityDefinitionBySupplier/{LogId}/{suppliername}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadActivityDefinitionBySupplier(string LogId, string suppliername);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Update/ActivityCategoryTypes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void UpdateActivityCategoryTypes();

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Update/ActivityInterestType", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void UpdateActivityInterestType();
        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Update/ActivityDOW", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void UpdateActivityDOW();

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Update/ActivitySpecial", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void UpdateActivitySpecial();

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Update/ActivityPrices", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void UpdateActivityPrices();

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Update/ActivityDescription", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void UpdateActivityDescription();
        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/ActivityMasters", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadActivityMasters();

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/CountryMaster/{LogId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadCountryMaster(string LogId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/CityMaster/{LogId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadCityMaster(string LogId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/SupplierMaster/{LogId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadSupplierMaster(string LogId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/CountryMapping/{LogId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadCountryMapping(string LogId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "LoadBySupplier/CountryMapping/{LogId}/{Supplier_ID}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadCountryMappingBySupplier(string LogId, string Supplier_ID);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/CityMapping/{LogId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadCityMapping(string LogId);

        #region MPUSH By UI

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "LoadByObject/EntityMapping/{Entity}/{EntityMappingID}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadObjectMapping(string Entity, string EntityMappingID);

        #endregion

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "LoadBySupplier/CityMapping/{LogId}/{Supplier_ID}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadCityMappingBySupplier(string LogId, string Supplier_ID);

        #region Product Mapping Push
        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/ProductMapping/{LogId}/{MapId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadProductMapping(string LogId, string MapId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/ProductMappingLite/{LogId}/{MapId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadProductMappingLite(string LogId, string MapId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/LoadCompanyAccommodationProductMappingOnSave/{LogId}/{MapId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadCompanyAccommodationProductMappingOnSave(string LogId, string MapId);


        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/LoadCompanyAccommodationProductMapping/{LogId}/{Supplier_ID}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadCompanyAccommodationProductMapping(string LogId, string Supplier_ID);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/LoadCompanyAccommodationCountryWiseProductMapping/{LogId}/{Supplier_ID}/{Country_ID}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadCompanyAccommodationCountryWiseProductMapping(string LogId, string Supplier_ID, string Country_ID);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/LoadCompanyAccommodationProductMappingCrossVersion/{LogId}/{Supplier_ID}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadCompanyAccommodationProductMappingCrossVersion(string LogId, string Supplier_ID);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/LoadCompanyAccommodationProductMappingCrossVersion_CountryWise/{LogId}/{Supplier_ID}/{Country_Id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadCompanyAccommodationProductMappingCrossVersion_CountryWise(string LogId, string Supplier_ID, string Country_Id);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/LoadSupplierMCONS/{LogId}?Supplier_ID={Supplier_ID}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadMCONBySupplier(string LogId, string Supplier_ID = "");


        [OperationContract]
        [FaultContract(typeof(ErrorNotifier))]
        [WebGet(UriTemplate = "LoadBySupplier/ProductMapping/{LogId}/{Supplier_ID}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadProductMappingBySupplier(string LogId, string Supplier_ID);

        [OperationContract]
        [FaultContract(typeof(ErrorNotifier))]
        [WebGet(UriTemplate = "LoadBySupplier/ProductMappingLite/{LogId}/{Supplier_ID}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadProductMappingLiteBySupplier(string LogId, string Supplier_ID);
        #endregion

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/ActivityMapping/{LogId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadActivityMapping(string LogId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/ActivityMappingLite", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadActivityMappingLite();

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/KeywordMaster", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadKeywords();

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/StateMaster/{LogId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadStates(string LogId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/PortMaster/{LogId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadPorts(string LogId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/AccoStaticData/{LogId}/{SupplierId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadAccoStaticData(string LogId, string supplierId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/UpdateAccoStaticDataSingleColumn", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void UpdateAccoStaticDataSingleColumn();

        #region ZoneMaster
        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/ZoneMasters/{LogId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadZoneMaster(string LogId);
        #endregion

        #region SupplierZoneMaster
        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/SupplierZoneMasters/{LogId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadSupplierZoneMaster(string LogId);
        #endregion

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/HotelRoomTypeMapping/{LogId}/{Supplier_Id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void UpdateHotelRoomTypeMapping(string LogId, string Supplier_Id);

        #region ZoneTypeMaster
        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/ZoneTypeMaster/{LogId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadZoneTypeMaster(string LogId);
        #endregion


        /// <summary>
        /// Load Master Accommodation Data to MongoDB
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/MasterAccommodation/{LogId}/{Accommodation_Id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadMasterAccommodation(string LogId, string Accommodation_Id);

        /// <summary>
        /// Load Master AccommodationRoomInfo Data to MongoDB
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/MasterAccommodationRoomInfo/{LogId}/{Accommodation_Id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadMasterAccommodationRoomInfo(string LogId, string Accommodation_Id);
    }
}
