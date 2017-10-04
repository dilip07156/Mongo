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
    public interface ILoadData
    {
        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/HotelDefinition", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadHotelDefinition();

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/CountryMaster", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadCountryMaster();

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/CityMaster", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadCityMaster();

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/SupplierMaster", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadSupplierMaster();

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/CountryMapping", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadCountryMapping();

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/CityMapping", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadCityMapping();

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/ProductMapping", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadProductMapping();

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/ProductMappingLite", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadProductMappingLite();

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/ActivityMapping", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadActivityMapping();

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/ActivityMappingLite", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadActivityMappingLite();

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/RoomTypeMapping", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadRoomTypeMapping();

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Load/Keywords", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadKeywords();
    }
}
