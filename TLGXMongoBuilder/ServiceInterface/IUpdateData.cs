using System;
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
    public interface IUpdateData
    {
        #region Activity Mapping & Lite
        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Insert/ActivityMapping/MapId/{MapId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Insert_ActivityMapping_ByMapId(string MapId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Update/ActivityMapping/MapId/{MapId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Update_ActivityMapping_ByMapId(string MapId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Delete/ActivityMapping/MapId/{MapId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Delete_ActivityMapping_ByMapId(string MapId);
        #endregion

        #region Country Mapping
        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Insert/CountryMapping/MapId/{MapId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Insert_CountryMapping_ByMapId(string MapId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Update/CountryMapping/MapId/{MapId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Update_CountryMapping_ByMapId(string MapId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Delete/CountryMapping/MapId/{MapId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Delete_CountryMapping_ByMapId(string MapId);
        #endregion

        #region City Mapping
        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Upsert/CityMapping/MapId/{MapId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Upsert_CityMapping_ByMapId(string MapId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Delete/CityMapping/MapId/{MapId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Delete_CityMapping_ByMapId(string MapId);
        #endregion

        #region Hotel Mapping & Lite
        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Insert/HotelMapping/MapId/{MapId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Insert_HotelMapping_ByMapId(string MapId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Update/HotelMapping/MapId/{MapId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Update_HotelMapping_ByMapId(string MapId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Delete/HotelMapping/MapId/{MapId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Delete_HotelMapping_ByMapId(string MapId);
        #endregion

        #region Country Master
        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Insert/CountryMaster/Code/{Code}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Insert_CountryMaster_ByCode(string Code);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Update/CountryMaster/Code/{Code}/{OldCode}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Update_CountryMaster_ByCode(string Code, string OldCode);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Delete/CountryMaster/Code/{Code}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Delete_CountryMaster_ByCode(string Code);
        #endregion

        #region City Master
        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Insert/CityMaster/Code/{Code}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Insert_CityMaster_ByCode(string Code);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Update/CityMaster/Code/{Code}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Update_CityMaster_ByCode(string Code);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Delete/CityMaster/Code/{Code}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Delete_CityMaster_ByCode(string Code);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Upsert/CityMaster/Code/{Code}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Upsert_CityMaster_ByCode(string Code);
        #endregion

        #region Supplier Master
        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Insert/SupplierMaster/Code/{Code}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Insert_SupplierMaster_ByCode(string Code);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Upsert/SupplierMaster/Code/{Code}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Upsert_SupplierMaster_ByCode(string Code);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Delete/SupplierMaster/Code/{Code}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Delete_SupplierMaster_ByCode(string Code);
        #endregion

        #region Hotel Master
        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Insert/HotelMaster/HotelId/{HotelId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Insert_HotelMaster_ByHotelId(string HotelId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Update/HotelMaster/HotelId/{HotelId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Update_HotelMaster_ByHotelId(string HotelId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Delete/HotelMaster/HotelId/{HotelId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Delete_HotelMaster_ByHotelId(string HotelId);
        #endregion

        #region Activity Master
        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Insert/ActivityMaster/ActivityId/{ActivityId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Insert_ActivityMaster_ByActivityId(string ActivityId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Update/ActivityMaster/ActivityId/{ActivityId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Update_ActivityMaster_ByActivityId(string ActivityId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Delete/ActivityMaster/ActivityId/{ActivityId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Delete_ActivityMaster_ByActivityId(string ActivityId);
        #endregion

        #region Activity Definition
        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Sync/ActivityDefinition/{ActivityFlavourId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Sync_ActivityDefinition_ByActivityFlavourId(string ActivityFlavourId);
        #endregion

        #region Room Type Mapping
        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Upsert/RoomTypeMapping/MapId/{MapId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Upsert_RoomTypeMapping_ByMapId(string MapId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Delete/RoomTypeMapping/MapId/{MapId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Delete_RoomTypeMapping_ByMapId(string MapId);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Upsert/CompanySpecificRoomTypeMapping/MapId/{MapId}/{SupplierProductReference}/{Supplier_id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Upsert_CompanySpecificRoomTypeMapping_ByMapId(string MapId,string SupplierProductReference, string Supplier_id);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Delete/CompanySpecificRoomTypeMapping/MapId/{MapId}/{SupplierProductReference}/{Supplier_id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Delete_CompanySpecificRoomTypeMapping_ByMapId(string MapId,string SupplierProductReference, string Supplier_id);
        #endregion
    }
}
