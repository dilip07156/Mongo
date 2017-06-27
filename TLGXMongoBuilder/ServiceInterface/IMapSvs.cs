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
    public interface IMapSvs : IMasters, ILoadData, IMapping, IUpdateData
    {
        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebInvoke(Method = "POST", UriTemplate = "Authenticate", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        DataContracts.TokenContract AuthenicateUser(DataContracts.Credentials creds);

        //[OperationContract]
        //[FaultContract(typeof(DataContracts.ErrorNotifier))]
        //[WebGet(UriTemplate = "hotelsws/api/supplier/{supplier_name}/id/{hotel_id}/format/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //DataContracts.ProductDetails GetProductDetails(string ID);

        //[OperationContract]
        //[FaultContract(typeof(DataContracts.ErrorNotifier))]
        //[WebGet(UriTemplate = "hotelsws/api/supplier/{suppliername}/city/{city_name}/format/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //DataContracts.ProductDetails GetProductDetails(string ID);

        //[OperationContract]
        //[FaultContract(typeof(DataContracts.ErrorNotifier))]
        //[WebGet(UriTemplate = "hotelsws/api/supplier/{suppliername}/id/{hotel_id}/city/{city_name}/format/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //DataContracts.ProductDetails GetProductDetails(string ID);

        
    }
}
