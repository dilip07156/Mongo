using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataContracts.Mapping
{
    [DataContract]
    [BsonIgnoreExtraElements]
    public class DC_SupplierMCONS
    {
        [BsonIgnore]
        [DataMember]
        public string SupplierName { get; set; }

        [DataMember]
        public string HoldInsertOnlineRoomTrypeMapping { get; set; }

        
        [DataMember]
        public string HoldReadOnlineRoomTrypeMappingData { get; set; }
    }
}
