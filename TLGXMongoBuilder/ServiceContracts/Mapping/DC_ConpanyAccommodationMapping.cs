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
    public class DC_ConpanyAccommodationMapping
    {
        [BsonIgnore]
        [DataMember]
        public Guid SupplierId { get; set; }

        [BsonId]
        [DataMember]
        [Newtonsoft.Json.JsonProperty("_id")]
        public string _id { get; set; }


        [DataMember]
        public String SupplierCode { get; set; }

        [DataMember]
        public string SupplierProductCode { get; set; }

        [DataMember]
        public string SupplierProductName { get; set; }

        [DataMember]
        public string CompanyProductName { get; set; }

        [DataMember]
        public string CompanyProductId { get; set; }

        [DataMember]
        public string CommonProductId { get; set; }

        [DataMember]
        public string TLGXCompanyId { get; set; }

        [DataMember]
        public string Rating { get; set; }

        [DataMember]
        public string TLGXCompanyName { get; set; }

        [DataMember]
        public string CountryName { get; set; }

        [DataMember]
        public string CityName { get; set; }

        [DataMember]
        public string StateName { get; set; }

        [DataMember]
        public bool IsRoomMappingCompleted { get; set; }

        [DataMember]
        public bool IsDirectContract { get; set; }

        //[DataMember]
        //public string NakshatraMappingId { get; set; }

        [DataMember]
        public string ProductCategorySubType { get; set; }

        [DataMember]
        public string Brand { get; set; }

        [DataMember]
        public string Chain { get; set; }

        [DataMember]
        public string Interest { get; set; }

        [BsonIgnore]
        [DataMember]
        public Guid Accommodation_CompanyVersion_Id { get; set; }

        [DataMember]
        public List<DC_ConpanyAccommodationRoomMapping> MappedRooms { get; set; }

        //Collection of Room data from version data
    }

    [DataContract]
    [BsonIgnoreExtraElements]
    public class DC_ConpanyAccommodationRoomMapping
    {
        [DataMember]
        public string SupplierRoomId { get; set; }

        [DataMember]
        public string SupplierRoomTypeCode { get; set; }

        [DataMember]
        public string SupplierRoomName { get; set; }

        [DataMember]
        public string SupplierRoomCategory { get; set; }

        [DataMember]
        public string SupplierRoomCategoryId { get; set; }

        [DataMember]
        public string CompanyRoomId { get; set; }

        [DataMember]
        public string TLGXCommonRoomId { get; set; }

        [DataMember]
        public string CompanyRoomName { get; set; }

        [DataMember]
        public string CompanyRoomCategory { get; set; }

        [DataMember]
        public string NakshatraRoomMappingId { get; set; }

        [BsonIgnore]
        [DataMember]
        public Guid Accommodation_CompanyVersion_Id { get; set; }

        [BsonIgnore]
        [DataMember]
        public string SupplierProductId { get; set; }

        [BsonIgnore]
        [DataMember]
        public Guid Supplier_Id { get; set; }
    }
}
