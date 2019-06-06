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
        public string HoldInsertOnlineRoomTypeMappingData { get; set; }

        
        [DataMember]
        public string HoldReadOnlineRoomTypeMappingData { get; set; }
    }
}
