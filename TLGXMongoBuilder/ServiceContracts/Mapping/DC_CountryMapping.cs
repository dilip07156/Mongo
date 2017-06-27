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
    public class DC_CountryMapping
    {
        //[DataMember]
        //public string CountryMapping_Id { get; set; }

        //[DataMember]
        //public string Country_Id { get; set; }

        [DataMember]
        public string CountryName { get; set; }

        [DataMember]
        public string CountryCode { get; set; }

        //[DataMember]
        //public string Supplier_Id { get; set; }

        [DataMember]
        public string SupplierName { get; set; }

        [DataMember]
        public string SupplierCode { get; set; }

        [DataMember]
        public string SupplierCountryCode { get; set; }

        [DataMember]
        public string SupplierCountryName { get; set; }
        
        [DataMember]
        public int MapId { get; set; }

    }
}
