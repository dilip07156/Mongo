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
    public class DC_ProductMapping
    {
        [DataMember]
        public string SupplierCode { get; set; }
        [DataMember]
        public string SupplierProductCode { get; set; }
        [DataMember]
        public string SupplierCountryCode { get; set; }
        [DataMember]
        public string SupplierCountryName { get; set; }
        [DataMember]
        public string SupplierCityCode { get; set; }
        [DataMember]
        public string SupplierCityName { get; set; }
        [DataMember]
        public string SupplierProductName { get; set; }
        [DataMember]
        public string MappingStatus { get; set; }
        [DataMember]
        public int MapId { get; set; }
        [DataMember]
        public string SystemProductCode { get; set; }
        [DataMember]
        public string SystemProductName { get; set; }
        [DataMember]
        public string SystemCountryName { get; set; }
        [DataMember]
        public string SystemCityName { get; set; }
        [DataMember]
        public string SystemCountryCode { get; set; }
        [DataMember]
        public string SystemCityCode { get; set; }
        [DataMember]
        public string SystemProductType { get; set; }
    }
}
