using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL
{
    public class BL_Mapping : IDisposable
    {
        public void Dispose()
        { }

        public List<DataContracts.Mapping.DC_CrossSystemMapping> Master_Get_Cross_Mapping(string SourceSystem, string SourceEntity, string SourceValue, string TargetSystem)
        {
            using (DAL.DL_Mapping objBL = new DAL.DL_Mapping())
            {
                return objBL.Master_Get_Cross_Mapping(SourceSystem, SourceEntity, SourceValue, TargetSystem);
            }
        }

        public List<DataContracts.Mapping.CityMappingRS> Get_City_Mapping(DataContracts.Mapping.CityMappingRQ RQ)
        {
            using (DAL.DL_Mapping obj = new DAL.DL_Mapping())
            {
                return obj.Get_City_Mapping(RQ);
            }
        }
    }
}
