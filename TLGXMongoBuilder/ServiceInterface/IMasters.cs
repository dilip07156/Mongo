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
    public interface IMasters
    {
        

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Masters/Get/Countries", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DataContracts.Masters.DC_Country> Master_GetCountries();

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Masters/Get/Countries/CountryCode/{CountryCode}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DataContracts.Masters.DC_Country> Master_GetCountries_ByCountryCode(string CountryCode);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Masters/Get/Countries/CountryName/{CountryName}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DataContracts.Masters.DC_Country> Master_GetCountries_ByCountryName(string CountryName);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Masters/Get/Cities/CountryName/{CountryName}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DataContracts.Masters.DC_City> Master_GetCities_ByCountryName(string CountryName);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Masters/Get/Cities/CountryCode/{CountryCode}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DataContracts.Masters.DC_City> Master_GetCities_ByCountryCode(string CountryCode);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Masters/Get/Supplier/Name/{Name}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DataContracts.Masters.DC_Supplier> Master_GetSupplier_ByName(string Name);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Masters/Get/Supplier/Code/{Code}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DataContracts.Masters.DC_Supplier> Master_GetSupplier_ByCode(string Code);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Masters/Get/Supplier/All", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DataContracts.Masters.DC_Supplier> Master_GetAllSupplier();

        //[OperationContract]
        //[FaultContract(typeof(DataContracts.ErrorNotifier))]
        //[WebGet(UriTemplate = "Masters/GetCountries/format/xml", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        //List<DataContracts.Masters.DC_Country> Master_GetCountriesXml();

        //[OperationContract]
        //[FaultContract(typeof(DataContracts.ErrorNotifier))]
        //[WebGet(UriTemplate = "Masters/GetCountries/format/jsonp", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //List<DataContracts.Masters.DC_Country> Master_GetCountriesJsonp();


    }
}
