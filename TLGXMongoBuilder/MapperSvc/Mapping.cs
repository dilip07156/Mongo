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
        public List<DataContracts.Mapping.DC_CrossSystemMapping> Master_Get_Cross_Mapping(string SourceSystem, string SourceEntity, string SourceValue, string TargetSystem)
        {
            using (BAL.BL_Mapping objBL = new BAL.BL_Mapping())
            {
                return objBL.Master_Get_Cross_Mapping(SourceSystem, SourceEntity, SourceValue, TargetSystem);
            }
        }

        public List<DataContracts.Mapping.CityMappingRS> Get_City_Mapping(DataContracts.Mapping.CityMappingRQ RQ)
        {
            using (BAL.BL_Mapping obj = new BAL.BL_Mapping())
            {
                return obj.Get_City_Mapping(RQ);
            }
        }
    }
}