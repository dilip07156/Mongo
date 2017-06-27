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
    public interface IMapping
    {


        #region Mapping

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebGet(UriTemplate = "Mapping/Get/Source/{SourceSystem}/Entity/{SourceEntity}/Value/{SourceValue}/Target/{TargetSystem}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DataContracts.Mapping.DC_CrossSystemMapping> Master_Get_Cross_Mapping(string SourceSystem, string SourceEntity, string SourceValue, string TargetSystem);

        [OperationContract]
        [FaultContract(typeof(DataContracts.ErrorNotifier))]
        [WebInvoke(UriTemplate = "Mapping/City", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DataContracts.Mapping.CityMappingRS> Get_City_Mapping(DataContracts.Mapping.CityMappingRQ RQ);

        #endregion
    }
}
