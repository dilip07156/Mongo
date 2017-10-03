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
    [BsonIgnoreExtraElements]
    public class DC_RoomTypeMapping
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierProductCode { get; set; }
        public string SupplierRoomTypeCode { get; set; }
        public string SupplierRoomTypeName { get; set; }
        public string SystemRoomTypeMapId { get; set; }
        public string SystemProductCode { get; set; }
        public string SystemRoomTypeCode { get; set; }
        public string SystemRoomTypeName { get; set; }
        public string SystemNormalizedRoomType { get; set; }
        public string SystemStrippedRoomType { get; set; }
        //public DC_RoomTypeMapping_AlternateRoomNames AlternateRoomNames { get; set; }
        public List<DC_RoomTypeMapping_Attributes> Attibutes { get; set; }
        public string Status { get; set; }
    }

    //public class DC_RoomTypeMapping_AlternateRoomNames
    //{
    //    public string Normalized;
    //    public string Stripped;
    //}

    public class DC_RoomTypeMapping_Attributes
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }

}
