using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace DataContracts.Mapping
{
    [DataContract]
    [BsonIgnoreExtraElements]
    public class DC_ProductMappingLite
    {
        [DataMember]
        public string SupplierCode { get; set; }
        [DataMember]
        public string SupplierProductCode { get; set; }
        [DataMember]
        public int MapId { get; set; }
        [DataMember]
        public string SystemProductCode { get; set; }
    }
}
