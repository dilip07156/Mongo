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
        [DataMember]
        public string TlgxMdmHotelId { get; set; }
    }

    public class DC_ProductMappingLite_WithStatus
    {
        public string SupplierCode { get; set; }
        public string SupplierProductCode { get; set; }
        public int MapId { get; set; }
        public string SystemProductCode { get; set; }
        public string TlgxMdmHotelId { get; set; }
        public string MappingStatus { get; set; }
    }

}
